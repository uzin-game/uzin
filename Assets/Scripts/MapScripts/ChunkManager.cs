using System.Collections.Generic;
using Unity.Netcode;
//using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapScripts
{
    public class ChunkManager : NetworkBehaviour
    {
        private int chunkSize = 16;
        private int renderDistance = 2;
        private float noiseScale = 0.1f;
        private Vector2Int lastPlayerChunkPos;
        
        public NetworkVariable<float> mapSeedX = new NetworkVariable<float>();
        public NetworkVariable<float> mapSeedY = new NetworkVariable<float>();
        
        
        public static int width = 100;  // Largeur de la grille
        public static int height = 100; // Hauteur de la grille
        public static float scale = 5f;
        public static float OffsetX;
        public static float OffsetY;
        private GameObject[,] grid; // Tableau 2D pour stocker les tiles placés
        
        
        private Tile waterTile;
        private Tile grassTile;
        private Tile BottomEdgeTile;
        private Tile I_BottomLeftTile;
        private Tile I_BottomRightTile;
        private Tile I_TopLeftTile;
        private Tile I_TopRightTile;
        private Tile LeftEdgeTile;
        private Tile O_BottomLeftTile;
        private Tile O_BottomRightTile;
        private Tile O_TopLeftTile;
        private Tile O_TopRightTile;
        private Tile RightEdgeTile;
        private Tile TopEdgeTile;
        private Tile weirdhybrid_bottom;
        private Tile weirdhybrid_top;
        private Tile weirdhybrid_left;
        private Tile weirdhybrid_right;
        
        public Tilemap tilemap;
        public TileBase[] tiles; // Assign different tiles in the inspector

        private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
        public Transform player;

        private void Start()
        {
            if (IsHost || IsServer)
            {
                mapSeedX.Value = (Random.Range(-1000000f, 1000000f));
                mapSeedY.Value = (Random.Range(-1000000f, 1000000f));
            }
            
            OffsetX = Random.Range(-1000000f, 1000000f);
            OffsetY = Random.Range(-1000000f, 1000000f);
            
            //essai avec une seed convenant a un serveur
            //OffsetX = seed.Item1;
            //OffsetY = seed.Item2; 
            
            LoadTiles();
            
            Vector2Int playerChunkPos = GetChunkCoords(player.position);
            
            lastPlayerChunkPos = GetChunkCoords(player.position);

            // Charger les chunks autour du joueur au démarrage
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(playerChunkPos.x + x, playerChunkPos.y + y);

                    if (!loadedChunks.ContainsKey(chunkPos))
                    {
                        LoadChunk(chunkPos);
                    }
                }
            }
        }
        
        void Update()
        {
            // Get the current player chunk position
            Vector2Int currentChunkPos = GetChunkCoords(player.position);

            // Only update if the player moves to a new chunk
            if (currentChunkPos != lastPlayerChunkPos)
            {
                lastPlayerChunkPos = currentChunkPos;
                UpdateChunksAroundPlayer();
            }
        }
        
        void UpdateChunksAroundPlayer()
        {
            // Load new chunks around the player
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    Vector2Int chunkPos = new Vector2Int(lastPlayerChunkPos.x + x, lastPlayerChunkPos.y + y);

                    if (!loadedChunks.ContainsKey(chunkPos))
                    {
                        LoadChunk(chunkPos);
                    }
                }
            }

            // Unload chunks that are too far
            List<Vector2Int> chunksToRemove = new List<Vector2Int>();

            foreach (var chunk in loadedChunks.Keys)
            {
                if (Vector2Int.Distance(chunk, lastPlayerChunkPos) > renderDistance + 1)
                {
                    chunksToRemove.Add(chunk);
                }
            }

            foreach (var chunkPos in chunksToRemove)
            {
                UnloadChunk(chunkPos);
            }
        }




        Vector2Int GetChunkCoords(Vector3 worldPos)
        {
            return new Vector2Int(Mathf.FloorToInt(worldPos.x / chunkSize), Mathf.FloorToInt(worldPos.y / chunkSize));
        }
    
        void LoadChunk(Vector2Int chunkPos)
        {
            Chunk newChunk = new Chunk(chunkPos, chunkSize);
            GenerateChunk(newChunk);
            RenderChunk(newChunk);
            loadedChunks.Add(chunkPos, newChunk);
        }
    
        void UnloadChunk(Vector2Int chunkPos)
        {
            loadedChunks.Remove(chunkPos);
            ClearChunkFromTilemap(chunkPos);
        }
    
        void GenerateChunk(Chunk chunk)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    // Apply a random offset to remove symmetry & change map every time
                    float worldX = (((chunk.position.x * chunkSize) + x) * noiseScale) + OffsetX;
                    float worldY = (((chunk.position.y * chunkSize) + y) * noiseScale) + OffsetY;
                    float noiseValue = Mathf.PerlinNoise(worldX, worldY);

                    TileBase selectedTile = GenerateTile(x, y, chunk, noiseValue); // Adjust tile selection
                    //TileBase newTile = GenerateTile(x, y);
    
                    chunk.tiles[x, y] = new TileData(noiseValue, selectedTile);
                }
            }
        }
        
        public TileBase GenerateTile(int x, int y, Chunk chunk, float sample)
        {
            //generate terrain color
            float xCoord = (((chunk.position.x * chunkSize) + x) * noiseScale) + OffsetX;
            float yCoord = (((chunk.position.y * chunkSize) + y) * noiseScale) + OffsetY;
            //float sample = Mathf.PerlinNoise(xCoord, yCoord);
    
            //ici, implémenter les conditions pour générer les rebords
            float up = Mathf.PerlinNoise(xCoord, (((chunk.position.y * chunkSize) + y+1) * noiseScale) + OffsetY);
            float down = Mathf.PerlinNoise(xCoord, (((chunk.position.y * chunkSize) + y-1) * noiseScale) + OffsetY);
            float right = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x+1) * noiseScale) + OffsetX, yCoord);
            float left = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x-1) * noiseScale) + OffsetX, yCoord);
    
            float topRight = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x+1) * noiseScale) + OffsetX, (((chunk.position.y * chunkSize) + y+1) * noiseScale) + OffsetY);
            float topLeft = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x+1) * noiseScale) + OffsetX, (((chunk.position.y * chunkSize) + y-1) * noiseScale) + OffsetY);
            float bottomRight = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x-1) * noiseScale) + OffsetX, (((chunk.position.y * chunkSize) + y+1) * noiseScale) + OffsetY);
            float bottomLeft = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x-1) * noiseScale) + OffsetX, (((chunk.position.y * chunkSize) + y-1) * noiseScale) + OffsetY);
    
            if (sample < 0.2f)
            {
                // On check les corners tah valo prime
                
    
                if (!IsWater(right) && !IsWater(up) && IsWater(down) && IsWater(left)) return tiles[2];
                if (IsWater(right) && !IsWater(up) && IsWater(down) && !IsWater(left)) return tiles[3];
                if (!IsWater(right) && IsWater(up) && !IsWater(down) && IsWater(left)) return tiles[4];
                if (IsWater(right) && IsWater(up) && !IsWater(down) && !IsWater(left)) return tiles[5];
                
                if (IsWater(right) && !IsWater(left) && !IsWater(up) && !IsWater(down)) return tiles[6];
                if (!IsWater(right) && IsWater(left) && !IsWater(up) && !IsWater(down)) return tiles[7];
                if (!IsWater(right) && !IsWater(left) && IsWater(up) && !IsWater(down)) return tiles[8];
                if (!IsWater(right) && !IsWater(left) && !IsWater(up) && IsWater(down)) return tiles[9];
                
                if (IsWater(up) && !IsWater(down)) return tiles[10];
                if (IsWater(left) && !IsWater(right)) return tiles[11];
                if (!IsWater(left) && IsWater(right)) return tiles[12];
                if (!IsWater(up) && IsWater(down)) return tiles[13];
                
                if (IsWater(right) && IsWater(up) && !IsWater(topRight)) return tiles[14];
                if (IsWater(left) && IsWater(up) && !IsWater(topLeft)) return tiles[15];
                if (IsWater(left) && IsWater(down) && !IsWater(bottomLeft)) return tiles[16];
                if (IsWater(right) && IsWater(down) && !IsWater(bottomRight)) return tiles[17];
            }
    
            if (sample < 0.2f) return tiles[0];
    
            return tiles[1];
    
            bool IsWater(float sample)
            {
                // Corrected logic
                return sample < 0.2f;
            }
            
            
        }
        
        void LoadTiles()
        {
            waterTile = Resources.Load("Prefabs/sol_eau_0") as Tile;
            grassTile = Resources.Load("Prefabs/sol_basique_0") as Tile;
            BottomEdgeTile = Resources.Load("Prefabs/BottomEdge") as Tile;
            I_BottomLeftTile = Resources.Load("Prefabs/I_BottomLeft") as Tile;
            I_BottomRightTile = Resources.Load("Prefabs/I_BottomRight") as Tile;
            I_TopLeftTile = Resources.Load("Prefabs/I_TopLeft") as Tile;
            I_TopRightTile = Resources.Load("Prefabs/I_TopRight") as Tile;
            LeftEdgeTile = Resources.Load("Prefabs/LeftEdge") as Tile;
            O_BottomLeftTile = Resources.Load("Prefabs/O_BottomLeft") as Tile;
            O_BottomRightTile = Resources.Load("Prefabs/O_BottomRight") as Tile;
            O_TopLeftTile = Resources.Load("Prefabs/O_TopLeft") as Tile;
            O_TopRightTile = Resources.Load("Prefabs/O_TopRight") as Tile;
            RightEdgeTile = Resources.Load("Prefabs/RightEdge") as Tile;
            TopEdgeTile = Resources.Load("Prefabs/TopEdge") as Tile;
            weirdhybrid_left = Resources.Load("Prefabs/weirdhybrid_left") as Tile;
            weirdhybrid_bottom = Resources.Load("Prefabs/weirdhybrid_bottom") as Tile;
            weirdhybrid_right = Resources.Load("Prefabs/weirdhybrid_right") as Tile;
            weirdhybrid_top = Resources.Load("Prefabs/weirdhybrid_top") as Tile;
        }
    
        void RenderChunk(Chunk chunk)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector3Int tilePos = new Vector3Int(chunk.position.x * chunkSize + x, chunk.position.y * chunkSize + y, 0);

                    tilemap.SetTile(tilePos, chunk.tiles[x, y].Tilebase);
                }
            }
        }
    
        void ClearChunkFromTilemap(Vector2Int chunkPos)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + chunkPos.x * chunkSize, y + chunkPos.y * chunkSize, 0);
                    tilemap.SetTile(tilePos, null);
                }
            }
        }
        
        
    }
    
}