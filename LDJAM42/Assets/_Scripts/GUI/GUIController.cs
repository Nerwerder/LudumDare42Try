using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUIController : MonoBehaviour {


    public Buildings buildings = null;
    public GameObject canvas = null;

    public Image efficiancy = null;

    public List<Sprite> efficiancies = new List<Sprite>();

    private bool canvasActive = false;
    private Place activeCanvasPlace = null;

    // Use this for initialization
    void Start () {
        if (canvas == null)
            canvas = GameObject.FindGameObjectWithTag("BuildMenu");
        canvas.SetActive(false);
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
    }

    public void ActionWrapper(Buildings.BuildingType type, Place p)
    {
        buildings.Build(type, p);
        RemoveCanvas(p);
    }

    public bool getCanvasActive()
    {
        return canvasActive;
    }
}
