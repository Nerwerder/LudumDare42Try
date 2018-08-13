using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route
{
    private int position = 0;
    private List<Place> places = new List<Place>();

    public void AddPlace(Place p)
    {
        places.Add(p);
    }

    public Place GetNextPlace()
    {
        if (++position >= places.Count)
            position = 0;
        return places[position];
    }

    public void Clear()
    {
        places.Clear();
        position = 0;
    }

    public int GetSize()
    {
        return places.Count;
    }

    private List<Place> GetPlaces() { return places; }
}
