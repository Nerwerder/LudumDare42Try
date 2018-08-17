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
    private LineRendererParent lineRendererParent;
    private bool drawSelection, drawTab;
    private bool drawPath;


    //MATERIAL
    public Material basicMaterial, glowMaterial;
    public void SetBasicMaterial() { SetMaterial(basicMaterial); }
    public void SetGlowMaterial() { SetMaterial(glowMaterial); }
    public void SetMaterial(Material m) { this.GetComponent<Renderer>().material = m; }
    /// <summary>
    /// Set Draw to false or True and give the source
    /// </summary>
    /// <param name="s"> 1 == Selection, 2 == Tab</param>
    public void SetDrawPath(bool t, int s)
    {
        if (s == 1)
            drawSelection = t;
        else if (s == 2)
            drawTab = t;

        drawPath = (drawSelection || drawTab);
        if (drawPath)   //activate
        {
            switch (carriageControlState)
            {
                case CarriageControlState.CarriageManual:
                    if (path != null)
                    {
                        lineRenderer[0].enabled = true;
                        lineRenderer[1].enabled = false;
                        lineRenderer[2].enabled = false;

                    }
                    break;
                case CarriageControlState.CarriageRoute:
                    if(route != null)
                    {
                        lineRenderer[0].enabled = false;
                        lineRenderer[1].enabled = true;
                        lineRenderer[2].enabled = true;
                    }
                    break;
            }
        }

        else            //deactivate
            foreach (var l in lineRenderer)
                l.enabled = false;
    }

    //BASIC
    private void Awake()
    {
        pathfinding = FindObjectOfType<Pathfinding>();

        lineRendererParent = FindObjectOfType<LineRendererParent>();
        lineRenderer = new List<LineRenderer>(3);
        for (int k = 0; k < 3; ++k)
        {
            lineRenderer.Add(new GameObject().AddComponent<LineRenderer>() as LineRenderer);
            lineRenderer[k].widthMultiplier = 0.1f;
            lineRenderer[k].transform.parent = lineRendererParent.transform;
        }
        lineRenderer[0].material = pathMaterial;
        lineRenderer[1].material = collectMaterial;
        lineRenderer[2].material = deliveryMaterial;

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
                        {
                            InteractWithPosition(targetPosition);
                            path = null;
                        }

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
    List<LineRenderer> lineRenderer;
    private void DrawPath()
    {
        switch (carriageControlState)
        {
            case CarriageControlState.CarriageManual:
                if (path != null)
                    path.Draw(lineRenderer[0]);

                break;
            case CarriageControlState.CarriageRoute:
                if (route != null)
                    route.DrawRoute(lineRenderer);
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

        //Change the active LineRenderers
        switch (s)
        {
            case CarriageControlState.CarriageManual:
                SetDrawPath(true, 1);
                break;
            case CarriageControlState.CarriageRoute:
                SetDrawPath(true, 1);
                break;
        }
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
        targetPosition = p;
        path = new Path(pathfinding, actualPosition, p);
        ChangeCarriageStateTO(CarriageState.CarriageOnMyWay);
    }

    //ROUTE = Route Mode
    public GameObject RouteStationMarker;

    private Route route;
    public Route GetRoute() { return route; }
    public void AddToRoute(Place p)
    {
        if (route == null)
        {
            ChangeCarriageStateTO(CarriageState.CarriageOnMyWay);
            route = new Route(pathfinding, this);
            ChangeCarriageControlState(CarriageControlState.CarriageRoute);
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
