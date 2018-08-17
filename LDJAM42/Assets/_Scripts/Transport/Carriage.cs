using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : MonoBehaviour
{
    //Diferent Models
    public Mesh basicMesh;
    public Mesh woodMesh;
    public Mesh grainMesh;
    public Mesh plankMesh;

    public bool Logging;
    private bool drawPath;

    //MATERIAL
    public Material basicMaterial, glowMaterial;
    public void SetBasicMaterial() { SetMaterial(basicMaterial); }
    public void SetGlowMaterial() { SetMaterial(glowMaterial); }
    public void SetMaterial(Material m) { this.GetComponent<Renderer>().material = m; }
    public void SetDrawPath(bool t)
    {
        drawPath = t;
        if (!t)
        {
            if (drawn != null)
                drawn.StopDrawing();
            if (route != null)
                route.StopDrawing();
        }
    }

    //BASIC
    private void Awake()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
    }
    private void Update()
    {
        switch (carriageControlState)
        {
            case CarriageControlState.CarriageManual:
                switch (carriageState)
                {
                    case CarriageState.CarriageIdle:
                        break;
                    case CarriageState.CarriageWaitForResource:
                        InteractWithPosition(actualPosition);
                        break;
                    case CarriageState.CarriageOnMyWay:
                        if (followThePath(path))
                            InteractWithPosition(targetPosition);
                        break;
                }
                break;
            case CarriageControlState.CarriageRoute:
                FollowRoute();
                break;
        }

        if (drawPath)
            DrawPath();
    }

    //DRAW PATH
    public Material pathMaterial, deliveryMaterial, collectMaterial;
    Path drawn = null;
    private void DrawPath()
    {
        if (drawn != null && drawn != path)
            drawn.StopDrawing();

        switch (carriageControlState)
        {
            case CarriageControlState.CarriageManual:
                drawn = path;
                if (drawn != null)
                    drawn.Draw(pathMaterial);
                break;
            case CarriageControlState.CarriageRoute:
                //route.DrawRoute(pathMaterial, deliveryMaterial, collectMaterial);
                drawn = route.GetCurrentPath();
                if (drawn != null)
                    drawn.Draw(pathMaterial);
                break;
        }

    }


    //PATH
    private enum CarriageControlState { CarriageManual, CarriageRoute };
    private CarriageControlState carriageControlState = CarriageControlState.CarriageManual;
    private void ChangeCarriageControlState(CarriageControlState s)
    {
        if (Logging)
            Debug.Log("carriageState: " + carriageControlState.ToString() + " -> " + s.ToString());
        carriageControlState = s;
    }

    private Pathfinding pathfinding;
    private ConnectionPoint actualPosition = null, targetPosition = null;
    private Path path;

    public void SetActualPosition(ConnectionPoint c) { actualPosition = c; }
    public ConnectionPoint GetActualPosition() { return actualPosition; }
    //GOTO = Manual Mode
    public void GoTo(Building b)
    {
        if (carriageControlState == CarriageControlState.CarriageRoute)
            ClearRoute();

        var wB = b.GetComponent<WorkBuilding>();
        var ci = b.GetComponent<City>();

        switch (carriageCargoState)
        {
            case CarriageCargoState.CarriageFull:
                if (wB && wB.inputLocation)             //It is a WorkBuilding and it has an Input
                    GoTo(wB.inputLocation);
                else if (ci)                            //It is a City
                    GoTo(ci.getFreeInputLocation());

                break;
            case CarriageCargoState.CarriageEmpty:      //If the Cariage is Empty -> Go and Pick the Resource from the Output up
                if (wB)
                    GoTo(wB.outputLocation);
                break;
            default:
                break;
        }
    }
    public void GoTo(Place p)
    {
        if (carriageControlState == CarriageControlState.CarriageRoute)
            ClearRoute();

        GoTo(p.GetRandomFreeMOVELocation());
    }
    public void GoTo(ConnectionPoint p)
    {
        ChangeCarriageStateTO(CarriageState.CarriageOnMyWay);
        targetPosition = p;

        if (path != null)
            path.StopDrawing();

        path = new Path(pathfinding, actualPosition, p);
    }

    //ROUTE = Route Mode
    public GameObject RouteStationMarker;

    private Route route;
    public Route GetRoute() { return route; }
    public void AddToRoute(Place p)
    {
        ChangeCarriageControlState(CarriageControlState.CarriageRoute);
        if (route == null)
        {
            ChangeCarriageStateTO(CarriageState.CarriageOnMyWay);
            route = new Route(pathfinding, this);
        }

        route.AddPlace(p);
    }
    public void ClearRoute()
    {
        ChangeCarriageControlState(CarriageControlState.CarriageManual);
        route = null;
    }
    public void FollowRoute()
    {
        switch (carriageState)
        {
            case CarriageState.CarriageIdle:
                route.DoneWithStation();
                carriageState = CarriageState.CarriageOnMyWay;
                break;
            case CarriageState.CarriageWaitForResource:
                InteractWithPosition(actualPosition);
                break;
            case CarriageState.CarriageOnMyWay:
                if (followThePath(route.GetCurrentPath()))
                    InteractWithPosition(route.GetCurrentTargetPosition());
                break;
        }
    }

    //TRAVEL
    private enum CarriageState { CarriageIdle, CarriageWaitForResource, CarriageOnMyWay };
    private CarriageState carriageState = CarriageState.CarriageIdle;
    private void ChangeCarriageStateTO(CarriageState s)
    {
        if (Logging)
            Debug.Log("carriageState: " + carriageState.ToString() + " -> " + s.ToString());
        carriageState = s;
    }

    public float rotationSpeed = 5f;
    public float arrivalDistance = 0.2f;
    public bool followThePath(Path p)
    {
        var t = p.GetTarget();

        //ROTATE
        var targetRot = Quaternion.LookRotation((t.transform.position - this.transform.position), Vector3.up);
        var slerpRot = Quaternion.Slerp((this.transform.rotation), targetRot, rotationSpeed * Time.deltaTime);

        //TRANSLATE
        //Build the Vector with a Max Length of 1
        var v = t.transform.position - this.transform.position;
        v.Normalize();
        Debug.DrawLine(this.transform.position, this.transform.position + v, Color.red);

        //Only Move if the target Field is Free (exeption: the target Field has a Resource you have to Pick up)
        Vector3 pos;

        bool tp = (t == targetPosition || ((route != null) && (route.GetCurrentTargetPosition() == t))); //WORKAROUND
        if (t.FreeToMoveOn() || (tp && t.ResourceWaitingForPickup())) //TODO Resource Waiting for Pickup
            pos = this.transform.position + v * Time.deltaTime * actualPosition.getConnectionSpeed(t);
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
    public enum CarriageCargoState { CarriageFull, CarriageEmpty };
    private CarriageCargoState carriageCargoState = CarriageCargoState.CarriageEmpty;
    public void ChangeCarriageCargoStateTO(CarriageCargoState s)
    {
        if (Logging)
            Debug.Log("carriageCargoState: " + carriageCargoState.ToString() + " -> " + s.ToString());
        carriageCargoState = s;
    }
    public CarriageCargoState GetCarriageCargoState() { return carriageCargoState; }
    public GameObject cargo;
    public void InteractWithPosition(ConnectionPoint f)
    {
        switch (carriageCargoState)
        {
            case CarriageCargoState.CarriageFull:    //Try to Empty the Carriage (Lay Down the Resource)
                if (f.resourceInput && !f.resourceOnField)
                {
                    if (f.PushResource(cargo))
                    {
                        ChangeMesh(basicMesh);
                        ChangeCarriageCargoStateTO(CarriageCargoState.CarriageEmpty);
                        ChangeCarriageStateTO(CarriageState.CarriageIdle);
                    }
                }

                break;
            case CarriageCargoState.CarriageEmpty:
                //Is there a Resource on this Field
                if (f.ResourceWaitingForPickup() && f.resourceOutput)
                {
                    cargo = f.PullResource();
                    ChangeCarriageCargoStateTO(CarriageCargoState.CarriageFull);

                    var t = cargo.GetComponent<Resource>().type;
                    if (t == Resource.ResourceType.Wood)
                        ChangeMesh(woodMesh);
                    else if (t == Resource.ResourceType.Planks)
                        ChangeMesh(plankMesh);
                    else if (t == Resource.ResourceType.Grain || t == Resource.ResourceType.Flour || t == Resource.ResourceType.Bread)
                        ChangeMesh(grainMesh);

                    ChangeCarriageStateTO(CarriageState.CarriageIdle);
                }
                else if (f.resourceOutput)
                    ChangeCarriageStateTO(CarriageState.CarriageWaitForResource);
                else
                    ChangeCarriageStateTO(CarriageState.CarriageIdle);
                break;
        }
    }
    private void ChangeMesh(Mesh m)
    {
        this.GetComponent<MeshFilter>().mesh = m;
    }
}
