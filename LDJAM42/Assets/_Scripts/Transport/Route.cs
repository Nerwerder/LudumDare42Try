using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route
{
    //extern
    private Pathfinding pathfinding;
    private Carriage carriage;

    //local
    private RouteStation curent;
    private List<RouteStation> stations = new List<RouteStation>();
    private Path tmpPath;

    public Route(Pathfinding p, Carriage c)
    {
        pathfinding = p;
        carriage = c;
    }

    ///AddPlaces (PlaceToAdd, actualPosition)
    public void AddPlace(Place p)
    {
        if (stations.Count == 0) //Create the first Station
        {
            var station = new RouteStation(pathfinding, this, p);
            stations.Add(station);
            curent = station;
            tmpPath = new Path(pathfinding, carriage.GetActualPosition(), stations[0].point, this);
        }
        else                                                                    //Create a following Station
        {
            var before = stations[(stations.Count - 1)];                        //Get the Station bevore this one
            var station = new RouteStation(pathfinding, this, p, before);       //Create the Station
            stations.Add(station);                                              //Add the Station
            before.SetNext(station);                                            //Set the Next for the last Station

            //A Route is round
            station.SetNext(stations[0]);
        }
    }

    public void UpdateRoute(Path p, ConnectionPoint c, WorkBuilding.WorkBuildingInteractionPoint i)
    {
        foreach (var s in stations)
            if (s.pathToNext == p)
                s.UpdatePoint(c, i);
    }

    public void DoneWithStation()
    {
        if (tmpPath != null)
            tmpPath = null;

        else if (curent.next != null)
        {
            curent.pathToNext.Reset();
            curent = curent.next;
        }

    }

    public Path GetCurrentPath()
    {
        if (tmpPath != null)
            return tmpPath;
        else
            return curent.pathToNext;
    }

    public ConnectionPoint GetCurrentTargetPosition()
    {
        if (tmpPath != null)
            return tmpPath.end;
        else
            return curent.next.point;
    }

    //WORK IN PROGRESS
    public void DrawRoute(LineRenderer l, Material p, Material d, Material c)
    {
        int c0 = 0;
        if (tmpPath != null)
            c0 = tmpPath.Draw(l, p);

        if (stations.Count <= 1)
            return;

        //Calculate the PointCount
        int c1 = 0;
        foreach (var s in stations)
            c1 += s.pathToNext.GetPathSize();
        l.positionCount = (c0 + c1);

        l.material = p;

        int c2 = c0;
        foreach (var s in stations)
            foreach (var n in s.pathToNext.GetPath())
            {
                l.SetPosition(c2, n.transform.position);
                c2++;
            }
    }
}
