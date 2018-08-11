using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public enum PlaceType { Water, Meadow, Forest, City }

    [HideInInspector] public PlaceType type;
    [HideInInspector] public int line, column, id;
    [HideInInspector] public bool buildSpaceFree, canvasSpaceFree;
    [HideInInspector] public Neighborhood neighborhood;
    private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    public void Set(int t, int x, int z, int i)
    {
        buildSpaceFree = true;
        canvasSpaceFree = true;

        type = (PlaceType)t;
        SetPos(x, z, i);
    }

    private void SetPos(int x, int z, int i)
    {
        column = x;
        line = z;
        id = i;
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

    public void RegisterConnectionPoint(ConnectionPoint p)
    {
        connectionPoints.Add(p);
    }

    public bool ContainsConnectionPoint(Place o1, Place o2)
    {
        if (connectionPoints.Count == 0)
            return false;

        //Calculate the other ID
        var oID = connectionPoints[0].CreateID(this.id, o1.id, o2.id);

        foreach (var c in connectionPoints)
            if (c.Compare(oID))
                return true;

        return false;
    }
}
