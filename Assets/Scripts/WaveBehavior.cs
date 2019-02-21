using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Vertex
{
    float xPos;
    float yPos;
    float velocity;
    GameObject sphere;

    public Vertex(float x, float y, float v)
    {
        xPos = x;
        yPos = y;
        velocity = v;
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(xPos, yPos, 0);
    }

    public void updateSpherePosition()
    {
        sphere.transform.position = new Vector3(this.xPos, this.yPos, 0);
    }

    public void setVelocity(float v)
    {
        velocity = v;
    }

    public void setXPos(float x)
    {
        xPos = x;
    }

    public void setYPos(float y)
    {
        yPos = y;
    }

    public float getVelocity()
    {
        return velocity;
    }

    public float getXPos()
    {
        return xPos;
    }

    public float getYPos()
    {
        return yPos;
    }
}

public class WaveBehavior : MonoBehaviour
{
    List<Vertex> waveVertices = new List<Vertex>();
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
        timeStep = 0.5f;
        time = 0;
        waveSpeed = 5;
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
            waveVertices.Add(new Vertex(x, Random.value*2 + 10, 0));
            newHeights.Add(0);
        }
    }

    void updateVertices()
    {
        int leftIndex;
        int rightIndex;
        float f;

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

            f = (waveSpeed * waveSpeed) * ((waveVertices[leftIndex].getYPos() + waveVertices[rightIndex].getYPos()) - (2 * waveVertices[i].getYPos()));
            float velocity = waveVertices[i].getVelocity() + (f * timeStep);
            waveVertices[i].setVelocity(velocity);
            
            newHeights[i] = waveVertices[i].getYPos() + (waveVertices[i].getVelocity() * timeStep);
            if (i.Equals(1))
            {
                print("Velocity : " + velocity + "\n");
                print("Velocity 2: " + waveVertices[i].getVelocity() + "\n");
                print("Height : " + waveVertices[i].getYPos() + "\n");
                print("New Height: " + newHeights[i] + "\n");
                print("F: " + f + "\n");
            }
        }

        for(int i = 0; i < numberVertices; i++)
        {
            waveVertices[i].setYPos(newHeights[i]);
            waveVertices[i].updateSpherePosition();
        }
    }
}
