using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUWaveBehavior : MonoBehaviour
{
    private Renderer waterRenderer;
    private Texture2D columnData;
    private Texture2D fluxData;
    private Texture2D velocityData;

    public int numberVertices;
    public float gridLength;
    public float gridWidth;

    // Start is called before the first frame update
    void Start()
    {
        waterRenderer = GetComponent<Renderer>();
        numberVertices = 256;
        gridLength = gridWidth = 1;
        columnData = new Texture2D(numberVertices, numberVertices);
        fluxData = new Texture2D(numberVertices, numberVertices);
        velocityData = new Texture2D(numberVertices, numberVertices);

        initializeShaderTextures();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initializeShaderTextures()
    {
        for(int x = 0; x < numberVertices; x++)
        {
            for(int y = 0; y < numberVertices; y++)
            {
                // r = terrain height, g = water height, b = sediment height
                columnData.SetPixel(x, y, new Color(Mathf.Sin(x+y), 1f, 0.0f, 1f));

                // r = flux left, g = flux right, b = flux top, a = flux bottom
                fluxData.SetPixel(x, y, new Color(0, 0, 0, 1));

                // r = velocity in x direction, g = velocity in y direction
                velocityData.SetPixel(x, y, new Color(0, 0, 1, 1));
            }
        }
        
        waterRenderer.material.SetTexture("_CDTex", columnData);
        waterRenderer.material.SetTexture("_FTex", fluxData);
        waterRenderer.material.SetTexture("_VTex", velocityData);
        columnData.Apply();
        fluxData.Apply();
        velocityData.Apply();
    }
}
