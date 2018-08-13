using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Material basicMaterial, glowMaterial;

    private GameObject IOMarker;
    private Material inputMarkerMaterial, outputMarkerMaterial, potentialMarkerMaterial;
    private GameObject inputMarker, outputMarker;
    public void SetIOMarker(GameObject go, Material mi, Material mo, Material mp, Place p, GameObject parent)
    {
        IOMarker = go;
        inputMarkerMaterial = mi;
        outputMarkerMaterial = mo;
        potentialMarkerMaterial = mp;

        inputMarker = Instantiate(IOMarker, this.transform.position, IOMarker.transform.rotation, parent.transform);
        inputMarker.GetComponent<Renderer>().material = inputMarkerMaterial;
        inputMarker.SetActive(false);

        outputMarker = Instantiate(IOMarker, this.transform.position, IOMarker.transform.rotation, parent.transform);
        outputMarker.GetComponent<Renderer>().material = outputMarkerMaterial;
        outputMarker.SetActive(false);
    }

    public void SelectBuilding()
    {
        SetGlowMaterial();

        var wB = this.GetComponent<WorkBuilding>();
        if(wB)
        {
            if(wB.inputLocation)
            {
                inputMarker.transform.position = wB.inputLocation.transform.position + Vector3.up * 0.2f;
                inputMarker.SetActive(true);
            }
            if(wB.outputLocation)
            {
                outputMarker.transform.position = wB.outputLocation.transform.position + Vector3.up * 0.2f;
                outputMarker.SetActive(true);
            }
        }
    }

    public void DeselectBuilding()
    {
        SetBasicMaterial();
        inputMarker.SetActive(false);
        outputMarker.SetActive(false);
    }

    private void SetBasicMaterial() { SetMaterial(basicMaterial); }
    private void SetGlowMaterial() { SetMaterial(glowMaterial); }
    private void SetMaterial(Material m) { this.GetComponent<Renderer>().material = m; }

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
