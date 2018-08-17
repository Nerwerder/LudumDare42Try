using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public ConnectionPoint start, end;

    private int startPosition = 1;
    private int position;
    private Pathfinding pathfinding;
    private List<ConnectionPoint> points;
    private Route route;

    public Path(Pathfinding p)
    {
        Set(p);
    }

    public Path(Pathfinding p, ConnectionPoint s, ConnectionPoint e, Route r = null)
    {
        Set(p);
        CalculatePath(s, e);
        route = r;
    }

    private void Set(Pathfinding p)
    {
        pathfinding = p;
    }

    public void CalculatePath(ConnectionPoint s, ConnectionPoint e)
    {
        start = s;
        end = e;
        Calculate();
    }

    public ConnectionPoint GetTarget()
    {
        return points[position];
    }

    public void Reset()
    {
        position = startPosition;
    }

    public bool Arrive()
    {
        if ((position + 1) >= points.Count)
            return true;

        ++position;
        return false;
    }

    public void ChangePath(ConnectionPoint p, WorkBuilding.WorkBuildingInteractionPoint i)
    {
        switch (i)
        {
            case WorkBuilding.WorkBuildingInteractionPoint.WBInput:
                end = p;
                break;
            case WorkBuilding.WorkBuildingInteractionPoint.WBOutput:
                start = p;
                break;
        }

        if (route != null)
            route.UpdateRoute(this, p, i);

        Calculate(); //TODO this will cause a lot of Problems (if the Carriage is not currently ON the new Path -> Calculate a tmpPath to Drive to a Position on the new Path)
    }

    private void Calculate()
    {
        points = pathfinding.CalculatePath(start, end);
        Reset();
    }

    //DRAW
    public void Draw(Material m)
    {
        if (points == null)
            return;
        for (int k = 0; k < (points.Count - 1); ++k)
            points[k].DrawLineTo(points[k + 1], m);
    }
    public void StopDrawing()
    {
        if (points == null)
            return;

        foreach (var p in points)
            p.StopDrawing();
    }
}
