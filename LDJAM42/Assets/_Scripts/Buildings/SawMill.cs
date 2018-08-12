using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMill : WorkBuilding
{
    public override void Init()
    {
        base.Init();

        FindRandomInputLocation();
    }

    public override void Work(float time)
    {
        base.Work(time);

        Debug.DrawLine(inputLocation.transform.position, inputLocation.transform.position + Vector3.up, Color.red);

        if(!inputResource && inputLocation.ResourceWaiting())
        {
            inputLocation.PullResource();
            inputResource = true;
        }

        if (!outputLocation.Full() && inputResource)
            workTimer += time;
    }
}
