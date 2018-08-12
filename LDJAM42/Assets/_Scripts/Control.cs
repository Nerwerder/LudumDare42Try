﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    public Buildings buildings = null;
    public Material TEST = null;

    private GUIController guiControll;

    //Selection
    private enum SelectionState { PlaceSelected, BuildingSelected, CarriageSelected, NothingSelected };
    private SelectionState selectionState = SelectionState.NothingSelected;
    private Place selectedPlace = null;
    private Building selectedBuilding = null;
    private Carriage selectedCarriage = null;

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

    private Collider MouseRayCast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo);
        return hitInfo.collider;
    }

    private void LeftClick()                //This is basically a state Machine
    {
        LeftClickSelection();
    }

    private void LeftClickSelection()
    {
        var c = MouseRayCast();
        if (c)
        {
            if (c.CompareTag("Place"))
                selectPlace(c.GetComponent<Place>());

            if (c.CompareTag("Carriage"))
                selectCarriage(c.GetComponent<Carriage>());

            if (c.CompareTag("Building"))
                selectBuilding(c.GetComponent<Building>());
        }
        else
            selectNothing();

    }
    private void selectPlace(Place p)
    {
        selectNothing();    //Clean

        selectionState = SelectionState.PlaceSelected;
        p.SetGlowMaterial();
        selectedPlace = p;
        if (p.TestForBuilding())
            guiControll.ActivateBuildPanel(p);
    }
    private void deselectPlace(Place p)
    {
        selectionState = SelectionState.NothingSelected;
        if (p != null)
            p.SetBasicMaterial();
        p = null;
    }
    private void selectCarriage(Carriage c)
    {
        selectNothing();    //Clean

        selectionState = SelectionState.CarriageSelected;
        c.SetGlowMaterial();
        selectedCarriage = c;
    }
    private void deselectCarriage(Carriage c)
    {
        selectionState = SelectionState.NothingSelected;
        if (c != null)
            c.SetBasicMaterial();
        selectedCarriage = null;
    }
    private void selectBuilding(Building b)
    {
        selectNothing();    //Clean

        selectionState = SelectionState.BuildingSelected;
        guiControll.ActivateBuildingInfo();
    }
    private void deselectBuilding(Building b)
    {
        selectionState = SelectionState.NothingSelected;
        selectedBuilding = null;
    }
    private void selectNothing()
    {
        selectionState = SelectionState.NothingSelected;

        guiControll.RemoveCanvas();
        deselectPlace(selectedPlace);
        deselectCarriage(selectedCarriage);
        deselectBuilding(selectedBuilding);
    }


    private void RightClick()
    {
        var c = MouseRayCast();
        
        if(selectionState == SelectionState.CarriageSelected && c)
        {
            if(c.CompareTag("Place"))
            {
                var p = c.GetComponent<Place>();
                if (p.building)
                    selectedCarriage.GoTo(p.building);
                else
                    selectedCarriage.GoTo(p);
            }

            else if(c.CompareTag("Building"))
            {
                selectedCarriage.GoTo(c.GetComponent<Building>());
            }

        }
    }
}
