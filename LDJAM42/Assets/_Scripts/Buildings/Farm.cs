using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : WorkBuilding
{
    public override void Init()
    {
        base.Init();
    }

    public override void Work(float time)
    {
        if ((!outputLocation.Full() || outputLocation.EmptyCarriageWaiting()) && !stopForIOChange)
            workTimer += (time);

        base.Work(time);
    }
}
