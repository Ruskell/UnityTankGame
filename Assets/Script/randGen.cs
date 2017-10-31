using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randGen : MonoBehaviour {

    Mesh mesh;
    MeshFilter meshFilter;
    System.Random rand = new System.Random();

    // Use this for initialization
    void Start () {
        
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        //Create a 2d array of vectors
        Vector2[] vecArr = new Vector2[26];

        //Get a random point on the Y to start the map on
        float fRand = (float)rand.NextDouble() * 0.6f;
        vecArr[0] = new Vector2(-0.5f, fRand);

        //Iterate through 20 times creating a randomly generated series of verteces on the Y
        int iterator = 1;
        for (double i = -4.5; i <= 5; i+=0.5f)
        {
            vecArr[iterator] = new Vector3((float)(i * 0.1), getClose(vecArr[iterator-1].y));
            iterator++;
        }

        // Get the lower bounds of the object
        vecArr[iterator] = new Vector3(0.5f, -0.5f);
        vecArr[iterator + 1] = new Vector3(0.25f, -0.5f);
        vecArr[iterator + 2] = new Vector3(0, -0.5f);
        vecArr[iterator + 3] = new Vector3(-0.25f, -0.5f);
        vecArr[iterator+4] = new Vector3(-0.5f, -0.5f);

        // Convert the 2D array to a 3D to be used as the mesh
        Vector3[] vecArr3D = new Vector3[vecArr.Length];
        for (int i=0; i!=vecArr.Length; i++)
        {
            vecArr3D[i] = new Vector3(vecArr[i].x, vecArr[i].y);
        }

        //Initialising left 2 triangle vertices
        int[] tri = new int[72];
        tri[0] = 0;
        tri[1] = 1;
        tri[2] = 25;
        tri[3] = 1;
        tri[4] = 2;
        tri[5] = 25;

        //Initialising main body of terrain triangle vertices
        int pos = 2;
        int count = 6;
        for (int i=24; i>21; i--)
        {
            for (int y = 0; y != 5; y++)
            {
                tri[count] = pos;
                pos++;
                count++;
                tri[count] = pos;
                count++;
                tri[count] = i;
                count++;
            }
        }

        //Initialising right 3 triangle vertices
        tri[count] = 17;
        count++;
        tri[count] = 18;
        count++;
        tri[count] = 21;
        count++;
        tri[count] = 18;
        count++;
        tri[count] = 19;
        count++;
        tri[count] = 21;
        count++;
        tri[count] = 19;
        count++;
        tri[count] = 20;
        count++;
        tri[count] = 21;
        count++;

        //Initialising base triangle verteces to fill body of shape
        tri[count] = 2;
        count++;
        tri[count] = 25;
        count++;
        tri[count] = 24;
        count++;
        tri[count] = 7;
        count++;
        tri[count] = 24;
        count++;
        tri[count] = 23;
        count++;
        tri[count] = 12;
        count++;
        tri[count] = 23;
        count++;
        tri[count] = 22;
        count++;
        tri[count] = 17;
        count++;
        tri[count] = 22;
        count++;
        tri[count] = 21;
        count++;

        //Render randGen terrain
        mesh.vertices = vecArr3D;
        mesh.triangles = tri;
        //mesh.uv = vecArr;
        //mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        GetComponent<PolygonCollider2D>().points = vecArr;
    }

    float getClose(float f)
    {
        double terrainJaggies = 0.045;
        if (f < -0.1) return f * (float)(rand.NextDouble()*0.5);
        int test = rand.Next(0, 2);
        if (test == 0) f -= (float)(rand.NextDouble() * terrainJaggies);
        else f += (float)(rand.NextDouble() * terrainJaggies);
        return f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
