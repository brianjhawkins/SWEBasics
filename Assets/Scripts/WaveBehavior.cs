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
        timeStep = 0.05f;
        time = 0;
        waveSpeed = 1;
        clamp = 0.99f;
        spawnVertices();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time >= timeStep)
        {
            updateVertices();
            time -= timeStep;
        }
    }

    void spawnVertices()
    {
        for (int x = 0; x < numberVertices; x++)
        {
            yPositions.Add(Random.value * 2 + 10);
            velocities.Add(0);
            newHeights.Add(0);
            spheres.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
            spheres[x].transform.position = new Vector3((float)x, yPositions[x], 0);
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
            velocities[i] = newVelocity;
            newHeights[i] = yPositions[i] + (velocities[i] * timeStep);

            if (i.Equals(1))
            {
                print("Velocity : " + newVelocity + "\n");
                print("Velocity 2: " + velocities[i] + "\n");
                print("Height : " + yPositions[i] + "\n");
                print("New Height: " + newHeights[i] + "\n");
                print("F: " + f + "\n");
            }
        }

        for(int i = 0; i < numberVertices; i++)
        {
            yPositions[i] = newHeights[i];
            spheres[i].transform.position = new Vector3((float)i, yPositions[i], 0);
        }
    }
}
