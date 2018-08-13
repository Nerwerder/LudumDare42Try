using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [HideInInspector] public int ID;
    private List<Place> places = new List<Place>(3);
    public List<Connection> connections = new List<Connection>();

    //PATHFINDING
    public float pathCost = 0f;
    public ConnectionPoint pathPredecessor = null;
    private int pathID = -1;
    public void resetPathfindingValues()
    {
        pathCost = 0f;
        pathPredecessor = null;
    }
    public void resetIfFirstEncounter(int id)
    {
        if(pathID != id)
        {
            pathID = id;
            resetPathfindingValues();
        }
    }

    //Resources and Travel
    public bool resourceOutput, resourceInput, full, carriageOnField, resourceOnField;
    private Building    building = null;
    private GameObject  resource = null;    

    public void changeFullState()
    {
        full = (carriageOnField || resourceOnField);
    }
    public bool Full()
    {
        return full;
    }
    public bool FreeForUse()
    {
        return ((!(resourceInput || resourceOutput)) && building == null);
    }
    public void UseAsOPutput(Building source)
    {
        resourceOutput = true;
        building = source;
    }
    public void UseAsInput(Building source)
    {
        resourceInput = true;
        building = source;
    }
    public void StopUsing(Building source)
    {
        resourceInput = false;
        resourceOutput = false;
        building = null;
    }
    public bool PushResource(GameObject prefab, Resource.ResourceType t)
    {
        if (full)
            return false;

        resource = Instantiate(prefab, this.transform);
        resource.GetComponent<Resource>().type = t;

        //Remove the Parent Scale
        this.transform.parent.GetComponent<Place>().removeScale(resource);

        resourceOnField = true;
        changeFullState();
        return true;
    }
    public GameObject PullResource()
    {
        resource.SetActive(false);
        resourceOnField = false;
        changeFullState();
        return resource;
    }
    public bool ResourceWaitingForPickup()
    {
        if (resourceOnField && resource != null && resourceOutput)
            return true;
        return false;
    }
    public bool ResourceWaitingForUse()
    {
        if (resourceOnField && resource != null && resourceInput)
            return true;
        return false;
    }

    public bool FreeToMoveOn()
    {
        return !full;
    }
    public void CarriageMovesOnField(Carriage c)
    {
        c.SetActualPosition(this);
        carriageOnField = true;
        changeFullState();
        
    }
    public void CarriageMovesFromField(Carriage c)
    {
        c.SetActualPosition(null);
        carriageOnField = false;
        changeFullState();
    }

    private void Update()
    {
        //foreach (var c in connections)
        //    Debug.DrawLine(this.transform.position, c.target.transform.position);
    }

    static int SortByID(Place p1, Place p2)
    {
        return p1.id.CompareTo(p2.id);
    }

    //Registers the Placed AND registers the ConnectionPoint with the Places !
    public void RegisterPlaces(Place p1, Place p2, Place p3)
    {
        places.Add(p1);
        places.Add(p2);
        places.Add(p3);

        foreach (var p in places)
            p.RegisterConnectionPoint(this);

        places.Sort(SortByID);

        ID = CreateID(places[0].id, places[1].id, places[2].id);
    }

    //Does NOT work for Worlds bigger than 255 ELEMENTS
    public int CreateID(int p1ID, int p2ID, int p3ID)
    {
        //Sort the List
        List<int> l = new List<int> { p1ID, p2ID, p3ID };
        l.Sort();

        //Create the ID
        int ID = (l[0]);
        ID = ID | (l[1] << 8);
        ID = ID | (l[2] << 16);

        return ID;
    }

    public bool Compare(int oID)
    {
        return oID == ID;
    }

    public int GetSharedPlaces(ConnectionPoint other, List<Place> output)
    {
        output.Clear();

        foreach (var mp in places)
            foreach (var op in other.places)
                if (mp == op)
                    output.Add(mp);

        return output.Count;
    }

    public bool ConnectedTo(ConnectionPoint other)
    {
        foreach (var c in connections)
            if (c.target == other)
                return true;
        return false;
    }
    private List<Place> sharedPlaces = new List<Place>(2);
    public Connection TestForConnection(ConnectionPoint other)
    {
        //Do not Connect to yourself
        if (this == other)
            return null;

        if(this.GetSharedPlaces(other, sharedPlaces) >= 2)
        {

            //Test if this Connection already exists
            if (this.ConnectedTo(other))
                return null;
            
            var c = new Connection(other, sharedPlaces[0], sharedPlaces[1]);
            connections.Add(c);
            return c;
        }
        
        return null;
    }
}
