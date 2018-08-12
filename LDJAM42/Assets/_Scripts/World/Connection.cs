using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public ConnectionPoint target;
    public Place place1, place2;
    public float speed;

    public Connection(ConnectionPoint t, Place p1, Place p2)
    {
        Set(t, p1, p2);
    }

    public void Set(ConnectionPoint t, Place p1, Place p2)
    {
        target = t;
        place1 = p1;
        place2 = p2;
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }
}
