using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    [HideInInspector] public int ID;
    private Place[] places = new Place[3];

    //Registers the Placed AND registers the ConnectionPoint with the Places !
    public void RegisterPlaces(Place p1, Place p2, Place p3)
    {
        places[0] = p1;
        places[1] = p2;
        places[2] = p3;

        for (int k = 0; k < 3; ++k)
            places[k].RegisterConnectionPoint(this);

        ID = CreateID(p1.id, p2.id, p3.id);
    }

    //Does NOT work for Worlds bigger than 255 ELEMENTS
    public int CreateID(int p1ID, int p2ID, int P3ID)
    {
        //Sort the List
        List<int> l = new List<int> { p1ID, p2ID, P3ID };
        l.Sort();

        //Create the ID
        int ID = (l[0]);
        ID = ID | (l[1] << 8);
        ID = ID | (l[2] << 16);

        return ID;
    }

    public bool Compare(Place p1, Place p2, Place p3)
    {
        var cID = CreateID(p1.id, p2.id, p3.id);
        return cID == ID;
    }

    public bool Compare (int oID)
    {
        return oID == ID;
    }
}
