using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    private int posX, posZ, type;

    public void set(int t, int x, int z)
    {
        type = t;
        setPos(x, z);
    }

    private void setPos(int x, int z)
    {
        posX = x;
        posZ = z;
    }
}
