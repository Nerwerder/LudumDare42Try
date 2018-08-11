using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public enum PlaceType { Water, Meadow, Forest, City }

    private PlaceType type;
    private int line, column;
    private bool buildSpaceFree, canvasSpaceFree;

    public void Set(int t, int x, int z)
    {
        buildSpaceFree = true;
        canvasSpaceFree = true;

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
        if (type == PlaceType.Meadow && buildSpaceFree == true)
            return true;
        return false;
    }

    public GameObject BuildBuilding(GameObject g, Transform parent)
    {
        buildSpaceFree = false;
        return Instantiate(g, (this.transform.position + g.transform.position), this.transform.rotation, parent);
    }

    public bool GetCanvasSpaceFree() { return canvasSpaceFree; }
    public void SetCanvasSpaceFree(bool free) { canvasSpaceFree = free; }

    public PlaceType getPlaceType() { return type; }

}
