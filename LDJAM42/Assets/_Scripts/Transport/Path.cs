using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<ConnectionPoint> nodes = null;
    public int position = 0;

    public void SetPath(List<ConnectionPoint> p)
    {
        nodes = p;
        position = p.Count - 1;
    }

    public ConnectionPoint GetNetxtTarget()
    {
        return nodes[position];
    }

    public bool Arrive()
    {
        if (position == 0)
            return true;
        else
            position--;
        return false;
    }

    public void Clear()
    {
        nodes = null;
        position = 0;
    }

    public bool Empty()
    {
        return (nodes == null);
    }
}
