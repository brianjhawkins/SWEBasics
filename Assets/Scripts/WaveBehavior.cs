using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Flux
{
    private float left;
    private float right;
    private float top;
    private float bottom;

    public Flux(float l, float r, float t, float b)
    {
        left = l;
        right = r;
        top = t;
        bottom = b;
    }

    public float getLeft()
    {
        return left;
    }

    public float getRight()
    {
        return right;
    }

    public float getTop()
    {
        return top;
    }

    public float getBottom()
    {
        return bottom;
    }
}

public class WaveBehavior : MonoBehaviour
{
    private GameObject[,] waterColumns;
    private GameObject[,] terrainColumns;
    private float[,] waterHeight;
    private float[,] terrainHeight;
    private Flux[,] outflowFlux;
    private Vector2[,] velocity;
    private float[,] newWaterHeight;
    private Material waterMat;

    float A;
    float g;
    public int numberVertices;
    public float gridLength;
    public float gridWidth;
    public float time;
    public float timeStep;
    public float waveSpeed;
    public float clamp;
    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        numberVertices = 100;
        gridLength = 0.1f;
        gridWidth = 0.1f;
        A = 0.01f;
        g = 2f;
        terrainColumns = new GameObject[numberVertices, numberVertices];
        waterColumns = new GameObject[numberVertices, numberVertices];
        waterHeight = new float[numberVertices, numberVertices];
        terrainHeight = new float[numberVertices, numberVertices];
        outflowFlux = new Flux[numberVertices, numberVertices];
        velocity = new Vector2[numberVertices, numberVertices];
        newWaterHeight = new float[numberVertices, numberVertices];
        timeStep = 0.01f;
        time = 0;
        waveSpeed = 13;
        clamp = 0.99f;
        spawnColumns();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= timeStep && count < 1)
        {
            // print("Time: " + time + "\n");
            updateColumnData();
            time = 0;
            //count++;
        }
    }

    void spawnColumns()
    {
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                terrainHeight[x, y] = 3 * (float)((x + y) % numberVertices)/numberVertices + 1;
                waterHeight[x, y] = 1;

                outflowFlux[x, y] = new Flux(0, 0, 0, 0);
                velocity[x, y] = new Vector2(0, 0);
                newWaterHeight[x, y] = 0;

                spawnTerrain(x, y);
                spawnWater(x, y);
                scaleWater(x, y);
            }
        }
    }

    void updateColumnData()
    {
        float fluxLeft;
        float fluxRight;
        float fluxTop;
        float fluxBottom;
        float K;
        float volumeChange;
        Flux[,] tempFlux = new Flux[numberVertices, numberVertices];

        // Calculate and store new flux values for each vertex in a temporary array
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                if (y.Equals(0))
                {
                    fluxLeft = 0;
                }
                else
                {
                    fluxLeft = computeFlux(x, y, "Left");
                }

                if (y.Equals(numberVertices - 1))
                {
                    fluxRight = 0;
                }
                else
                {
                    fluxRight = computeFlux(x, y, "Right");
                }

                if (x.Equals(0))
                {
                    fluxTop = 0;
                }
                else
                {
                    fluxTop = computeFlux(x, y, "Top");
                }

                if (x.Equals(numberVertices - 1))
                {
                    fluxBottom = 0;
                }
                else
                {
                    fluxBottom = computeFlux(x, y, "Bottom");
                }

                if (fluxLeft != 0 || fluxRight != 0 || fluxTop != 0 || fluxBottom != 0)
                {
                    K = Mathf.Min(1, waterHeight[x, y] * gridLength * gridWidth / ((fluxLeft + fluxRight + fluxTop + fluxBottom) * time));
                }
                else
                {
                    K = 0;
                }

                fluxLeft = K * fluxLeft;
                fluxRight = K * fluxRight;
                fluxTop = K * fluxTop;
                fluxBottom = K * fluxBottom;

                tempFlux[x, y] = new Flux(fluxLeft, fluxRight, fluxTop, fluxBottom);
            }
        }

        // Replace old flux values with new ones from temporary array
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                outflowFlux[x, y] = tempFlux[x, y];
            }
        }

        // Use fluxes to change height of each vertex
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                if (y.Equals(0))
                {
                    fluxLeft = 0;
                }
                else
                {
                    fluxLeft = inflowFlux(x, y, "Left");
                }

                if (y.Equals(numberVertices - 1))
                {
                    fluxRight = 0;
                }
                else
                {
                    fluxRight = inflowFlux(x, y, "Right");
                }

                if (x.Equals(0))
                {
                    fluxTop = 0;
                }
                else
                {
                    fluxTop = inflowFlux(x, y, "Top");
                }

                if (x.Equals(numberVertices - 1))
                {
                    fluxBottom = 0;
                }
                else
                {
                    fluxBottom = inflowFlux(x, y, "Bottom");
                }

                volumeChange = time * ((fluxLeft + fluxRight + fluxTop + fluxBottom) - (outflowFlux[x, y].getLeft() + outflowFlux[x, y].getRight() + outflowFlux[x, y].getTop() + outflowFlux[x, y].getBottom()));
                waterHeight[x, y] = waterHeight[x, y] + (volumeChange / (gridLength * gridWidth));
                scaleWater(x, y);
            }
        }
    }

    float computeFlux(int x, int y, string s)
    {
        float flux;

        switch (s)
        {
            case "Left":
                flux = Mathf.Max(0, outflowFlux[x, y].getLeft() + time * A * (g * heightDifference(x, y, s) / gridLength));
                break;
            case "Right":
                flux = Mathf.Max(0, outflowFlux[x, y].getRight() + time * A * (g * heightDifference(x, y, s) / gridLength));
                break;
            case "Top":
                flux = Mathf.Max(0, outflowFlux[x, y].getTop() + time * A * (g * heightDifference(x, y, s) / gridLength));
                break;
            case "Bottom":
                flux = Mathf.Max(0, outflowFlux[x, y].getBottom() + time * A * (g * heightDifference(x, y, s) / gridLength));
                break;
            default:
                print("Error: String " + s + " in computeFlux() not recognized");
                flux = 0;
                break;
        }

        return flux;
    }

    float heightDifference(int x, int y, int a, int b)
    {
        return waterHeight[x, y] + terrainHeight[x, y] - (waterHeight[a, b] + terrainHeight[a, b]);
    }

    float heightDifference(int x, int y, string s)
    {

        float difference;

        switch (s)
        {
            case "Left":
                difference = heightDifference(x, y, x, y - 1);
                break;
            case "Right":
                difference = heightDifference(x, y, x, y + 1);
                break;
            case "Top":
                difference = heightDifference(x, y, x - 1, y);
                break;
            case "Bottom":
                difference = heightDifference(x, y, x + 1, y);
                break;
            default:
                print("Error: String " + s + " in heightDifference() not recognized");
                difference = 0;
                break;
        }

        return difference;
    }

    float inflowFlux(int x, int y, string s)
    {
        float inflow;

        switch (s)
        {
            case "Left":
                inflow = outflowFlux[x, y - 1].getRight();
                break;
            case "Right":
                inflow = outflowFlux[x, y + 1].getLeft();
                break;
            case "Top":
                inflow = outflowFlux[x - 1, y].getBottom();
                break;
            case "Bottom":
                inflow = outflowFlux[x + 1, y].getTop();
                break;
            default:
                print("Error: String " + s + " in inflowFlux() not recognized");
                inflow = 0;
                break;
        }

        return inflow;
    }

    void scaleWater(int x, int y)
    {
        float height = Mathf.Max(0, waterHeight[x, y]);
        waterColumns[x, y].transform.position = new Vector3(x * gridWidth, (float)((height / 2) + terrainHeight[x, y]), y * gridLength);
        waterColumns[x, y].transform.localScale = new Vector3(gridWidth, height, gridLength);
    }

    void spawnWater(int x, int y)
    {
        waterColumns[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        waterColumns[x, y].GetComponent<Renderer>().material.color = new Color(0, 1, 1, 1);
    }

    void spawnTerrain(int x, int y)
    {
        float height = Mathf.Max(0, terrainHeight[x, y]);
        terrainColumns[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terrainColumns[x, y].transform.position = new Vector3(x * gridWidth, (float)(height / 2), y * gridLength);
        terrainColumns[x, y].transform.localScale = new Vector3(gridWidth, height, gridLength);
    }
}