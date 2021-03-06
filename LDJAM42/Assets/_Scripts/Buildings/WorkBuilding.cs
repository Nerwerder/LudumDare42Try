﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBuilding : Building
{
    public GameObject inputResourcePrefab = null;
    public Resource.ResourceType inputResourceType;
    public GameObject outputResourcePrefab = null;
    public Resource.ResourceType outputResourceType;

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

        if (workTimer >= productionTime)
            Done();
    }

    public override void Done()
    {
        base.Done();
        if (outputLocation.CreateResource(outputResourcePrefab, outputResourceType))
        {
            inputResource = false;
            workTimer = 0f;
        }
    }

    public float getWorkTimerProgress()
    {
        return workTimer / productionTime;
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

    //IOCHANGE
    public bool wBChangeInput(IOMarker m)
    {      
        //Check if there is a InputLocation and the Place is free
        if(inputLocation && m.point && m.point.FreeForUse())
        {
            inputLocation.StopUsing(this);
            m.point.UseAsInput(this);
            inputLocation = m.point;
            return true;
        }
        return false;
    }

    public bool wbChangeOutput(IOMarker m)
    {
        if(outputLocation && m.point && m.point.FreeForUse())
        {
            outputLocation.StopUsing(this);
            m.point.UseAsOPutput(this);
            outputLocation = m.point;
            return true;
        }
        return false;
    }
}