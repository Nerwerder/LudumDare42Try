using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBuilding : Building
{
    public GameObject inputResourcePrefab = null;
    public GameObject outputResourcePrefab = null;

    protected ConnectionPoint outputLocation;
    protected ConnectionPoint inputLocation;

    public float productionTime = 1f;

    protected float workTimer;

    public override void Init()
    {
        base.Init();

        if (!FindRandomOutputLocation())
            Debug.Log("Buildings.init -> No Output Found");
    }

    public override void Work(float time)
    {
        base.Work(time);

        if (workTimer >= productionTime && !outputLocation.Full())
            Done();
    }

    public override void Done()
    {
        base.Done();
        outputLocation.PushResource(outputResourcePrefab);
    }

    protected bool FindRandomOutputLocation()
    {
        var c = FindRandomFreeLocation();
        if (c == null)
            return false;

        c.UseAsOPutput(this);
        outputLocation = c;
        return true;
    }

    protected bool FindRandomInputLocation()
    {
        var c = FindRandomFreeLocation();
        if (c == null)
            return false;

        c.UseAsInput(this);
        inputLocation = c;
        return true;
    }

    protected ConnectionPoint FindRandomFreeLocation()
    {
        foreach (var c in place.GetConnectionPoints())
            if (c.FreeForUse())
                return c;

        return null;
    }
}