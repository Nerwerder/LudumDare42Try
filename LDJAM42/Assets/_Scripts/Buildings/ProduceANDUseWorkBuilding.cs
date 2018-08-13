using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceANDUseWorkBuilding : WorkBuilding
{
    public override void Init()
    {
        base.Init();

        FindRandomInputLocation();
    }

    public override void Work(float time)
    {
        base.Work(time);

        if (!inputResource && inputLocation.ResourceWaitingForUse())
        {
            var r = inputLocation.PullResource();
            if (r)
                Destroy(r);
            inputResource = true;
        }

        if ((!outputLocation.Full() || outputLocation.EmptyCarriageWaiting()) && inputResource && !stopForIOChange)
            workTimer += time;
    }
}
