using UnityEngine;

public class TileMaplesgo : MonoBehaviour
{
    [Header("Terrain Colors")]
    public GameObject waterTile;
    public GameObject grassTile;
    public GameObject mountainTile;

    [Header("Map Settings")]
    public int mapWidth = 256;
    public int mapHeight = 256;
    public float scale = 20f;
    public float offsetX = 100f;
    public float offsetY = 100f;

    public void GenerateTileMap()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float xCoord = (float)x / mapWidth * scale + offsetX;
                float yCoord = (float)y / mapHeight * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                GameObject tilePrefab;
                if (sample < 0.4f) tilePrefab = waterTile;
                else if (sample < 0.7f) tilePrefab = grassTile;
                else tilePrefab = mountainTile;

                Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        GenerateTileMap();
    }

}
