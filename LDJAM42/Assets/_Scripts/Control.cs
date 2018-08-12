using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    
    public Buildings buildings = null;

    private enum LeftClickState { Selection, Selected };
    LeftClickState lCState = LeftClickState.Selection;

    public GameObject canvas = null;

    public Image efficiancy = null;

    public List<Sprite> efficiancies = new List<Sprite>();

    private bool canvasActive = false;
    private Place activeCanvasPlace = null;


    void Start()
    {
        if(canvas == null)
            canvas = GameObject.FindGameObjectWithTag("BuildMenu");
        canvas.SetActive(false);
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
                            ActivateCanvas(p);
                            lCState = LeftClickState.Selected;
                        }
                        
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void ActivateCanvas(Place p)
    {
        canvas.SetActive(true);

        var buttons = canvas.GetComponentsInChildren<Button>();
        ChangeButton(buttons[0], Buildings.BuildingType.WoodCutter, p, Place.PlaceType.Forest);
        ChangeButton(buttons[1], Buildings.BuildingType.Sawmill, p);
        ChangeButton(buttons[2], Buildings.BuildingType.Farm, p);
        ChangeButton(buttons[3], Buildings.BuildingType.Windmill, p);
        ChangeButton(buttons[4], Buildings.BuildingType.Bakery, p);

        canvasActive = true;
        activeCanvasPlace = p;
    }

    //ChangeButton but Search the Environment for a special Type of Area
    private void ChangeButton(Button b, Buildings.BuildingType t, Place p, Place.PlaceType pt)
    {
        int f = 0;
        int e = 20;
        foreach (var n in p.neighborhood.GetNeighbors())
            if (n.place.type == pt)
                f += e;

        string s = " (" + f.ToString() + "%)";
        ChangeButton(b, t, p, s);
    }

    private void ChangeButton(Button b, Buildings.BuildingType t, Place p, string s = "")
    {
        b.onClick.AddListener(() => ActionWrapper(t, p));
    }

    private void RemoveCanvas(Place p)
    {
        canvas.SetActive(false);
        p.canvasSpaceFree = true;
        canvasActive = false;
        activeCanvasPlace = null;
        lCState = LeftClickState.Selection;
    }

    public void ActionWrapper(Buildings.BuildingType type, Place p)
    {
        buildings.Build(type, p);
        RemoveCanvas(p);
    }
}
