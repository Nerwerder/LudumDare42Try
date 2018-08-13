using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Material basicMaterial, glowMaterial;
    public void SetBasicMaterial() { SetMaterial(basicMaterial); }
    public void SetGlowMaterial() { SetMaterial(glowMaterial); }
    public void SetMaterial(Material m) { this.GetComponent<Renderer>().material = m; }

    public Buildings.BuildingType type;
    protected Place place;

    private void Update()
    {
        Work(Time.deltaTime);
    }

    public void SetPlace(Place p) { place = p; }
    public Place GetPlace() { return place; }

    public virtual void Init()
    {

    }

    public virtual void Work(float time)
    {

    }

    public virtual void Done()
    {

    }

    protected ConnectionPoint FindRandomFreeUSELocation()
    {
        var l = GetFreeUSEPoints();
        if (l.Count == 0)
            return null;
        return l[Random.Range(0, l.Count)];
    }

    protected ConnectionPoint FindRandomFreeMOVELocation()
    {
        var l = GetFreeMOVEPoints();
        if (l.Count == 0)
            return null;
        return l[Random.Range(0, l.Count)];
    }

    protected List<ConnectionPoint> GetFreeUSEPoints()
    {
        List<ConnectionPoint> l = new List<ConnectionPoint>();

        foreach (var c in place.GetConnectionPoints())
            if (c.FreeForUse())
                l.Add(c);

        return l;
    }

    protected List<ConnectionPoint> GetFreeMOVEPoints()
    {
        List<ConnectionPoint> l = new List<ConnectionPoint>();

        foreach (var c in place.GetConnectionPoints())
            if (c.FreeToMoveOn())
                l.Add(c);

        return l;
    }
}
