using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct ColumnData
{
    private float terrainHeight;
    private float waterHeight;
    private float sedimentHeight;
    private float[] outflowFlux;
    private Vector2 velocity;

    public ColumnData( float t, float w, float s)
    {
        terrainHeight = t;
        waterHeight = w;
        sedimentHeight = s;
        outflowFlux = new float[] { 0, 0, 0, 0 };
        velocity = new Vector2(0, 0);
    }

    public ColumnData(float t, float w, float s, float[] o, Vector2 v)
    {
        terrainHeight = t;
        waterHeight = w;
        sedimentHeight = s;
        outflowFlux = o;
        velocity = v;
    }

    public float getTerrainHeight()
    {
        return terrainHeight;
    }

    public float getWaterHeight()
    {
        return waterHeight;
    }

    public float getSedimentHeight()
    {
        return sedimentHeight;
    }

    public float[] getOutflowFlux()
    {
        return outflowFlux;
    }

    public Vector2 getVelocity()
    {
        return velocity;
    }
}

public class WaveBehavior : MonoBehaviour
{
    GameObject[,] waterColumns;
    GameObject[,] terrainColumns;
    ColumnData[,] cData;
    float[,] newHeights;
    public int numberVertices;
    public float time;
    public float timeStep;
    public float waveSpeed;
    public float clamp;

    // Start is called before the first frame update
    void Start()
    {
        numberVertices = 100;
        terrainColumns = new GameObject[numberVertices, numberVertices];
        waterColumns = new GameObject[numberVertices, numberVertices];
        cData = new ColumnData[numberVertices, numberVertices];
        newHeights = new float[numberVertices, numberVertices];
        timeStep = 0.02f;
        time = 0;
        waveSpeed = 13;
        clamp = 0.99f;
        spawnColumns();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= timeStep)
        {
            print("Time: " + time + "\n");
            //updateColumnData();
            time = 0;
        }
    }

    void spawnColumns()
    {
        float terrainHeight;
        float waterHeight;
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                cData[x, y] = new ColumnData(1, 2, 0);
                terrainHeight = cData[x, y].getTerrainHeight();
                waterHeight = cData[x, y].getWaterHeight();
        
                newHeights[x, y] = 0;
                terrainColumns[x,y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terrainColumns[x, y].transform.position = new Vector3(x, (float)(terrainHeight / 2), y);
                terrainColumns[x, y].transform.localScale = new Vector3(1, terrainHeight, 1);
                waterColumns[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                waterColumns[x, y].transform.position = new Vector3(x, (float)((waterHeight / 2) + terrainHeight), y);
                waterColumns[x, y].transform.localScale = new Vector3(1, waterHeight, 1);
                waterColumns[x, y].GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1f, 1);
            }
        }
    }

    void updateColumnData()
    {
        Vector2Int northIndex;
        Vector2Int eastIndex;
        Vector2Int southIndex;
        Vector2Int westIndex;
        float f;
        float newVelocity;

        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {

            }
        }
    }
}