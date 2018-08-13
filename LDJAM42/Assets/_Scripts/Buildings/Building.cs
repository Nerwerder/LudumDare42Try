using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    //IOCHANGE
    private GameObject IOMarker;
    private Material inputMarkerMaterial, outputMarkerMaterial, potentialMarkerMaterial;
    protected GameObject inputMarker, outputMarker;
    [HideInInspector] public bool stopForIOChange;
    public void SetIOMarker(GameObject go, Material mi, Material mo, Material mp, GameObject parent)
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

        foreach (var p in place.GetConnectionPoints())
            p.CreatePotentialMarker(IOMarker, potentialMarkerMaterial, parent);
    }
    public void SelectBuilding()
    {
        SetGlowMaterial();

        var wB = this.GetComponent<WorkBuilding>();
        if (wB)
        {
            if (wB.inputLocation)
            {
                inputMarker.transform.position = wB.inputLocation.transform.position + Vector3.up * 0.2f;
                inputMarker.SetActive(true);
            }
            if (wB.outputLocation)
            {
                outputMarker.transform.position = wB.outputLocation.transform.position + Vector3.up * 0.2f;
                outputMarker.SetActive(true);
            }
            ActivateAllPotentialMarkers();
        }
    }
    public void ActivateAllPotentialMarkers()
    {
        foreach (var p in place.GetConnectionPoints())
            if (p.FreeForUse())
                p.potentialMarker.SetActive(true);
    }
    public void DeactivateAllPotentialMarkers()
    {
        foreach (var p in place.GetConnectionPoints())
            p.potentialMarker.SetActive(false);
    }
    public void DeselectBuilding()
    {
        SetBasicMaterial();
        inputMarker.SetActive(false);
        outputMarker.SetActive(false);
        DeactivateAllPotentialMarkers();
    }
    public void ChangeInput(IOMarker m)
    {
        var wB = this.GetComponent<WorkBuilding>();
        if (wB && wB.wBChangeInput(m))
        {
            DeactivateAllPotentialMarkers();
            SelectBuilding();
        }
    }
    public void ChangeOutput(IOMarker m)
    {
        var wB = this.GetComponent<WorkBuilding>();
        if (wB && wB.wbChangeOutput(m))
        {
            DeactivateAllPotentialMarkers();
            SelectBuilding();
        }
    }

    //MATERIAL BASIC AND GLOW
    public Material basicMaterial, glowMaterial;
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
