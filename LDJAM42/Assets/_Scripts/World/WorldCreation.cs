using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreation : MonoBehaviour
{
    public GameObject hexagon;
    public TextAsset worldFile;
    public List<Material> materials;

    public float xOffset, zOffset, x2Offset;


    World world;
    char lineSeperator = '\n';
    char fieldSeperator = ' ';
    int diferentFieldTypes = 4;

    private void Start()
    {
        //1. Create the World
        creatWorld();

        //2. Load the World from the Table
        loadTable();
    }

    private void creatWorld()
    {
        this.gameObject.AddComponent<World>();
        world = this.GetComponent<World>();
    }

    private void loadTable()
    {
        if (worldFile == null)
            Debug.Log("no worldFile");

        string[] lines = worldFile.text.Split(lineSeperator);

        int lineCounter = 0;
        foreach (var line in lines)
        {
            string[] entrys = line.Split(fieldSeperator);
            int columnCounter = 0;

            foreach (var entry in entrys)
            {
                createPlace(entry, lineCounter, columnCounter);
                columnCounter++;
            }

            lineCounter++;
        }
    }

    private void createPlace(string entry, int line, int column)
    {
        //get the Int from the Entry
        int type = int.Parse(entry);

        if (type > diferentFieldTypes)
            Debug.Log("Field " + line + "|" + column + " has the wrong Type: " + type);

        //Create the Hex-Field
        Vector3 pos = getPlacePosition(column, line);

        var nHex = Instantiate(hexagon, pos, hexagon.transform.rotation);

        //Set the Material
        nHex.GetComponent<Renderer>().material = materials[type - 1];

        //Create and Register the Place
        var nPla = nHex.AddComponent<Place>();
        nPla.set(type, column, line);
        world.registerPlace(nPla);
    }

    private Vector3 getPlacePosition(int x, int z)
    {
        if (z % 2 == 0)
            return new Vector3(x * xOffset, 0, -(zOffset * z));
        else
            return new Vector3(x * xOffset + x2Offset, 0, -(zOffset * z));
    }
}
