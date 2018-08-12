using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Place> places = new List<Place>();
    [HideInInspector] public int lines, columns, elements;
    [HideInInspector] public float sizeX, sizeZ;

    public void SetSize(int l, int c, float oX, float oZ, int e)
    {
        lines = l;
        columns = c;
        sizeX = columns * oX;
        sizeZ = lines * oZ;
        elements = e;
    }

    public void RegisterPlace(Place p)
    {
        places.Add(p);
    }

    public Place GetPlace(int l, int c)
    {
        return places[l * columns + c];
    }

    public Place GetPlace(int k)
    {
        return places[k];
    }

    public List<Place> GetPlaces()
    {
        return places;
    }

    public Neighborhood DetermineNeighborhood(Place p)
    {
        Neighborhood n = new Neighborhood();

        //Determine all the Neigboors

        //Border-Cases  top, right, down, left, uneven
        bool top = false, right = false, down = false, left = false, uneven = false;
        int step = 0;

        if (p.line == 0)                             //Border: TOP
            top = true;

        if (p.column % columns == (columns - 1))    //Border: RIGHT
            right = true;

        if (p.line == (lines - 1))                  //Border: DOWN
            down = true;

        if (p.column % columns == 0)                //Border: LEFT
            left = true;

        if (p.line % 2 != 0)                        //Case: Uneven
            uneven = true;


        //Add the 6 Neigboors
        if (!top && (uneven || !left))              //TOP LEFT
        {
            step = columns;
            if (!uneven)
                ++step;
            n.RegisterNeighbor(GetPlace(p.id - step), NeighborPosition.Position.TOP_LEFT);
        }

        if (!top && (!uneven || !right))             //TOP RIGHT
        {
            step = (columns - 1);
            if (!uneven)
                ++step;
            n.RegisterNeighbor(GetPlace(p.id - step), NeighborPosition.Position.TOP_RIGHT);
        }

        if (!right)                                 //RIGHT
            n.RegisterNeighbor(GetPlace(p.id + 1), NeighborPosition.Position.SAME_RIGHT);

        if (!down && (!uneven || !right))           //DOWN RIGHT
        {
            step = (columns + 1);
            if (!uneven)
                --step;
            n.RegisterNeighbor(GetPlace(p.id + step), NeighborPosition.Position.DOWN_RIGHT);
        }

        if (!down && (uneven || !left))             //DOWN LEFT
        {
            step = columns;
            if (!uneven)
                --step;
            n.RegisterNeighbor(GetPlace(p.id + step), NeighborPosition.Position.DOWN_LEFT);
        }

        if (!left)                                   //LEFT
            n.RegisterNeighbor(GetPlace(p.id - 1), NeighborPosition.Position.SAME_LEFT);

        return n;
    }
}
