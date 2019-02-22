using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBehavior : MonoBehaviour
{
    List<GameObject> spheres = new List<GameObject>();
    List<float> yPositions = new List<float>();
    List<float> velocities = new List<float>();
    List<float> newHeights = new List<float>();
    public int numberVertices;
    public float time;
    public float timeStep;
    public float waveSpeed;
    public float clamp;

    // Start is called before the first frame update
    void Start()
    {
        numberVertices = 200;
        timeStep = 0.02f;
        time = 0;
        waveSpeed = 50;
        clamp = 0.9f;
        spawnVertices();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time >= timeStep)
        {
            print("Time: " + time + "\n");
            updateVertices();
            time -= timeStep;
        }
    }

    void spawnVertices()
    {
        for (int x = 0; x < numberVertices; x++)
        {
            yPositions.Add((float)(20*x/numberVertices + 10f));
            velocities.Add(0);
            newHeights.Add(0);
            spheres.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
            spheres[x].transform.position = new Vector3((float)x, (float)yPositions[x]/2, 0);
            spheres[x].transform.localScale = new Vector3(1, 2 * spheres[x].transform.position.y, 1);
            spheres[x].GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1f, 1);
        }
    }

    void updateVertices()
    {
        int leftIndex;
        int rightIndex;
        float f;
        float newVelocity;

        for(int i = 0; i < numberVertices; i++)
        {
            if(i.Equals(0))
            {
                leftIndex = i;
                rightIndex = i + 1;
            }
            else if(i.Equals(numberVertices - 1))
            {
                leftIndex = i - 1;
                rightIndex = i;
            }
            else
            {
                leftIndex = i - 1;
                rightIndex = i + 1;
            }

            f = (waveSpeed * waveSpeed) * ((yPositions[leftIndex] + yPositions[rightIndex]) - (2 * yPositions[i]));
            newVelocity = velocities[i] + (f * timeStep);
            velocities[i] = clamp*newVelocity;
            newHeights[i] = yPositions[i] + (velocities[i] * timeStep);
        }

        for(int i = 0; i < numberVertices; i++)
        {
            yPositions[i] = newHeights[i];
            spheres[i].transform.position = new Vector3((float)i, (float)yPositions[i]/2, 0);
            spheres[i].transform.localScale = new Vector3(1, 2 * spheres[i].transform.position.y, 1);
        }
    }
}
