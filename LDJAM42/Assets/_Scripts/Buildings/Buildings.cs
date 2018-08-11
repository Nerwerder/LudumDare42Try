using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public enum BuildingType { City, WoodCutter, Sawmill, Farm, Windmill, Bakery}
    public List<GameObject> buildingPrefabs;

    private List<Building> buildings = new List<Building>();

    //Things the System builds
    public bool DefaultBuild(int t, Place p)
    {
        switch (t)
        {
            case 3:     //Trees

                return true;
            case 4:     //City
                var gO = p.buildBuilding(buildingPrefabs[0], this.transform);
                buildings.Add(AddBuildingForType(BuildingType.City, gO));
                return true;
            default: return false;
        }
    }

    //Things the Player can Build
    public bool Build(BuildingType t, Place p)
    {
        //1. Test the Field
        if (p.TestForBuilding() == false)
            return false;

        //2. Build the Building
        GameObject gO = p.buildBuilding(buildingPrefabs[(int)t], this.transform);
        buildings.Add(AddBuildingForType(t, gO));

        return true;

    }

    public Building AddBuildingForType(BuildingType t, GameObject g)
    {
        switch (t)
        {
            case BuildingType.City:
                return g.AddComponent<City>();

            case BuildingType.WoodCutter:
                return g.AddComponent<WoodCutter>();

            case BuildingType.Sawmill:
                return g.AddComponent<SawMill>();

            case BuildingType.Farm:

                return null;

            case BuildingType.Windmill:

                return null;

            case BuildingType.Bakery:

                return null;

            default: return null;
        }
    }
}
