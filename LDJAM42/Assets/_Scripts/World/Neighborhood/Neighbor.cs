using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NeighborPosition
{
    public enum Level { TOP, SAME, DOWN };
    public enum Position { TOP_LEFT, TOP_RIGHT, SAME_RIGHT, SAME_LEFT, DOWN_RIGHT, DOWN_LEFT };

    public Level level;
    public Position position;

    public void set(Position p)
    {
        if ((int)p < 2)
            level = Level.TOP;
        else if ((int)p < 4)
            level = Level.SAME;
        else
            level = Level.DOWN;
        position = p;
    }
}

public class Neighbor
{
    public Place place;
    public NeighborPosition position;
    
    public Neighbor(Place pl, NeighborPosition.Position po)
    {
        Set(pl, po);
    }

    public void Set(Place pl, NeighborPosition.Position po)
    {
        place = pl;
        position.set(po);
    }

}