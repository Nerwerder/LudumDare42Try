using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public WorldCreation worldCreation; //Needed for the Default Travel-Speed
    public float defaultCost = 0.5f;

    private int cycleID = 0;
    private List<ConnectionPoint> open = new List<ConnectionPoint>(256);
    private List<ConnectionPoint> closed = new List<ConnectionPoint>(256);

    static int SortByCost(ConnectionPoint p1, ConnectionPoint p2)
    {
        return p1.pathCost.CompareTo(p2.pathCost);
    }

    public List<ConnectionPoint> FindPath(ConnectionPoint start, ConnectionPoint end)   //TODO Zero the Cost at Start
    {
        //PREPERATION
        ConnectionPoint current = null;
        open.Clear();
        closed.Clear();
        cycleID++;
        start.resetPathfindingValues();
        open.Add(start);

        //A*
        do
        {
            //Get The Element with the Lowest Cost
            open.Sort(SortByCost);
            current = open[0];
            open.RemoveAt(0);

            //Is this the Target?
            if (current == end)
                return CalculatePath(current);

            closed.Add(current);
            Expand(current);
        } while (open.Count != 0);

        return null;
    }

    private void Expand(ConnectionPoint currend)
    {
        foreach(var c in currend.connections)
        {
            if (closed.Contains(c.target))
                continue;

            var cost = currend.pathCost + CalculateCost(c);

            //Reset the PahtinfindingValues in ConnectionPoint if this is his first appearance in this Cycle
            c.target.resetIfFirstEncounter(cycleID);

            if (open.Contains(c.target) && cost >= c.target.pathCost)
                continue;

            c.target.pathPredecessor = currend;
            c.target.pathCost = cost;

            if (!open.Contains(c.target))
                open.Add(c.target);
        }
    }

    private float CalculateCost(Connection c)
    {
        return (worldCreation.defaultTravelSpeed + defaultCost) - c.speed;
    }

    private List<ConnectionPoint> CalculatePath(ConnectionPoint p)
    {
        var c = p;
        List<ConnectionPoint> path = new List<ConnectionPoint>();

        while(c.pathPredecessor != null)
        {
            path.Add(c);
            c = c.pathPredecessor;
        }

        return path;
    }
}
