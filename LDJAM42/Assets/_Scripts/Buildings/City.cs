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
        var l = FindRandomFreeMOVELocation();
        l.CarriageMovesOnField();
        var c = Instantiate(carriagePrefab, l.transform.position, carriagePrefab.transform.rotation, carriageParent);
        workTimer = 0f;
        carriages.Add(c.GetComponent<Carriage>());
        Debug.Log("Spawn Carriage");
    }
}
