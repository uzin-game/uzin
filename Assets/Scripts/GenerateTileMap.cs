using UnityEngine;

public class GenerateTileMap : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapHeight = 256;
    public int mapWidth = 256;

    public float scale = 20f;
    public float OffsetX = 100f;
    public float OffsetY = 100f;
    public float OreOffsetX = 100f;
    public float OreOffsetY = 100f;

    public Color waterColor = Color.blue;
    public Color grassColor = Color.green;
    public Color mountainColor = Color.gray;
    public Color coalColor = Color.black;

    void Start()
    {
        OffsetX = Random.Range(0,9999f);
        OffsetY = Random.Range(0,9999f);

        OreOffsetX = Random.Range(0f,9999f);
        OreOffsetY = Random.Range(0f,9999f);
    }

    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateColoredTexture();
    }

    Texture2D GenerateColoredTexture()
    {
        Texture2D texture = new Texture2D(mapWidth, mapHeight);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                
                //generate terrain color
                float xCoord = (float)x / mapWidth * scale + OffsetX;
                float yCoord = (float)y / mapHeight * scale + OffsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                Color color;
                if (sample < 0.4f) color = waterColor;
                else if (sample < 0.7f) color = grassColor;
                else color = mountainColor;

                texture.SetPixel(x, y, color);

                //generate the ressources
                

                float xCoordOre = (float)x / mapWidth * scale + OreOffsetX;
                float yCoordOre = (float)y / mapHeight * scale + OreOffsetY;
                float sample2 = Mathf.PerlinNoise(xCoordOre, yCoordOre);

                //Color orecolor;
                if (sample2>0.9f && sample>=0.4f) texture.SetPixel(x, y, coalColor);
                
            }
        }

        texture.Apply();
        return texture;
    }

    public void WaveFunctionCollapse()
    {
        
    }

}
