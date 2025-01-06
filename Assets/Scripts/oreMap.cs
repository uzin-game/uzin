using UnityEngine;

public class oreMap : MonoBehaviour
{
    private int width = 256;
    private int height = 256;

    private float scale = 20f;

    private float offsetX = 100f;
    private float offsetY = 100f;

    public Color coalColor = Color.black;
    public bool[,] oreLocations;

    void Start()
    {
        oreLocations = new bool[width,height];
        offsetX = Random.Range(0f,9999f);
    }

    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width,height);
        //generate perlin noise map
        for (int x = 0; x<width;x++)
        {
            for (int y = 0; y<height;y++)
            {
                float xCoord = (float)x/width * scale + offsetX;
                float yCoord = (float)y/height * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord,yCoord);


                Color color = new Color();
                if (sample > 0.9f) 
                {
                    color = coalColor;
                    oreLocations[x,y] = true;
                }
                texture.SetPixel(x,y,color);
            }
        }
        texture.Apply();
        return texture;
    }

}
