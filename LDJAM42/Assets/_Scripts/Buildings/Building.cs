using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Buildings.BuildingType type;

    public void set (Buildings.BuildingType t)
    {
        type = t;
    }

}
