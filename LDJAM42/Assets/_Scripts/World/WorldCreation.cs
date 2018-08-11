using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreation : MonoBehaviour
{
    public GameObject   hexagon;
    public TextAsset    worldFile;
    public Buildings    buildings;
    public List<Material> materials;

    public float xOffset, zOffset, x2Offset;

    World world;
    char lineSeperator = '\n';
    char fieldSeperator = ' ';
    int diferentFieldTypes = 4;

    private void Awake()
    {
        //1. Create the World
        world = this.GetComponent<World>();

        //2. Load the World from the Table
        CreateWorld();
    }

    public World GetWorld()
    {
        return world;
    }

    private void CreateWorld()
    {
        if (worldFile == null)
            Debug.Log("no worldFile");

        string[] lines = worldFile.text.Split(lineSeperator);

        int lineCounter = 0;
        int columnCounter = 0;

        foreach (var line in lines)
        {
            string[] entrys = line.Split(fieldSeperator);

            columnCounter = 0;
            foreach (var entry in entrys)
            {
                CreatePlace(entry, lineCounter, columnCounter);
                columnCounter++;
            }

            lineCounter++;
        }

        //set the WorldSize
        world.SetSize(lineCounter, columnCounter, xOffset, zOffset);
    }

    private void CreatePlace(string entry, int line, int column)
    {
        //get the Int from the Entry
        int type = int.Parse(entry);

        if (type > diferentFieldTypes)
            Debug.Log("Field " + line + "|" + column + " has the wrong Type: " + type);

        //Create the Hex-Field
        Vector3 pos = GetPlacePosition(column, line);

        var nHex = Instantiate(hexagon, pos, hexagon.transform.rotation, this.transform);

        //Set the Material
        nHex.GetComponent<Renderer>().material = materials[type - 1];

        //Create and Register the Place
        var nPla = nHex.AddComponent<Place>();
        nPla.Set(type, column, line);
        world.RegisterPlace(nPla);

        //Test if there is already a Building on this Place
        buildings.DefaultBuild(type, nPla);
    }

    private Vector3 GetPlacePosition(int x, int z)
    {
        if (z % 2 == 0)
            return new Vector3(x * xOffset, 0, -(zOffset * z));
        else
            return new Vector3(x * xOffset + x2Offset, 0, -(zOffset * z));
    }


}
