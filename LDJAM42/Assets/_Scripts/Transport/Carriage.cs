using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : MonoBehaviour
{
    //MATERIAL
    public Material basicMaterial, glowMaterial;
    public void SetBasicMaterial(){SetMaterial(basicMaterial);}
    public void SetGlowMaterial(){SetMaterial(glowMaterial);}
    public void SetMaterial(Material m){this.GetComponent<Renderer>().material = m;}

    //BASIC
    private void Awake()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    //TRAVEL
    private Pathfinding pathfinding;
    private ConnectionPoint actualPosition = null, targetPosition = null;
    private List<ConnectionPoint> path;
    public void SetActualPosition(ConnectionPoint c) { actualPosition = c; }

    public void GoTo (Building b)
    {
        Debug.Log("GoTo Building : " + b.type.ToString());

        //Test if the Building is a WorkBuilding or a City
        var wB = b.GetComponent<WorkBuilding>();
        if (wB)
            GoTo(wB.outputLocation);
    }

    public void GoTo (Place p)
    {
        Debug.Log("GoTo Place : " + p.type.ToString());

    }

    public void GoTo (ConnectionPoint p)
    {
        Debug.Log("Goto Place :" + p.ToString());
        targetPosition = p;
        path = pathfinding.findPath(actualPosition, targetPosition);
    }
      
}
