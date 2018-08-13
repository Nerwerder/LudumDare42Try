﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBuilding : Building
{
    public GameObject inputResourcePrefab = null;
    public GameObject outputResourcePrefab = null;

    [HideInInspector] public ConnectionPoint outputLocation;
    [HideInInspector] public ConnectionPoint inputLocation;

    public float productionTime = 1f;

    protected float workTimer;
    protected bool inputResource;

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
        if (outputLocation.PushResource(outputResourcePrefab))
        {
            inputResource = false;
            workTimer = 0f;
        }
           
    }

    protected bool FindRandomOutputLocation()
    {
        var c = FindRandomFreeUSELocation();
        if (c == null)
            return false;

        c.UseAsOPutput(this);
        outputLocation = c;
        return true;
    }

    protected bool FindRandomInputLocation()
    {
        var c = FindRandomFreeUSELocation();
        if (c == null)
            return false;

        c.UseAsInput(this);
        inputLocation = c;
        return true;
    }
}