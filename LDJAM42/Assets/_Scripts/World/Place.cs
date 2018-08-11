using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public enum PlaceType { Water, Meadow, Forest, City}

    PlaceType type;
    private int line, column;
    private bool free;

    public void Set(int t, int x, int z)
    {
        free = true;
        type = (PlaceType)t;
        SetPos(x, z);
    }

    private void SetPos(int x, int z)
    {
        column = x;
        line = z;
    }

    public int GetColumn()
    {
        return column;
    }

    public int GetLine()
    {
        return line;
    }

    public Vector3 GetPos()
    {
        return GetComponent<GameObject>().transform.position;
    }

    public bool TestForBuilding()
    {
        if (type == PlaceType.Meadow && free == true)
            return true;
        return false;
    }

    public GameObject buildBuilding(GameObject g, Transform parent)
    {
        free = false;
        return Instantiate(g, (this.transform.position + g.transform.position), this.transform.rotation, parent);
    }

}
