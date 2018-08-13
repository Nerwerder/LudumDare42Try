using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour
{
    public enum PlaceType { Water, Meadow, Forest, City }

    public List<Material> basicMaterials;
    public List<Material> glowiMaterials;

    [HideInInspector] public PlaceType type;
    [HideInInspector] public int line, column, id;
    [HideInInspector] public bool buildSpaceFree;
    [HideInInspector] public Building building = null;
    [HideInInspector] public Neighborhood neighborhood;
    private List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();

    public void Set(int t, int x, int z, int i)
    {
        buildSpaceFree = true;

        type = (PlaceType)t;
        SetPos(x, z, i);

        SetBasicMaterial();
    }

    public void SetBasicMaterial()
    {
        SetMaterial(basicMaterials[(int)type]);
    }

    public void SetGlowMaterial()
    {
        SetMaterial(glowiMaterials[(int)type]);
    }

    public void SetMaterial(Material m)
    {
        this.GetComponent<Renderer>().material = m;
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

    public Building BuildBuilding(GameObject g)
    {
        buildSpaceFree = false;
        var b = Instantiate(g, (this.transform.position + g.transform.position), this.transform.rotation, this.transform);
        removeScale(b);

        building = b.GetComponent<Building>();
        building.SetPlace(this);
        building.GetComponent<Building>().Init();

        return building;
    }

    public void removeScale(GameObject o)
    {
        var os = o.transform.localScale;
        var ns = this.transform.localScale;
        o.transform.localScale = new Vector3(os.x / ns.x, os.y / ns.y, os.z / ns.z);
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

    public List<ConnectionPoint> GetConnectionPoints() { return connectionPoints; }

    public ConnectionPoint GetRandomFreeConnectionPoint()
    {
        List<ConnectionPoint> p = new List<ConnectionPoint>();
        foreach (var c in GetConnectionPoints())
            if (c.FreeToMoveOn())
                p.Add(c);

        if (p.Count != 0)
            return p[Random.Range(0, p.Count)];
        else
            return null;
    }
}
