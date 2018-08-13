using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Building
{
    public GameObject carriagePrefab = null;
    public float carriageSpawnTime = 1f;

    public int maxCarriages;

    private float workTimer = 0f;
    private Transform carriageParent = null;
    private List<Carriage> carriages;

    public override void Init()
    {
        base.Init();

        Carriages c = FindObjectOfType<Carriages>();
        carriageParent = c.gameObject.transform;
        carriages = c.carriages;
    }

    public override void Work(float time)
    {
        base.Work(time);

        workTimer += time;
        if ((workTimer >= carriageSpawnTime) && (carriages.Count < maxCarriages))
            SpawnCariage();        
    }

    public void SpawnCariage()
    {
        workTimer = 0f;
        var l = FindRandomFreeMOVELocation();
        if(l != null)
        {
            var c = Instantiate(carriagePrefab, l.transform.position, carriagePrefab.transform.rotation, carriageParent);
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
