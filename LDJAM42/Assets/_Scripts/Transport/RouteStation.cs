using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteStation
{
    public enum RouteStationType { None, Output, Input };
    public RouteStationType routeStationType = RouteStationType.None;

    public Path pathToNext = null;
    public Place place = null;
    public ConnectionPoint point = null;
    public Pathfinding pathfinding = null;
    public Route route = null;
    public RouteStation before = null, next = null;

    /// <summary>
    /// Creates a RouteStation
    /// </summary>
    /// <param name="p"> a ref to pathfinding </param>
    /// <param name="r"> a ref to the Route this Station is part of</param>
    /// <param name="c"> the current Place</param>
    /// <param name="b"> the RouteStation bevore (Count-1)</param>
    /// <param name="n"> the next RouteStation (will mostly be null)</param>
    public RouteStation(Pathfinding p, Route r, Place c, RouteStation b = null, RouteStation n = null)
    {
        pathfinding = p;
        route = r;
        before = b;
        place = c;
        next = n;
        DetermineRouteStationType();
        DeterminePoint();
    }

    public void SetNext(RouteStation n)
    {
        next = n;
        DeterminePoint();   //Check if Point is still ok
        CalculatePath();
    }

    public void SetBefore(RouteStation b)
    {
        before = b;
    }

    public void UpdatePoint(ConnectionPoint p, WorkBuilding.WorkBuildingInteractionPoint i)
    {
        point = p;
    }

    private void DetermineRouteStationType()
    {
        //Determine the RouteStationType
        if (before == null)
        {
            if (place.GetBuilding())
                routeStationType = RouteStationType.Output;
            else
                routeStationType = RouteStationType.None;
        }
        else
        {
            if (place.GetBuilding())
                switch (before.routeStationType)
                {
                    case RouteStationType.None:
                        routeStationType = RouteStationType.Output;
                        break;
                    case RouteStationType.Input:
                        routeStationType = RouteStationType.Output;
                        break;
                    case RouteStationType.Output:
                        routeStationType = RouteStationType.Input;
                        break;
                }
            else  //Bevore this Station there was a Place without Building <-- that does NOT mean the Carriage is empty
            {
                //Search for a building bevore this one and use its TYPE //TODO
                routeStationType = RouteStationType.None;
            }
        }
    }

    private void DeterminePoint()
    {
        switch (routeStationType)
        {
            case RouteStationType.None:
                point = place.GetRandomFreeMOVELocation();
                break;
            case RouteStationType.Output:
                point = place.GetOutput();
                break;
            case RouteStationType.Input:
                point = place.GetInput();
                break;
        }
    }

    private void CalculatePath()
    {
        pathToNext = new Path(pathfinding, point, next.point, route);
    }
}
