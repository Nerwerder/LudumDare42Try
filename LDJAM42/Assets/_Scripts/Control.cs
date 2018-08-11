using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public CameraBehaviour camera = null;

	void Update ()
    {
        //Camera
        var cBR = Input.GetButton("Fire2");
        var cBM = Input.GetButton("Fire3");
        var cAX = Input.GetAxis("Mouse X");
        var cAY = Input.GetAxis("Mouse Y");
        var cAZ = Input.GetAxis("Mouse ScrollWheel");
        var cKR = Input.GetKey(KeyCode.R);
        camera.SetInput(cBR, cBM, cAX, cAY, cAZ, cKR);
	}
}
