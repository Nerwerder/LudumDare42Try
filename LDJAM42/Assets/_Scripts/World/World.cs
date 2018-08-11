using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Place> Places = new List<Place>();
    private int lines, columns;
    private float sizeX, sizeZ;

    public void SetSize (int l, int c, float oX, float oZ)
    {
        lines = l;
        columns = c;
        sizeX = columns * oX;
        sizeZ = lines * oZ;
    }

    public void RegisterPlace(Place p )
    {
        Places.Add(p);
    }

    public Place GetPlace(int l, int c)
    {
        return Places[l * columns + c];
    }

    public int GetLines() { return lines; }
    public int getColumns() { return columns; }
    public float getSizeX() { return sizeX; }
    public float getSizeY() { return sizeZ; }
}
