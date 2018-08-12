using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMill : WorkBuilding
{
    private bool input;

    public override void Init()
    {
        base.Init();

        FindRandomInputLocation();
    }

    public override void Work(float time)
    {
        base.Work(time);

        Debug.DrawLine(inputLocation.transform.position, inputLocation.transform.position + Vector3.up, Color.red);

        if(!input && inputLocation.ResourceWaiting())
        {
            inputLocation.PullResource();
            input = true;
        }

        if (!outputLocation.Full() && input)
            workTimer += time;
    }

    public override void Done()
    {
        base.Done();
        input = false;
    }

}
