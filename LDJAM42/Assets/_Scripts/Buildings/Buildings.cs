using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public enum BuildingType { City, WoodCutter, Sawmill, Farm, Windmill, Bakery }
    public List<GameObject> buildingPrefabs;
    public int CostPerBuilding = 80;

    public GameObject IOMarkerParent;
    public GameObject IOMarker;
    public Material inputMarkerMaterial, outputMarkerMaterial, potentialMarkerMaterial;

    private City city;

    //Things the System builds
    public bool DefaultBuild(Place p)
    {
        switch (p.type)
        {
            case Place.PlaceType.Forest:     //Trees
                int treeCount = Random.Range(1, 4);
                for (int i = 0; i < treeCount; i++)
                {
                    Vector3 treepos;
                    int whileThreshold = 20;
                    int counter = 0;
                    do
                    {
                        float deltaX = Random.Range(-0.35f, 0.35f);
                        float deltaZ = Random.Range(-0.35f, 0.35f);
                        treepos = new Vector3(p.transform.position.x + deltaX, p.transform.position.y + 0.114f, p.transform.position.z + deltaZ);
                        ++counter;
                    } while (checkTreesInRange(treepos) && counter < whileThreshold);


                    float sizeModifier = Random.Range(0.8f, 1.2f) + 1 / treeCount;
                    GameObject tree = Instantiate(buildingPrefabs[6], treepos, p.transform.rotation, this.transform);
                    Vector3 scale = new Vector3(tree.transform.localScale.x * sizeModifier, tree.transform.localScale.y * sizeModifier, tree.transform.localScale.z * sizeModifier);
                    tree.transform.localScale = scale;
                }
                return true;
            case Place.PlaceType.City:     //City
                city = (p.BuildBuilding(buildingPrefabs[0])).GetComponent<City>();
                city.SetIOMarker(IOMarker, inputMarkerMaterial, outputMarkerMaterial, potentialMarkerMaterial, IOMarkerParent);
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
        if (city.GetGold(CostPerBuilding))
            (p.BuildBuilding(buildingPrefabs[(int)t])).SetIOMarker(IOMarker, inputMarkerMaterial, outputMarkerMaterial, potentialMarkerMaterial, IOMarkerParent);
        return true;
    }

    private bool checkTreesInRange(Vector3 pos)
    {
        Collider[] hitcolliders = Physics.OverlapSphere(pos, 0.1f);
        for (int i = 0; i < hitcolliders.Length; ++i)
            if (hitcolliders[i].tag == "tree")
                return true;
        return false;
    }
}
