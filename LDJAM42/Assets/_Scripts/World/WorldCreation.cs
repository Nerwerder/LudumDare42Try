using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreation : MonoBehaviour
{
    public GameObject hexagonPrefab;
    public GameObject connecionPointPrefab;

    public TextAsset worldFile;

    public Buildings buildings;
    public List<Material> materials;

    public float xOffset, zOffset, x2Offset;

    private World world;
    private char lineSeperator = '\n';
    private char fieldSeperator = ' ';
    private int diferentFieldTypes = 4;

    private void Awake()
    {
        //1. Create the World
        world = this.GetComponent<World>();

        //2. Parse the Table and build the World
        CreateWorld();
    }

    public World GetWorld()
    {
        return world;
    }

    private void CreateWorld()
    {
        System.DateTime start = System.DateTime.Now;

        //Create All the Tiles from the txt File
        CreateTiles();

        //Create the Neighborhoods
        DetermineNeighbors();

        //Create the ConnectionPoints
        CreateConnectionPoints();

        //Create Connections between ConnectionPoints
        CreateConnections();

        System.DateTime end = System.DateTime.Now;
        System.TimeSpan time = end - start;
        Debug.Log("Worldcreation Done - Time : " + time.TotalMilliseconds + " ms");

    }

    private void CreateTiles()
    {
        if (worldFile == null)
            Debug.Log("no worldFile");

        string[] lines = worldFile.text.Split(lineSeperator);

        int lineCounter = 0;
        int columnCounter = 0;
        int counter = 0;

        foreach (var line in lines)
        {
            string[] entrys = line.Split(fieldSeperator);

            columnCounter = 0;
            foreach (var entry in entrys)
            {
                CreatePlace(entry, lineCounter, columnCounter, counter);
                columnCounter++;
                counter++;
            }

            lineCounter++;
        }

        //set the WorldSize
        world.SetSize(lineCounter, columnCounter, xOffset, zOffset, counter);
    }

    private void CreatePlace(string entry, int line, int column, int counter)
    {
        //get the Int from the Entry
        int type = int.Parse(entry);

        if (type > diferentFieldTypes)
            Debug.Log("Field " + line + "|" + column + " has the wrong Type: " + type);

        //Create the Hex-Field
        Vector3 pos = GetPlacePosition(column, line);

        var nHex = Instantiate(hexagonPrefab, pos, hexagonPrefab.transform.rotation, this.transform);

        //Set the Material
        nHex.GetComponent<Renderer>().material = materials[type - 1];

        //Create and Register the Place
        var nPla = nHex.AddComponent<Place>();
        nPla.Set((type - 1), column, line, counter);
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

    private void DetermineNeighbors()
    {
        foreach (var p in world.GetPlaces())
            p.neighborhood = world.DetermineNeighborhood(p);
    }

    private void CreateConnectionPoints()
    {
        Place n1, n2;
        int n1ID, n2ID;
        foreach (Place n0 in world.GetPlaces())
        {
            //A ConnectionPoint exists if 3 places Connect with each other -> Test this place with every possible Neighborhood Pair
            for (int k = 0; k < n0.neighborhood.GetNeighbors().Count; ++k)
            {
                //Special Case: only two Neighbors
                if (k == 1 && n0.neighborhood.GetNeighbors().Count == 2)
                    continue;

                n1ID = n0.neighborhood.GetNeighbors()[k].place.id;

                if ((k + 1) < n0.neighborhood.GetNeighbors().Count)
                    n2ID = n0.neighborhood.GetNeighbors()[(k + 1)].place.id;
                else
                    n2ID = n0.neighborhood.GetNeighbors()[0].place.id; //The Circle is closed!


                //We know Allready that p is Neighbor of n1 and n2, so we only have to test if n1 and n2 are Neighbors
                n1 = world.GetPlace(n1ID);
                if (!n1.neighborhood.Contains(n2ID))
                    continue;

                //Test if this ConnectionPoint exists allready (the System will try to create every ConnectionPoint 3 Times)
                n2 = world.GetPlace(n2ID);
                if (n0.ContainsConnectionPoint(n1, n2))
                    continue;

                //There is a ConnectionPoint!
                var nCon = Instantiate(connecionPointPrefab, GetMiddle(n0, n1, n2), connecionPointPrefab.transform.rotation, n0.transform);
                var con = nCon.GetComponent<ConnectionPoint>();

                //Register the ConnectionPoint with n0, n1, n2  
                con.RegisterPlaces(n0, n1, n2);
            }
        }
    }

    private Vector3 GetMiddle(Place p0, Place p1, Place p2)
    {
        List<Vector3> vs = new List<Vector3>() { p0.transform.position, p1.transform.position, p2.transform.position };

        float minX = vs[0].x, maxX = vs[0].x, minZ = vs[0].z, maxZ = vs[0].z;
        for (int k = 1; k < 3; ++k)
        {
            //X
            if (minX > vs[k].x)
                minX = vs[k].x;
            else if (maxX < vs[k].x)
                maxX = vs[k].x;

            //Z
            if (minZ > vs[k].z)
                minZ = vs[k].z;
            else if (maxZ < vs[k].z)
                maxZ = vs[k].z;
        }

        Vector3 of = new Vector3(maxX - ((maxX - minX) / 2), 0.1f, maxZ - ((maxZ - minZ) / 2));

        //WORKAROUND (FIX THIS)
        //Correction-Factor
        float cZ = 0.25f;
        int dC = 0;

        if (p0.neighborhood.getPosition(p1.id).level == NeighborPosition.Level.DOWN)
            ++dC;
        if (p0.neighborhood.getPosition(p2.id).level == NeighborPosition.Level.DOWN)
            ++dC;

        if (dC < 2)
            of.z += cZ;
        else
            of.z -= cZ;

        return of;
    }

    public void CreateConnections()
    {
        foreach (var p in world.GetPlaces())
            foreach (var c1 in p.GetConnectionPoints())
                foreach (var c2 in p.GetConnectionPoints())
                    c1.TestForConnection(c2);
    }
}
