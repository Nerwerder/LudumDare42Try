using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighborhood
{
    public int count = 0;
    public int[] Adresses = new int[6] { 0, 0, 0, 0, 0, 0 };

    public void registerAdress(int a)
    {
        Adresses[count++] = a;
    }

    public void registerAdresses(int[] a)
    {
        foreach (var ad in a)
            registerAdress(ad);
    }

    public bool Contains(int id)
    {
        for (int k = 0; k < count; ++k)
            if (Adresses[k] == id)
                return true;
        return false;
    }
}
