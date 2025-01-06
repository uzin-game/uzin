using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public int width = 100;  // Largeur de la grille
    public int height = 100; // Hauteur de la grille
    public float scale = 5f;
    public float OffsetX = 100f;
    public float OffsetY = 100f;
    private GameObject[,] grid; // Tableau 2D pour stocker les tiles placés

    private GameObject waterTile;
    private GameObject grassTile;
    private GameObject BottomEdgeTile;
    private GameObject I_BottomLeftTile;
    private GameObject I_BottomRightTile;
    private GameObject I_TopLeftTile;
    private GameObject I_TopRightTile;
    private GameObject LeftEdgeTile;
    private GameObject O_BottomLeftTile;
    private GameObject O_BottomRightTile;
    private GameObject O_TopLeftTile;
    private GameObject O_TopRightTile;
    private GameObject RightEdgeTile;
    private GameObject TopEdgeTile;
    

    private float tileSize = 32/100f;

    void Start()
    {
        
        //nouvelles valeures aléatoires aka seed de la map
        OffsetX = Random.Range(0,9999f);
        OffsetY = Random.Range(0,9999f);
        
        // Initialiser la grille vide
        grid = new GameObject[width, height];
        
        //load up the tiles
        LoadTiles();
        
        // Générer la map
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Appelle ton algorithme Perlin noise pour déterminer quel tile placer ici
                GameObject tile = GenerateTile(x, y);
                
                // Instancier le tile à la position correspondante
                if (tile != null)
                {
                    Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                    GameObject newTile = Instantiate(tile, position, Quaternion.identity);
                    grid[x, y] = newTile;
                }
            }
        }
    }

    //on load les différentes tiles dans le dossier Resources/grid
    void LoadTiles()
    {
        waterTile = Resources.Load("Prefabs/sol_eau_0") as GameObject;
        grassTile = Resources.Load("Prefabs/sol_basique_0") as GameObject;
        BottomEdgeTile = Resources.Load("Prefabs/BottomEdge") as GameObject;
        I_BottomLeftTile = Resources.Load("Prefabs/I_BottomLeft") as GameObject;
        I_BottomRightTile = Resources.Load("Prefabs/I_BottomRight") as GameObject;
        I_TopLeftTile = Resources.Load("Prefabs/I_TopLeft") as GameObject;
        I_TopRightTile = Resources.Load("Prefabs/I_TopRight") as GameObject;
        LeftEdgeTile = Resources.Load("Prefabs/LeftEdge") as GameObject;
        O_BottomLeftTile = Resources.Load("Prefabs/O_BottomLeft") as GameObject;
        O_BottomRightTile = Resources.Load("Prefabs/O_BottomRight") as GameObject;
        O_TopLeftTile = Resources.Load("Prefabs/O_TopLeft") as GameObject;
        O_TopRightTile = Resources.Load("Prefabs/O_TopRight") as GameObject;
        RightEdgeTile = Resources.Load("Prefabs/RightEdge") as GameObject;
        TopEdgeTile = Resources.Load("Prefabs/TopEdge") as GameObject;
    }

    GameObject GenerateTile(int x, int y)
    {
        //generate terrain color
        float xCoord = (float)x / width * scale + OffsetX;
        float yCoord = (float)y / height * scale + OffsetY;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        
        //ici, implémenter les conditions pour générer les rebords
        float up = Mathf.PerlinNoise(xCoord, ((float)y+1) / height * scale + OffsetY);
        float down = Mathf.PerlinNoise(xCoord, ((float)y-1) / height * scale + OffsetY);
        float right = Mathf.PerlinNoise(((float)x+1) / height * scale + OffsetX, yCoord);
        float left = Mathf.PerlinNoise(((float)x-1) / height * scale + OffsetX, yCoord);
        
        float topRight = Mathf.PerlinNoise(((float)x+1) / height * scale + OffsetX, ((float)y+1) / height * scale + OffsetY);
        float topLeft = Mathf.PerlinNoise(((float)x+1) / height * scale + OffsetX, ((float)y-1) / height * scale + OffsetY);
        float bottomRight = Mathf.PerlinNoise(((float)x - 1) / height * scale + OffsetX, ((float)y + 1) / height * scale + OffsetY);
        float bottomLeft = Mathf.PerlinNoise(((float)x - 1) / height * scale + OffsetX, ((float)y - 1) / height * scale + OffsetY);

        if (sample < 0.2f)
        {
            //on check les corners tah valo prime
            
            if (IsWater(right) && IsWater(up) && !IsWater(down) && !IsWater(left)) return I_TopRightTile;
            if (!IsWater(right) && IsWater(up) && !IsWater(down) && IsWater(left)) return I_TopLeftTile;
            if (IsWater(right) && !IsWater(up) && IsWater(down) && !IsWater(left)) return I_BottomRightTile;
            if (!IsWater(right) && !IsWater(up) && IsWater(down) && IsWater(left)) return I_BottomLeftTile;
            
            if (up < 0.2f && down >= 0.2f) return BottomEdgeTile;
            if (left < 0.2f && right >= 0.2f) return RightEdgeTile;
            if (left >= 0.2f && right < 0.2f) return LeftEdgeTile;
            if (up >= 0.2f && down < 0.2f) return TopEdgeTile;
            
            if (!IsWater(right) && !IsWater(up) && IsWater(topRight)) return O_TopRightTile;
            if (!IsWater(left) && !IsWater(up) && IsWater(topLeft)) return O_BottomRightTile;
            if (!IsWater(left) && !IsWater(down) && IsWater(bottomLeft)) return O_BottomLeftTile;
            if (!IsWater(right) && !IsWater(down) && IsWater(bottomRight)) return O_TopLeftTile;
        }
        
        
        if (sample < 0.2f) return waterTile;
        return grassTile;
    }

    bool IsWater(float sample)
    {
        if (sample >= 0.2f) return true;
        return false;
    }
}