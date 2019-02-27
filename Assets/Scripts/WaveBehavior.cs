using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBehavior : MonoBehaviour
{
    GameObject[,] waterColumns;
    GameObject[,] terrainColumns;
    float[,] waterHeight;
    float[,] terrainHeight;
    Vector4[,] outflowFlux;
    Vector2[,] velocity;
    float[,] newWaterHeight;

    public int numberVertices;
    public float gridLength;
    public float gridWidth;
    public float time;
    public float timeStep;
    public float waveSpeed;
    public float clamp;

    // Start is called before the first frame update
    void Start()
    {
        numberVertices = 100;
        gridLength = 0.5f;
        gridWidth = 0.5f;
        terrainColumns = new GameObject[numberVertices, numberVertices];
        waterColumns = new GameObject[numberVertices, numberVertices];
        waterHeight = new float[numberVertices, numberVertices];
        terrainHeight = new float[numberVertices, numberVertices];
        outflowFlux = new Vector4[numberVertices, numberVertices];
        velocity = new Vector2[numberVertices, numberVertices];
        newWaterHeight = new float[numberVertices, numberVertices];
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
            updateColumnData();
            time = 0;
        }
    }

    void spawnColumns()
    {
        float tempTerrainHeight;
        float tempWaterHeight;
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                terrainHeight[x, y] = 3*x/numberVertices + 1;
                waterHeight[x, y] = 2;

                // flux = (Left, Right, Top, Bottom)
                outflowFlux[x, y] = new Vector4(0, 0, 0, 0);
                velocity[x, y] = new Vector2(0, 0);
                tempTerrainHeight = terrainHeight[x, y];
                tempWaterHeight = waterHeight[x, y];
                newWaterHeight[x, y] = 0;

                terrainColumns[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                terrainColumns[x, y].transform.position = new Vector3(x * gridWidth, (float)(tempTerrainHeight / 2), y * gridLength);
                terrainColumns[x, y].transform.localScale = new Vector3(gridWidth, tempTerrainHeight, gridLength);
                waterColumns[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                waterColumns[x, y].transform.position = new Vector3(x * gridWidth, (float)((tempWaterHeight / 2) + tempTerrainHeight), y * gridLength);
                waterColumns[x, y].transform.localScale = new Vector3(gridWidth, tempWaterHeight, gridLength);
                waterColumns[x, y].GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1f, 1);
            }
        }
    }

    void updateColumnData()
    {
        float fluxLeft;
        float fluxRight;
        float fluxTop;
        float fluxBottom;
        float A = 1;
        float g = -0.5f;
        float l = gridLength;
        float K;
        float volumeChange;

        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                if (y.Equals(0)) {
                    fluxLeft = 0;
                }
                else {
                    fluxLeft = Mathf.Max(0, outflowFlux[x, y].x) + time * A * (g * heightDifferenceLeft(x, y) / l);
                }

                if (y.Equals(numberVertices - 1))
                {
                    fluxRight = 0;
                }
                else
                {
                    fluxRight = Mathf.Max(0, outflowFlux[x, y].x) + time * A * (g * heightDifferenceRight(x, y) / l);
                }

                if (x.Equals(0))
                {
                    fluxTop = 0;
                }
                else
                {
                    fluxTop = Mathf.Max(0, outflowFlux[x, y].x) + time * A * (g * heightDifferenceTop(x, y) / l);
                }

                if (x.Equals(numberVertices - 1))
                {
                    fluxBottom = 0;
                }
                else
                {
                    fluxBottom = Mathf.Max(0, outflowFlux[x, y].x) + time * A * (g * heightDifferenceBottom(x, y) / l);
                }

                K = Mathf.Min(1, waterHeight[x, y] * gridLength * gridWidth / ((fluxLeft + fluxRight + fluxTop + fluxBottom) * time));

                fluxLeft = K * fluxLeft;
                fluxRight = K * fluxRight;
                fluxTop = K * fluxTop;
                fluxBottom = K * fluxBottom;

                outflowFlux[x, y] = new Vector4(fluxLeft, fluxRight, fluxTop, fluxBottom);
            }
        }

        for(int x = 0; x < numberVertices; x++)
        {
            for(int y = 0; y < numberVertices; y++)
            {
                if (y.Equals(0))
                {
                    fluxLeft = 0;
                }
                else
                {
                    fluxLeft = inflowFluxLeft(x, y);
                }

                if (y.Equals(numberVertices - 1))
                {
                    fluxRight = 0;
                }
                else
                {
                    fluxRight = inflowFluxRight(x, y);
                }

                if (x.Equals(0))
                {
                    fluxTop = 0;
                }
                else
                {
                    fluxTop = inflowFluxTop(x, y);
                }

                if (x.Equals(numberVertices - 1))
                {
                    fluxBottom = 0;
                }
                else
                {
                    fluxBottom = inflowFluxBottom(x, y);
                }

                volumeChange = time * ((fluxLeft + fluxRight + fluxTop + fluxBottom) - (outflowFlux[x, y].x + outflowFlux[x, y].y + outflowFlux[x, y].z + outflowFlux[x, y].w));
                print("1: " + waterHeight[x, y]);
                waterHeight[x, y] = waterHeight[x, y] + (volumeChange / (gridLength * gridWidth));
                print("2: " + waterHeight[x, y]);
                waterColumns[x, y].transform.position = new Vector3(x * gridWidth, (float)((waterHeight[x, y] / 2) + terrainHeight[x, y]), y * gridLength);
                waterColumns[x, y].transform.localScale = new Vector3(gridWidth, waterHeight[x, y], gridLength);
            }
        }
    }

    float heightDifferenceLeft(int x, int y)
    {
        return waterHeight[x, y] + terrainHeight[x, y] - waterHeight[x, y - 1] - terrainHeight[x, y - 1];
    }

    float heightDifferenceRight(int x, int y)
    {
        return waterHeight[x, y] + terrainHeight[x, y] - waterHeight[x, y + 1] - terrainHeight[x, y + 1];
    }

    float heightDifferenceTop(int x, int y)
    {
        return waterHeight[x, y] + terrainHeight[x, y] - waterHeight[x - 1, y] - terrainHeight[x - 1, y];
    }

    float heightDifferenceBottom(int x, int y)
    {
        return waterHeight[x, y] + terrainHeight[x, y] - waterHeight[x + 1, y] - terrainHeight[x + 1, y];
    }

    float inflowFluxLeft(int x, int y)
    {
        return outflowFlux[x, y - 1].y;
    }

    float inflowFluxRight(int x, int y)
    {
        return outflowFlux[x, y + 1].x;
    }

    float inflowFluxTop(int x, int y)
    {
        return outflowFlux[x - 1, y].w;
    }

    float inflowFluxBottom(int x, int y)
    {
        return outflowFlux[x + 1, y].z;
    }
}