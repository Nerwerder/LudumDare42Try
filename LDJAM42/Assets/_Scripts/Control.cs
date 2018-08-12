using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    public Buildings buildings = null;
    public Material TEST = null;

    private enum LeftClickState { Selection, Selected };
    private LeftClickState lCState = LeftClickState.Selection;

    private GUIController guiControll;

    //Selection
    private Place selectedPlace = null;


    void Start()
    {
        guiControll = GetComponent<GUIController>();
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

    void Update()
    {
        //Camera
        CameraControl();

        //LeftClick
        if (Input.GetMouseButtonDown(0))
            LeftClick();

        //RightClick
        if (Input.GetMouseButtonDown(1))
            RightClick(); ;
    }


    private void LeftClick()                //This is basically a state Machine
    {
        LeftClickSelection();
    }

    private void LeftClickSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log(hitInfo.collider.tag.ToString());

            if (hitInfo.collider.CompareTag("Place"))
                selectPlace(hitInfo.collider.GetComponent<Place>());

            if (hitInfo.collider.CompareTag("Carriage"))
                selectCarriage(hitInfo.collider.GetComponent<Carriage>());

            if (hitInfo.collider.CompareTag("Building"))
                selectBuilding(hitInfo.collider.GetComponent<Building>());
        }
    }

    private void selectPlace(Place p)
    {
        //Mark the Selected Element
        if (selectedPlace)
            deselectPlace(selectedPlace);

        p.SetGlowMaterial();
        selectedPlace = p;

        guiControll.ActivateCanvas(p);
    }
    private void deselectPlace(Place p)
    {
        p.SetBasicMaterial();
    }

    private void selectCarriage(Carriage c)
    {

    }

    private void selectBuilding(Building b)
    {

    }

    private void RightClick()
    {

    }



    //ChangeButton but Search the Environment for a special Type of Area

}
