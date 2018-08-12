using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    
    public Buildings buildings = null;

    private enum LeftClickState {Selection, Selected};
    LeftClickState lCState = LeftClickState.Selection;

    public GameObject canvas = null;
    private bool canvasActive = false;
    private Place activeCanvasPlace = null;

    void Start()
    {
        //canvas = GameObject.FindGameObjectWithTag("BuildMenu");
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
        if(Input.GetMouseButtonDown(0))
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
                        if(p && p.getPlaceType() == Place.PlaceType.Meadow && p.GetCanvasSpaceFree())
                        {
                            ActivateCanvas(p);
                            lCState = LeftClickState.Selected;
                        }
                        
                    }
                       
                    
                    break;
                //case LeftClickState.Selected:

                //    if (canvasActive == true)
                //        RemoveCanvas(activeCanvasPlace);

                //    lCState = LeftClickState.Selection;
                //    break;
                default:
                    break;
            }
        }
    }

    public void ActivateCanvas(Place p)
    {


        //Move the Canvas to the Right Position
        //var canvasRect = canvas.GetComponent<RectTransform>();
        //var canvasOffset = new Vector3((canvasRect.rect.width / 2) * canvas.transform.localScale.x, 0, (canvasRect.rect.height / 2) * canvas.transform.localScale.z);
        //canvas.transform.position += canvasOffset;

        //var text = canvas.GetComponentInChildren<Text>();
        //text.text = p.getPlaceType().ToString();

        canvas.SetActive(true);

        var buttons = canvas.GetComponentsInChildren<Button>();
        ChangeButton(buttons[0], Buildings.BuildingType.WoodCutter, p);
        ChangeButton(buttons[1], Buildings.BuildingType.Sawmill, p);
        ChangeButton(buttons[2], Buildings.BuildingType.Farm, p);
        ChangeButton(buttons[3], Buildings.BuildingType.Windmill, p);
        ChangeButton(buttons[4], Buildings.BuildingType.Bakery, p);

        canvasActive = true;
        activeCanvasPlace = p;
    }

    private void ChangeButton(Button b, Buildings.BuildingType t, Place p)
    {
        //b.GetComponentInChildren<Text>().text = t.ToString(); ;
        b.onClick.AddListener(() => ActionWrapper(t, p));
    }

    private void RemoveCanvas(Place p)
    {
        canvas.SetActive(false);
        p.SetCanvasSpaceFree(true);
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
