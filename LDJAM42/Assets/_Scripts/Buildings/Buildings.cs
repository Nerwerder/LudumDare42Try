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
                int treeCount = Random.Range(1, 4);
                for (int i = 0; i < treeCount; i++)
                {
                    Vector3 treepos;
                    do
                    {
                        float deltaX = Random.Range(-0.55f, 0.55f);
                        float deltaZ = Random.Range(-0.55f, 0.55f);
                        treepos = new Vector3(p.transform.position.x + deltaX, p.transform.position.y + 0.114f, p.transform.position.z + deltaZ);
                    } while (checkTreesInRange(treepos));
                    

                    float sizeModifier = Random.Range(1.5f, 2.5f) + 1/treeCount;
                    GameObject tree = Instantiate(buildingPrefabs[6], treepos, p.transform.rotation, this.transform);
                    Vector3 scale = new Vector3(tree.transform.localScale.x * sizeModifier, tree.transform.localScale.y * sizeModifier, tree.transform.localScale.z * sizeModifier);
                    tree.transform.localScale = scale;
                }
                return true;
            case 4:     //City
                var gO = p.BuildBuilding(buildingPrefabs[0], this.transform);
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
        GameObject gO = p.BuildBuilding(buildingPrefabs[(int)t], this.transform);
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
                return g.AddComponent<Farm>();

            case BuildingType.Windmill:
                return g.AddComponent<WindMill>();

            case BuildingType.Bakery:
                return g.AddComponent<Bakery>();

            default: return null;
        }
    }

    private bool checkTreesInRange(Vector3 pos)
    {
        Collider[] hitcolliders = Physics.OverlapSphere(pos, 0.1f);
        int i = 0;
        while (i < hitcolliders.Length)
        {
            if(hitcolliders[i].tag == "tree")
            {
                return true;
            }
            i++;
        }
        return false;
    }

}
