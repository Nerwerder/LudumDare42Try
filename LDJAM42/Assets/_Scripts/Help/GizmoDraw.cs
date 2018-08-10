//@ Sebastian

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple Helper-Script for Drawing Gizmos in the Editor (not visible inGame)
public class GizmoDraw : MonoBehaviour {

    public Color gizmoColor = Color.yellow;
    public float gizmoSize = 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);
    }
}
