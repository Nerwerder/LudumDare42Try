using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public enum BuildingType { city, lumberjack, sawmill, farm, windmill, bakery }
    public List<GameObject> buildingPrefabs;

    private List<Building> buildings;

    //Things the System builds
    public bool defaultBuild(int t, Place p)
    {
        switch (t)
        {
            case 3: //trees

                return true;
            case 4:     //City
                var cG = p.buildBuilding(buildingPrefabs[0], this.transform);
                var city = cG.AddComponent<City>();
                buildings.Add(city);
                return true;
            default:
                return false;
        }
    }

    //Things the Player can Build
    public bool build(BuildingType t, Place p)
    {
        //1. Test the Field
        if (p.TestForBuilding() == false)
            return false;

        //2. Build the Building
        switch (t)
        {
            case BuildingType.lumberjack:

                return true;

            case BuildingType.sawmill:

                return true;

            case BuildingType.farm:

                return true;

            case BuildingType.windmill:

                return true;

            case BuildingType.bakery:

                return true;
        }


        return false;
    }

}
