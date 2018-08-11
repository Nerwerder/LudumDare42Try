using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float startHeight = 10f;

    private World world;

	// Use this for initialization
	void Start ()
    {
        world = FindObjectOfType<WorldCreation>().GetWorld();
        var pos = new Vector3(world.getSizeX() / 2, startHeight, -(world.getSizeY()/2));
        var pos2 = pos;
        pos2.y = 0;

        this.transform.position = pos;
        this.transform.LookAt(pos2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
