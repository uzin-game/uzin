using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public static int width = 100;  // Largeur de la grille
    public static int height = 100; // Hauteur de la grille
    public static float scale = 5f;
    public static float OffsetX = 100f;
    public static float OffsetY = 100f;
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
    private GameObject weirdhybrid_bottom;
    private GameObject weirdhybrid_top;
    private GameObject weirdhybrid_left;
    private GameObject weirdhybrid_right;
    

    //private float tileSize = 32/100f;

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
                    Vector3 position = new Vector3(x, y, 0);
                    GameObject newTile = Instantiate(tile, position, Quaternion.identity);
                    AdjustTileSize(newTile);
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
        weirdhybrid_left = Resources.Load("Prefabs/weirdhybrid_left") as GameObject;
        weirdhybrid_bottom = Resources.Load("Prefabs/weirdhybrid_bottom") as GameObject;
        weirdhybrid_right = Resources.Load("Prefabs/weirdhybrid_right") as GameObject;
        weirdhybrid_top = Resources.Load("Prefabs/weirdhybrid_top") as GameObject;
    }

    public GameObject GenerateTile(int x, int y)
    {
        //generate terrain color
        float xCoord = (float)x / width * scale + OffsetX;
        float yCoord = (float)y / height * scale + OffsetY;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        //ici, implémenter les conditions pour générer les rebords
        float up = Mathf.PerlinNoise(xCoord, ((float)y + 1) / height * scale + OffsetY);
        float down = Mathf.PerlinNoise(xCoord, ((float)y - 1) / height * scale + OffsetY);
        float right = Mathf.PerlinNoise(((float)x + 1) / height * scale + OffsetX, yCoord);
        float left = Mathf.PerlinNoise(((float)x - 1) / height * scale + OffsetX, yCoord);

        float topRight = Mathf.PerlinNoise(((float)x + 1) / height * scale + OffsetX,
            ((float)y + 1) / height * scale + OffsetY);
        float topLeft = Mathf.PerlinNoise(((float)x + 1) / height * scale + OffsetX,
            ((float)y - 1) / height * scale + OffsetY);
        float bottomRight = Mathf.PerlinNoise(((float)x - 1) / height * scale + OffsetX,
            ((float)y + 1) / height * scale + OffsetY);
        float bottomLeft = Mathf.PerlinNoise(((float)x - 1) / height * scale + OffsetX,
            ((float)y - 1) / height * scale + OffsetY);

        if (sample < 0.2f)
        {
            // On check les corners tah valo prime
            

            if (!IsWater(right) && !IsWater(up) && IsWater(down) && IsWater(left)) return I_TopRightTile;
            if (IsWater(right) && !IsWater(up) && IsWater(down) && !IsWater(left)) return I_TopLeftTile;
            if (!IsWater(right) && IsWater(up) && !IsWater(down) && IsWater(left)) return I_BottomRightTile;
            if (IsWater(right) && IsWater(up) && !IsWater(down) && !IsWater(left)) return I_BottomLeftTile;
            
            if (IsWater(right) && !IsWater(left) && !IsWater(up) && !IsWater(down)) return weirdhybrid_left;
            if (!IsWater(right) && IsWater(left) && !IsWater(up) && !IsWater(down)) return weirdhybrid_right;
            if (!IsWater(right) && !IsWater(left) && IsWater(up) && !IsWater(down)) return weirdhybrid_bottom;
            if (!IsWater(right) && !IsWater(left) && !IsWater(up) && IsWater(down)) return weirdhybrid_top;
            
            if (IsWater(up) && !IsWater(down)) return BottomEdgeTile;
            if (IsWater(left) && !IsWater(right)) return RightEdgeTile;
            if (!IsWater(left) && IsWater(right)) return LeftEdgeTile;
            if (!IsWater(up) && IsWater(down)) return TopEdgeTile;
            
            if (IsWater(right) && IsWater(up) && !IsWater(topRight)) return O_TopRightTile;
            if (IsWater(left) && IsWater(up) && !IsWater(topLeft)) return O_BottomRightTile;
            if (IsWater(left) && IsWater(down) && !IsWater(bottomLeft)) return O_BottomLeftTile;
            if (IsWater(right) && IsWater(down) && !IsWater(bottomRight)) return O_TopLeftTile;
        }

        if (sample < 0.2f) return waterTile;

        return grassTile;

        bool IsWater(float sample)
        {
            // Corrected logic
            return sample < 0.2f;
        }
        
        
    }

    public static bool IsValidTile(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        float xCoord = (float)x / width * scale + OffsetX;
        float yCoord = (float)y / height * scale + OffsetY;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        //if (sample < 0.2f) return false;
        return true;
    }
    
    void AdjustTileSize(GameObject tile)
    {
        // Récupère le SpriteRenderer pour calculer la taille actuelle
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Taille actuelle de la tuile
            Vector2 currentSize = spriteRenderer.bounds.size;

            // Facteur d'échelle à appliquer pour atteindre une taille de 1 unité
            Vector3 scaleAdjustment = new Vector3(1 / currentSize.x, 1 / currentSize.y, 1);

            // Applique l'échelle au transform
            tile.transform.localScale = scaleAdjustment;
        }
    }

}