using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    
    public Buildings buildings = null;

    private enum LeftClickState { Selection, Selected };
    LeftClickState lCState = LeftClickState.Selection;

    GUIController guiControll;
    


    void Start()
    {
        guiControll = GetComponent<GUIController>();
    }

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
        if(!guiControll.getCanvasActive())
        {
            lCState = LeftClickState.Selection;
        }
        if (Input.GetMouseButtonDown(0))
        {
            switch (lCState)
            {
                case LeftClickState.Selection:  //Select a Field
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        //Get the Right Place
                        Place p = hitInfo.collider.GetComponent<Place>();
                        if(p && p.type == Place.PlaceType.Meadow && p.canvasSpaceFree)
                        {
                            guiControll.ActivateBuildPanel(p);
                            lCState = LeftClickState.Selected;
                        }
                        
                    }
                    break;
                default:
                    break;
            }
        }
    }

    

    //ChangeButton but Search the Environment for a special Type of Area
   
}
