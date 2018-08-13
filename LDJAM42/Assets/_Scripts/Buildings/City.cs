using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Building
{
    public GameObject carriagePrefab = null;
    public int startGold = 500;

    public int maxWood = 10;
    public float woodUsage = 1f;
    public int goldPerWood = 50;

    public int maxFood = 10;
    public float foodUsage = 1f;
    public int goldPerFood = 50;

    public int happyness = 0;
    public int nextStage = 100;
    public int people = 1;
    public int happynessReduction = 1;
    public float happynessTimer;
    public float happynessReductionTime = 20.0f;


    public int newCarriagePointLevel = 500;
    public float carriageSpawnTime = 1f;
    public int maxCarriages;

    public List<Resource.ResourceType> InputTypes;

    private float woodUsageTimer, foodUSageTimer;
    private GUIController gui;
    private float workTimer = 0f;
    private Transform carriageParent = null;
    private List<Carriage> carriages;
    private int wood, food, gold, points;
    private List<ConnectionPoint> inputLocations = new List<ConnectionPoint>(6);

    public override void Init()
    {
        base.Init();

        Carriages c = FindObjectOfType<Carriages>();
        carriageParent = c.gameObject.transform;
        carriages = c.carriages;
        gui = FindObjectOfType<GUIController>();

        foreach (var con in place.GetConnectionPoints())
            if (con.FreeForUse())
            {
                con.UseAsInput(this);
                inputLocations.Add(con);
            }

        gold = startGold;
        UpdateGui();
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
        gui.UpdateCityInfo(happyness);
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

        foodUSageTimer += Time.deltaTime;
        woodUsageTimer += Time.deltaTime;
        happynessTimer += Time.deltaTime;

        if (foodUSageTimer >= foodUsage && food >= 1)
        {
            --food;
            foodUSageTimer = 0f;
            gold += goldPerFood;
            points += goldPerFood;
            updateHappyness();
        }

        if (woodUsageTimer >= woodUsage && wood >= 1)
        {
            --wood;
            woodUsageTimer = 0f;
            gold += goldPerWood;
            points += goldPerFood;
            updateHappyness();
        }

        if (happynessTimer >= happynessReductionTime)
        {      
            happyness -= happynessReduction;
            happynessTimer = 0f;
            if (happyness < 0)
                happyness = 0;
            UpdateGui();
        }

        if (points >= newCarriagePointLevel)
        {
            points = 0;
            maxCarriages++;
        }
    }

    private void updateHappyness()
    {
        happyness += 10;
        if(happyness >= nextStage)
        {
            LevelUp();
        }
        UpdateGui();
    }

    public bool GetGold(int v)
    {
        if (gold >= v)
        {
            gold -= v;
            UpdateGui();
            return true;
        }
        return false;
    }

    private void LevelUp()
    {
        people++;
        nextStage = people;
        maxCarriages += people;
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
        return (float)happyness / (float)nextStage;
    }
}
