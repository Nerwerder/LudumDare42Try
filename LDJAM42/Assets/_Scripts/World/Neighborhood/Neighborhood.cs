using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Neighborhood
{
    List<Neighbor> neighbors = new List<Neighbor>(6);

    public void RegisterNeighbor(Place a, NeighborPosition.Position p)
    {
        var nN = new Neighbor(a, p);
        neighbors.Add(nN);
    }

    public bool Contains(Place p)
    {
        return Contains(p.id);
    }

    public bool Contains(int id)
    {
        foreach(var n in neighbors)
            if (n.place.id == id)
                return true;
        return false;
    }

    public NeighborPosition getPosition(int id)
    {
        foreach (var n in neighbors)
            if (n.place.id == id)
                return n.position;

        Debug.Log("Neighborhood.getPosition <-- THIS PLACE IS EVIL !");
        return new NeighborPosition();
    }

    public List<Neighbor> GetNeighbors() { return neighbors; }
}
