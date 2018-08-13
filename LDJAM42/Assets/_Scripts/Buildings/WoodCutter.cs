using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodCutter : WorkBuilding
{
    private float efficiency = 1f;

    public override void Init()
    {
        base.Init();

        efficiency = calculateEfficiency();
    }

    public override void Work(float time)
    {
        if (!outputLocation.Full() || outputLocation.EmptyCarriageWaiting())
            workTimer += (time * efficiency);

        base.Work(time);
    }

    public float calculateEfficiency()
    {
        float f = 0f;
        foreach (var n in place.neighborhood.GetNeighbors())
            if (n.place.type == Place.PlaceType.Forest)
                f += 0.2f;
        return f;
    }
}
