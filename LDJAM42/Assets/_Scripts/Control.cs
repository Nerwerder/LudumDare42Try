using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;

    private enum LeftClickState {Selection, Selected};
    LeftClickState lCState = LeftClickState.Selection;
	void Update ()
    {
        //Camera
        CameraControl();

        //LeftClick
        LeftClick();


	}


    private void CameraControl()
    {
        var cBR = Input.GetButton("Fire2");
        var cBM = Input.GetButton("Fire3");
        var cAX = Input.GetAxis("Mouse X");
        var cAY = Input.GetAxis("Mouse Y");
        var cAZ = Input.GetAxis("Mouse ScrollWheel");
        var cKR = Input.GetKey(KeyCode.R);
        myCamera.SetInput(cBR, cBM, cAX, cAY, cAZ, cKR);
    }

    private void LeftClick()                //This is basically a state Machine
    {
        if(Input.GetButton("Fire1"))
        {
            switch (lCState)
            {
                case LeftClickState.Selection:  //Select a Field
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        hitInfo.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    }


                    break;
                case LeftClickState.Selected:
                    break;
                default:
                    break;
            }
        }
    }
}
