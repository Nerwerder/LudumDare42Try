using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Place> Places = new List<Place>();

    public void registerPlace(Place p )
    {
        Places.Add(p);
    }

}
