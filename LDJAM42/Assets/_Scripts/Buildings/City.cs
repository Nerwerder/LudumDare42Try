using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Building
{
    public GameObject carriagePrefab = null;
    public int startGold = 500;
    public int maxWood = 10;
    public float woodUsage = 1f;
    public int maxFood = 10;
    public float foodUsage = 1f;

    public float carriageSpawnTime = 1f;
    public int maxCarriages;

    public List<Resource.ResourceType> InputTypes;

    private GUIController gui;
    private float workTimer = 0f;
    private Transform carriageParent = null;
    private List<Carriage> carriages;
    private int wood, food, gold;
    private List<ConnectionPoint> inputLocations = new List<ConnectionPoint>(6);

    public override void Init()
    {
        base.Init();

        Carriages c = FindObjectOfType<Carriages>();
        carriageParent = c.gameObject.transform;
        carriages = c.carriages;
        gui = FindObjectOfType<GUIController>();
        UpdateGui();

        foreach (var con in place.GetConnectionPoints())
            if (con.FreeForUse())
            {
                con.UseAsInput(this);
                inputLocations.Add(con);
            }

        gold = startGold;
    }

    public ConnectionPoint getFreeInputLocation()
    {
        foreach (var i in inputLocations)
            if (i.FreeToMoveOn())
                return i;

        return null;
    }

    private void UpdateGui()
    {
        gui.UpdateGold(gold);
        gui.UpdateCityInfo((food * 100) / maxFood, (wood * 100) / maxWood);
    }

    public override void Work(float time)
    {
        base.Work(time);

        if (carriages.Count < maxCarriages)
            workTimer += time;
        if ((workTimer >= carriageSpawnTime))
            SpawnCariage();

        foreach (var i in inputLocations)
            if (i.ResourceWaitingForUse())
            {
                var r = i.PullResource();
                if (r)
                {
                    var t = r.GetComponent<Resource>().type;
                    if (t == InputTypes[0])
                        wood++;
                    else if (t == InputTypes[1])
                        food++;
                    Destroy(r);
                }

                UpdateGui();
            }
    }

    public void SpawnCariage()
    {
        workTimer = 0f;
        var l = FindRandomFreeMOVELocation();
        if (l != null)
        {
            var rot = Quaternion.LookRotation((l.transform.position - this.transform.position), Vector3.up);
            var c = Instantiate(carriagePrefab, l.transform.position, rot, carriageParent);
            var ca = c.GetComponent<Carriage>();
            l.CarriageMovesOnField(ca);
            carriages.Add(ca);
        }
    }

    public float getSpawnTimerProgress()
    {
        return workTimer / carriageSpawnTime;
    }
}
