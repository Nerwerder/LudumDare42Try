using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Control : MonoBehaviour
{
    public CameraBehaviour myCamera = null;
    public GameObject canvasPrefab = null;
    public Buildings buildings = null;

    private enum LeftClickState {Selection, Selected};
    LeftClickState lCState = LeftClickState.Selection;

    private GameObject activeCanvas = null;
    private Place activeCanvasPlace = null;

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

                    if (activeCanvas != null)
                        RemoveCanvas(activeCanvas, activeCanvasPlace);

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        //Get the Right Place
                        Place p = hitInfo.collider.GetComponent<Place>();
                        if(p && p.GetCanvasSpaceFree())
                        {
                            AddCanvas(p);
                        }
                        lCState = LeftClickState.Selected;
                    }
                       
                    
                    break;
                case LeftClickState.Selected:
                    lCState = LeftClickState.Selection;
                    break;
                default:
                    break;
            }
        }
    }

    public void AddCanvas(Place p)
    {
        Vector3 canvasPosition = new Vector3(p.transform.position.x, 1, p.transform.position.z);
        Quaternion canvasRotation = Quaternion.LookRotation(myCamera.transform.forward, myCamera.transform.up);

        //Prefab, Position, Rotation, Parent
        var canvas = Instantiate(canvasPrefab, canvasPosition, canvasRotation, p.transform);

        //Move the Canvas to the Right Position
        //var canvasRect = canvas.GetComponent<RectTransform>();
        //var canvasOffset = new Vector3((canvasRect.rect.width / 2) * canvas.transform.localScale.x, 0, (canvasRect.rect.height / 2) * canvas.transform.localScale.z);
        //canvas.transform.position += canvasOffset;

        var text = canvas.GetComponentInChildren<Text>();
        text.text = p.getPlaceType().ToString();

        var buttons = canvas.GetComponentsInChildren<Button>();
        ChangeButton(buttons[0], Buildings.BuildingType.WoodCutter, p);
        ChangeButton(buttons[1], Buildings.BuildingType.Sawmill, p);
        ChangeButton(buttons[2], Buildings.BuildingType.Farm, p);
        ChangeButton(buttons[3], Buildings.BuildingType.Windmill, p);
        ChangeButton(buttons[4], Buildings.BuildingType.Bakery, p);

        activeCanvas = canvas;
        activeCanvasPlace = p;
    }

    private void ChangeButton(Button b, Buildings.BuildingType t, Place p)
    {
        b.GetComponentInChildren<Text>().text = t.ToString(); ;
        b.onClick.AddListener(() => ActionWrapper(t, p));
    }

    private void RemoveCanvas(GameObject canvas, Place p)
    {
        Destroy(canvas, 0.3f);
        p.SetCanvasSpaceFree(true);
        activeCanvas = null;
        activeCanvasPlace = null;
    }

    public void ActionWrapper(Buildings.BuildingType type, Place p)
    {
        buildings.Build(type, p);
    }

    


}
