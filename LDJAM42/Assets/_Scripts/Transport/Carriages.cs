using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriages : MonoBehaviour
{
    public List<Carriage> carriages = new List<Carriage>();

    public void DrawAllCarriagePaths()
    {
        foreach (var c in carriages)
            c.SetDrawPath(true, 2);
    }

    public void StopDrawing()
    {
        foreach (var c in carriages)
            c.SetDrawPath(false, 2);
    }
}
