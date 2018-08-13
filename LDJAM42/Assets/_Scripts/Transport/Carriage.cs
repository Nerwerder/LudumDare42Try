using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : MonoBehaviour
{
    //Diferent Models
    public Mesh basicMesh;
    public Mesh woodMesh;
    public Mesh grainMesh;

    //MATERIAL
    public Material basicMaterial, glowMaterial;
    public void SetBasicMaterial() { SetMaterial(basicMaterial); }
    public void SetGlowMaterial() { SetMaterial(glowMaterial); }
    public void SetMaterial(Material m) { this.GetComponent<Renderer>().material = m; }

    //BASIC
    private void Awake()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    private void Update()
    {
        switch (cState)
        {
            case carriageState.CarriageIdle:
                break;
            case carriageState.CarriageWaitForResource:
                InteractWithPosition(actualPosition);
                break;
            case carriageState.CarriageOnMyWay:
                if (followThePath(path))
                {
                    path.Clear();
                    InteractWithPosition(targetPosition);
                }
                break;
        }
    }

    //PATH
    private Pathfinding pathfinding;
    private ConnectionPoint actualPosition = null, targetPosition = null;
    Path path = new Path();

    public void SetActualPosition(ConnectionPoint c) { actualPosition = c; }

    public void GoTo(Building b)
    {
        Debug.Log("GoTo Building : " + b.type.ToString());
        var wB = b.GetComponent<WorkBuilding>();
        var ci = b.GetComponent<City>();

        switch (cCState)
        {
            case carriageCargoState.CarriageFull:
                if (wB && wB.inputLocation)             //It is a WorkBuilding and it has an Input
                    GoTo(wB.inputLocation);
                else if (ci)                            //It is a City
                    GoTo(ci.getFreeInputLocation());

                break;
            case carriageCargoState.CarriageEmpty:      //If the Cariage is Empty -> Go and Pick the Resource from the Output up
                if (wB)
                    GoTo(wB.outputLocation);
                break;
            default:
                break;
        }
    }
    public void GoTo(Place p)
    {
        Debug.Log("GoTo Place : " + p.type.ToString());

    }
    public void GoTo(ConnectionPoint p)
    {
        if (p)
        {
            cState = carriageState.CarriageOnMyWay;
            targetPosition = p;
            path.SetPath(pathfinding.FindPath(actualPosition, targetPosition));
        }
        else
            Debug.Log("Carriage: " + this.ToString() + " got a null as Target in Carriage.Goto(ConnectionPoint p)");

    }

    //TRAVEL
    enum carriageState { CarriageIdle, CarriageWaitForResource, CarriageOnMyWay };
    carriageState cState = carriageState.CarriageIdle;
    public float rotationSpeed = 5f;
    public float arrivalDistance = 0.2f;
    public bool followThePath(Path p)
    {
        var t = p.GetNetxtTarget();

        //ROTATE
        var targetRot = Quaternion.LookRotation((t.transform.position - this.transform.position), Vector3.up);
        var slerpRot = Quaternion.Slerp((this.transform.rotation), targetRot, rotationSpeed * Time.deltaTime);

        //TRANSLATE
        //Build the Vector with a Max Length of 1
        var v = t.transform.position - this.transform.position;
        //if (v.magnitude > 1)
        v.Normalize();
        Debug.DrawLine(this.transform.position, this.transform.position + v, Color.red);

        //Only Move if the target Field is Free (exeption: the target Field has a Resource you have to Pick up)
        Vector3 pos;
        if (t.FreeToMoveOn() || (t == targetPosition && t.ResourceWaitingForPickup())) //TODO Resource Waiting for Pickup
            pos = this.transform.position + v * Time.deltaTime;
        else
            pos = this.transform.position;

        this.transform.SetPositionAndRotation(pos, slerpRot);

        //ARRIVED
        var d = (t.transform.position - this.transform.position).magnitude;
        if (d <= arrivalDistance)
        {
            //Leave the Old Field
            actualPosition.CarriageMovesFromField(this);
            //Move to the New Field
            t.CarriageMovesOnField(this);
            return p.Arrive();
        }

        return false;
    }

    //MATERIALTRANSPORT
    public enum carriageCargoState { CarriageFull, CarriageEmpty };
    public carriageCargoState cCState = carriageCargoState.CarriageEmpty;
    public GameObject cargo;

    public void InteractWithPosition(ConnectionPoint f)
    {
        switch (cCState)
        {
            case carriageCargoState.CarriageFull:    //Try to Empty the Carriage (Lay Down the Resource)
                if (f.resourceInput && !f.resourceOnField)
                {
                    if (f.PushResource(cargo))
                    {
                        ChangeMesh(basicMesh);
                        cCState = carriageCargoState.CarriageEmpty;
                        cState = carriageState.CarriageIdle;
                    }
                }

                break;
            case carriageCargoState.CarriageEmpty:
                //Is there a Resource on this Field
                if (f.ResourceWaitingForPickup() && f.resourceOutput)
                {
                    cargo = f.PullResource();
                    cCState = carriageCargoState.CarriageFull;

                    var t = cargo.GetComponent<Resource>().type;
                    if (t == Resource.ResourceType.Wood || t == Resource.ResourceType.Planks)
                        ChangeMesh(woodMesh);
                    else if (t == Resource.ResourceType.Grain || t == Resource.ResourceType.Flour || t == Resource.ResourceType.Bread)
                        ChangeMesh(grainMesh);

                    cState = carriageState.CarriageIdle;
                }
                else if (f.resourceOutput)
                    cState = carriageState.CarriageWaitForResource;
                else
                    cState = carriageState.CarriageIdle;
                break;
        }
    }

    private void ChangeMesh(Mesh m)
    {
        this.GetComponent<MeshFilter>().mesh = m;
    }
}
