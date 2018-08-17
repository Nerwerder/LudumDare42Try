using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBuilding : Building
{
    public GameObject inputResourcePrefab = null;
    public Resource.ResourceType inputResourceType;
    public GameObject outputResourcePrefab = null;
    public Resource.ResourceType outputResourceType;
    public float productionTime = 1f;

    [HideInInspector] public ConnectionPoint outputLocation;
    [HideInInspector] public ConnectionPoint inputLocation;

    protected float workTimer;
    protected bool inputResource;

    //OVERIDE BLOCK
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

    //FUNCTIONS
    public float GetWorkTimerProgress()
    {
        return workTimer / productionTime;
    }
    protected bool FindRandomOutputLocation()
    {
        var c = place.GetRandomFreeUSELocation();
        if (c == null)
            return false;

        c.UseAsOPutput(this);
        outputLocation = c;
        return true;
    }
    protected bool FindRandomInputLocation()
    {
        var c = place.GetRandomFreeUSELocation();
        if (c == null)
            return false;

        c.UseAsInput(this);
        inputLocation = c;
        return true;
    }

   //IOCHANGE
    public bool WBChangeInput(IOMarker m)
    {
        //Check if there is a InputLocation and the Place is free
        if (inputLocation && m.point && m.point.FreeForUse())
        {
            inputLocation.StopUsing(this);
            m.point.UseAsInput(this);
            inputLocation = m.point;
            ReCalculatePaths(inputLocation, WorkBuildingInteractionPoint.WBInput);
            return true;
        }
        return false;
    }
    public bool WbChangeOutput(IOMarker m)
    {
        if (outputLocation && m.point && m.point.FreeForUse())
        {
            outputLocation.StopUsing(this);
            m.point.UseAsOPutput(this);
            outputLocation = m.point;
            ReCalculatePaths(outputLocation, WorkBuildingInteractionPoint.WBOutput);
            return true;
        }
        return false;
    }

    //REGISTER PATHS (so input and output-Changes can change the Path)
    public enum WorkBuildingInteractionPoint { WBInput, WBOutput };
    private List<Path> inputPaths = new List<Path>();
    private List<Path> outputPaths = new List<Path>();
    public void RegisterPath(Path p, WorkBuildingInteractionPoint i)
    {
        switch (i)
        {
            case WorkBuildingInteractionPoint.WBInput:
                inputPaths.Add(p);
                break;
            case WorkBuildingInteractionPoint.WBOutput:
                outputPaths.Add(p);
                break;
        }
    }
    public void DeregisterPath(Path p, WorkBuildingInteractionPoint i)
    {
        switch (i)
        {
            case WorkBuildingInteractionPoint.WBInput:
                inputPaths.Remove(p);
                break;
            case WorkBuildingInteractionPoint.WBOutput:
                outputPaths.Remove(p);
                break;
        }
    }
    public void ReCalculatePaths(ConnectionPoint p, WorkBuildingInteractionPoint i)
    {
        switch (i)
        {
            case WorkBuildingInteractionPoint.WBInput:
                foreach (var ip in inputPaths)
                    ip.ChangePath(p, i);
                break;
            case WorkBuildingInteractionPoint.WBOutput:
                foreach (var op in outputPaths)
                    op.ChangePath(p, i);
                break;
        }
    }
}