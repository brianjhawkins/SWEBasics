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
        numberVertices = 20;
        gridLength = 0.5f;
        gridWidth = 0.5f;
        A = 0.25f;
        g = 1f;
        terrainColumns = new GameObject[numberVertices, numberVertices];
        waterColumns = new GameObject[numberVertices, numberVertices];
        waterHeight = new float[numberVertices, numberVertices];
        terrainHeight = new float[numberVertices, numberVertices];
        outflowFlux = new Flux[numberVertices, numberVertices];
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

        if (time >= timeStep && count < 1)
        {
            print("Time: " + time + "\n");
            updateColumnData();
            time = 0;
            //count++;
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
                terrainHeight[x, y] = 3 * (float)x / numberVertices + 1;
                waterHeight[x, y] = 2;

                // flux = (Left, Right, Top, Bottom)
                outflowFlux[x, y] = new Flux(0, 0, 0, 0);
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
        float K;
        float volumeChange;

        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                print("========= " + x + ", " + y + " =========");
                if (y.Equals(0))
                {
                    fluxLeft = 0;
                }
                else
                {
                    fluxLeft = computeFlux(x, y, "Left");
                }

                print("FluxLeft: " + fluxLeft);

                if (y.Equals(numberVertices - 1))
                {
                    fluxRight = 0;
                }
                else
                {
                    fluxRight = computeFlux(x, y, "Right");
                }

                print("FluxRight: " + fluxRight);

                if (x.Equals(0))
                {
                    fluxTop = 0;
                }
                else
                {
                    fluxTop = computeFlux(x, y, "Top");
                }

                print("FluxTop: " + fluxTop);

                if (x.Equals(numberVertices - 1))
                {
                    fluxBottom = 0;
                }
                else
                {
                    fluxBottom = computeFlux(x, y, "Bottom");
                }

                print("FluxBottom: " + fluxBottom);

                K = Mathf.Min(1, waterHeight[x, y] * gridLength * gridWidth / ((fluxLeft + fluxRight + fluxTop + fluxBottom) * time));

                print("K: " + K);

                fluxLeft = K * fluxLeft;
                fluxRight = K * fluxRight;
                fluxTop = K * fluxTop;
                fluxBottom = K * fluxBottom;

                print("Top: " + fluxTop);

                outflowFlux[x, y] = new Flux(fluxLeft, fluxRight, fluxTop, fluxBottom);
                print("Flux: " + outflowFlux[x, y]);
            }
        }

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
                //print("------------  " + x + ", " + y + "  -------------");
                //print("Volume change: " + volumeChange);
                //print("1: " + waterHeight[x, y]);
                waterHeight[x, y] = waterHeight[x, y] + (volumeChange / (gridLength * gridWidth));
                //print("2: " + waterHeight[x, y]);
                waterColumns[x, y].transform.position = new Vector3(x * gridWidth, (float)((waterHeight[x, y] / 2) + terrainHeight[x, y]), y * gridLength);
                waterColumns[x, y].transform.localScale = new Vector3(gridWidth, waterHeight[x, y], gridLength);
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
}