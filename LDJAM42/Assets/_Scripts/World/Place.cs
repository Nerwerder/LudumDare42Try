using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    private int line, column, type;

    public void Set(int t, int x, int z)
    {
        type = t;
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

}
