using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUIController : MonoBehaviour
{


    public Buildings buildings = null;
    public GameObject buildPanel = null;
    public GameObject buildingInfoPanel = null;

    public Image efficiancy = null;
    public Image infoPanelLabel = null;
    public Image progressbar = null;

    public List<Sprite> efficiancies = new List<Sprite>();
    public List<Sprite> buildingTypes = new List<Sprite>();

    public Text goldAmount = null;
    public Text foodInfo = null;
    public Text woodInfo = null;

    private bool buildPanelActive = false;
    private bool buildingInfoPanelActive = false;

    private Place activeCanvasPlace = null;
    private Building activeBuilding = null;

    // Use this for initialization
    void Start()
    {
        if (buildPanel == null)
            buildPanel = GameObject.FindGameObjectWithTag("BuildMenu");
        
        buildPanel.SetActive(false);
        buildingInfoPanel.SetActive(false);
    }


    void Update()
    {
        if(buildingInfoPanelActive)
        {
            if(activeBuilding.type != Buildings.BuildingType.City)
            {
                progressbar.fillAmount = activeBuilding.GetComponent<WorkBuilding>().getWorkTimerProgress();
            }
            else
            {
                progressbar.fillAmount = activeBuilding.GetComponent<City>().getSpawnTimerProgress();
            }
            
        }
    }

    public void UpdateCityInfo(int food, int wood)
    {
        string foodText = food.ToString() + "%";
        string woodText = wood.ToString() + "%";

        woodInfo.text = woodText;
        foodInfo.text = foodText;
    }

    public void UpdateGold(int amount)
    {
        string goldText = amount.ToString();
        goldAmount.text = goldText;
    }

    public void ActivateBuildPanel(Place p)
    {
        buildPanel.SetActive(true);

        var buttons = buildPanel.GetComponentsInChildren<Button>();

        //WORKAROUND
        foreach (var b in buttons)
            b.onClick.RemoveAllListeners();

        ChangeButton(buttons[0], Buildings.BuildingType.WoodCutter, p, Place.PlaceType.Forest);
        ChangeButton(buttons[1], Buildings.BuildingType.Sawmill, p);
        ChangeButton(buttons[2], Buildings.BuildingType.Farm, p);
        ChangeButton(buttons[3], Buildings.BuildingType.Windmill, p);
        ChangeButton(buttons[4], Buildings.BuildingType.Bakery, p);

        buildPanelActive = true;
        activeCanvasPlace = p;
    }
    public void ActivateBuildingInfo(Building building)
    {
        buildingInfoPanel.SetActive(true);
        buildingInfoPanelActive = true;
        activeBuilding = building;

        switch (building.type)
        {
            case Buildings.BuildingType.City:
                infoPanelLabel.sprite = buildingTypes[0];
                break;
            case Buildings.BuildingType.WoodCutter:
                infoPanelLabel.sprite = buildingTypes[1];
                break;
            case Buildings.BuildingType.Sawmill:
                infoPanelLabel.sprite = buildingTypes[2];
                break;
            case Buildings.BuildingType.Farm:
                infoPanelLabel.sprite = buildingTypes[3];
                break;
            case Buildings.BuildingType.Windmill:
                infoPanelLabel.sprite = buildingTypes[4];
                break;
            case Buildings.BuildingType.Bakery:
                infoPanelLabel.sprite = buildingTypes[5];
                break;
            default:
                break;
        }
        
    }

    private void ChangeButton(Button b, Buildings.BuildingType t, Place p, Place.PlaceType pt)
    {
        int f = 0;
        int e = 20;
        foreach (var n in p.neighborhood.GetNeighbors())
            if (n.place.type == pt)
                f += e;
        switch (f)
        {
            case 0:
                efficiancy.sprite = efficiancies[0];
                break;
            case 20:
                efficiancy.sprite = efficiancies[1];
                break;
            case 40:
                efficiancy.sprite = efficiancies[2];
                break;
            case 60:
                efficiancy.sprite = efficiancies[3];
                break;
            case 80:
                efficiancy.sprite = efficiancies[4];
                break;
            case 100:
                efficiancy.sprite = efficiancies[5];
                break;
            case 120:
                efficiancy.sprite = efficiancies[6];
                break;
            default:
                break;

        }

        ChangeButton(b, t, p);
    }

    private void ChangeButton(Button b, Buildings.BuildingType t, Place p)
    {
        b.onClick.AddListener(() => ActionWrapper(t, p));
    }

    public void RemoveCanvas()
    {
        buildPanel.SetActive(false);
        buildingInfoPanel.SetActive(false);
        buildPanelActive = false;
        buildingInfoPanelActive = false;
        activeCanvasPlace = null;
    }

    public void ActionWrapper(Buildings.BuildingType type, Place p)
    {
        buildings.Build(type, p);
        RemoveCanvas();
    }

    public bool getCanvasActive()
    {
        return buildPanelActive;
    }
}
