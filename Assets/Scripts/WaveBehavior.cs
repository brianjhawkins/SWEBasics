using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBehavior : MonoBehaviour
{
    GameObject[,] columns;
    float[,] yPositions;
    float[,] velocities;
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
        columns = new GameObject[numberVertices, numberVertices];
        yPositions = new float[numberVertices, numberVertices];
        velocities = new float[numberVertices, numberVertices];
        newHeights = new float[numberVertices, numberVertices];
        timeStep = 0.02f;
        time = 0;
        waveSpeed = 13;
        clamp = 0.99f;
        spawnVertices();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= timeStep)
        {
            print("Time: " + time + "\n");
            updateVertices();
            time = 0;
        }
    }

    void spawnVertices()
    {
        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++) { 
                yPositions[x, y] = (float)((x+y) / numberVertices + 1f);
                velocities[x, y] = 0;
                newHeights[x, y] = 0;
                columns[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                columns[x, y].transform.position = new Vector3((float)x, (float)yPositions[x, y] / 2, y);
                columns[x, y].transform.localScale = new Vector3(1, 2 * columns[x, y].transform.position.y, 1);
                columns[x, y].GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1f, 1);
            }
        }
    }

    void updateVertices()
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
                if (x.Equals(0))
                {
                    northIndex = new Vector2Int(x, y);
                    southIndex = new Vector2Int(x + 1, y);
                }
                else if (x.Equals(numberVertices - 1))
                {
                    northIndex = new Vector2Int(x - 1, y);
                    southIndex = new Vector2Int(x, y);
                }
                else
                {
                    northIndex = new Vector2Int(x - 1, y);
                    southIndex = new Vector2Int(x + 1, y);
                }

                if (y.Equals(0))
                {
                    eastIndex = new Vector2Int(x, y + 1);
                    westIndex = new Vector2Int(x, y);
                }
                else if (y.Equals(numberVertices - 1))
                {
                    eastIndex = new Vector2Int(x, y);
                    westIndex = new Vector2Int(x, y - 1);
                }
                else
                {
                    eastIndex = new Vector2Int(x, y + 1);
                    westIndex = new Vector2Int(x, y - 1);
                }

                f = (waveSpeed * waveSpeed) * ((yPositions[northIndex.x, northIndex.y] + yPositions[eastIndex.x, eastIndex.y] + yPositions[southIndex.x, southIndex.y] + yPositions[westIndex.x, westIndex.y]) - (4 * yPositions[x, y]));
                newVelocity = velocities[x, y] + (f * timeStep);
                velocities[x, y] = clamp * newVelocity;
                newHeights[x, y] = yPositions[x, y] + (velocities[x, y] * timeStep);
            }
        }

        for (int x = 0; x < numberVertices; x++)
        {
            for (int y = 0; y < numberVertices; y++)
            {
                yPositions[x, y] = newHeights[x, y];
                columns[x, y].transform.position = new Vector3(x, (float)yPositions[x, y] / 2, y);
                columns[x, y].transform.localScale = new Vector3(1, 2 * columns[x, y].transform.position.y, 1);
            }
        }
    }
}