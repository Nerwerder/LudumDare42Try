using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    public Buildings buildings = null;
    public Material TEST = null;

    private GUIController guiControll;
    private Carriages carriages;

    //Change BUILDING IO
    Building inputChangeBuilding = null;
    Building outputChangeBuilding = null;
    public void ChangeBuildingInput(Building b)
    {
        inputChangeBuilding = b;
        inputChangeBuilding.stopForIOChange = true;
    }

    public void ChangeBuildingOutput(Building b)
    {
        outputChangeBuilding = b;
        outputChangeBuilding.stopForIOChange = true;
    }

    //Selection
    private enum SelectionState { PlaceSelected, BuildingSelected, CarriageSelected, NothingSelected };
    private SelectionState selectionState = SelectionState.NothingSelected;
    private Place selectedPlace = null;
    private Building selectedBuilding = null;
    private Carriage selectedCarriage = null;

    void Start()
    {
        guiControll = GetComponent<GUIController>();
        carriages = FindObjectOfType<Carriages>();
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

        //Tab -> Draw all the Carriage Paths
        if (Input.GetKeyDown(KeyCode.Tab))
            carriages.DrawAllCarriagePaths();
        if (Input.GetKeyUp(KeyCode.Tab))
            carriages.StopDrawing();
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

        //CHANGE Building-IO
        if(inputChangeBuilding)
        {
            inputChangeBuilding.stopForIOChange = false;
            inputChangeBuilding = null;
        }
        if(outputChangeBuilding)
        {
            outputChangeBuilding.stopForIOChange = false;
            outputChangeBuilding = null;
        }

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

            if (c.CompareTag("Marker"))
                selectMarker(c.GetComponent<IOMarker>());
        }
        else
            selectNothing();

    }
    private void selectMarker(IOMarker m)
    {
        if (inputChangeBuilding)
            inputChangeBuilding.ChangeInput(m);
        if (outputChangeBuilding)
            outputChangeBuilding.ChangeOutput(m);
    }
    private void selectPlace(Place p)
    {
        selectNothing();    //Clean

        if (p.GetBuilding())     //If there is a Building on the Place, select this instead ! (TODO)
            selectBuilding(p.GetBuilding());
        else
        {
            p.SetGlowMaterial();
            selectionState = SelectionState.PlaceSelected;

            selectedPlace = p;
            if (p.TestForBuilding())
                guiControll.ActivateBuildPanel(p);
        }
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
        c.SetDrawPath(true, 1);
        selectedCarriage = c;
 
        guiControll.ActivateRoutePanel(c.GetRoute());
    }
    private void deselectCarriage(Carriage c)
    {
        selectionState = SelectionState.NothingSelected;
        if (c != null)
        {
            c.SetBasicMaterial();
            c.SetDrawPath(false, 1);
        }
        selectedCarriage = null;
    }
    private void selectBuilding(Building b)
    {
        selectNothing();    //Clean

        b.SelectBuilding();
        selectionState = SelectionState.BuildingSelected;
        selectedBuilding = b;
        guiControll.ActivateBuildingInfo(b);
    }
    private void deselectBuilding(Building b)
    {
        selectionState = SelectionState.NothingSelected;
        if (b != null)
            b.DeselectBuilding();
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

        if (selectionState == SelectionState.CarriageSelected && c)
        {
            var p = c.GetComponent<Place>();
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.X)) //Shift is pressed, Add Building or Place to Route
            {
                if (c.CompareTag("Place"))
                        selectedCarriage.AddToRoute(p);
                else if (c.CompareTag("Building"))
                    selectedCarriage.AddToRoute(c.GetComponent<Building>().GetPlace());
            }
            else
            {
                if (c.CompareTag("Place"))
                {
                    if (p.GetBuilding())
                        selectedCarriage.GoTo(p.GetBuilding());
                    else
                        selectedCarriage.GoTo(p);
                }
                else if (c.CompareTag("Building"))
                    selectedCarriage.GoTo(c.GetComponent<Building>());
            }
        }
    }
}
