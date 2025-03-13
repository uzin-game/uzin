using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace MapScripts
{
    public class ChunkManager : NetworkBehaviour
    {
        private int chunkSize = 16;
        private int renderDistance = 2;
        private float noiseScale = 0.1f;
        private Vector2Int lastPlayerChunkPos;

        // Les variables width et height ne sont plus utilisées pour un monde infini.
        // public static int width = 100;
        // public static int height = 100;

        // NetworkVariables pour les seeds de génération
        [SerializeField] public NetworkVariable<float> SeedX;
        [SerializeField] public NetworkVariable<float> SeedY;

        // Déclarations des tuiles
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
        public TileBase[] tiles; // Assignez les différentes tuiles dans l'inspecteur

        // Dictionnaire pour stocker les chunks chargés
        private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();

        // Référence au joueur pour déterminer sa position
        public Transform player;

        public override void OnNetworkSpawn()
        {
            // Générer la seed uniquement côté serveur ou host
            if (IsServer || IsHost)
            {
                if (SeedX.Value == 0 && SeedY.Value == 0)
                {
                    SeedX.Value = Random.Range(-1000000f, 1000000f);
                    SeedY.Value = Random.Range(-1000000f, 1000000f);
                }

                Debug.Log($"(Server/Host) Seeds générées: {SeedX.Value}, {SeedY.Value}");
            }
            else
            {
                Debug.Log($"(Client) Seeds reçues: {SeedX.Value}, {SeedY.Value}");
            }

            // Déterminer le chunk dans lequel se trouve le joueur et charger les chunks autour
            Vector2Int playerChunkPos = GetChunkCoords(player.position);
            lastPlayerChunkPos = playerChunkPos;

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

        private void Start()
        {
            // Charger les tuiles depuis les ressources
            LoadTiles();
        }

        private void Update()
        {
            // Mettre à jour dynamiquement les chunks en fonction du déplacement du joueur
            Vector2Int currentChunkPos = GetChunkCoords(player.position);

            if (currentChunkPos != lastPlayerChunkPos)
            {
                lastPlayerChunkPos = currentChunkPos;
                UpdateChunksAroundPlayer();
            }
        }

        void UpdateChunksAroundPlayer()
        {
            // Charger de nouveaux chunks autour du joueur
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

            // Décharger les chunks qui sont trop éloignés pour économiser la mémoire
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

        public TileBase GetTileAtCell(Vector3Int cellPos)
        {
            return tilemap.GetTile(cellPos);
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
                    // Calculer les coordonnées mondiales en fonction du chunk et de la seed
                    float worldX = (((chunk.position.x * chunkSize) + x) * noiseScale) + SeedX.Value;
                    float worldY = (((chunk.position.y * chunkSize) + y) * noiseScale) + SeedY.Value;
                    float noiseValue = Mathf.PerlinNoise(worldX, worldY);

                    TileBase selectedTile = GenerateTile(x, y, chunk, noiseValue);
                    chunk.tiles[x, y] = new TileData(noiseValue, selectedTile);
                }
            }
        }

        public TileBase GenerateTile(int x, int y, Chunk chunk, float sample)
        {
            // Calculer les coordonnées mondiales de la tuile
            float xCoord = (((chunk.position.x * chunkSize) + x) * noiseScale) + SeedX.Value;
            float yCoord = (((chunk.position.y * chunkSize) + y) * noiseScale) + SeedY.Value;

            // Calculer les valeurs de bruit pour les voisins
            float up = Mathf.PerlinNoise(xCoord, (((chunk.position.y * chunkSize) + y + 1) * noiseScale) + SeedY.Value);
            float down = Mathf.PerlinNoise(xCoord,
                (((chunk.position.y * chunkSize) + y - 1) * noiseScale) + SeedY.Value);
            float right = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x + 1) * noiseScale) + SeedX.Value,
                yCoord);
            float left = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x - 1) * noiseScale) + SeedX.Value,
                yCoord);

            float topRight = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x + 1) * noiseScale) + SeedX.Value,
                (((chunk.position.y * chunkSize) + y + 1) * noiseScale) + SeedY.Value);
            float topLeft = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x - 1) * noiseScale) + SeedX.Value,
                (((chunk.position.y * chunkSize) + y + 1) * noiseScale) + SeedY.Value);
            float bottomRight = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x + 1) * noiseScale) + SeedX.Value,
                (((chunk.position.y * chunkSize) + y - 1) * noiseScale) + SeedY.Value);
            float bottomLeft = Mathf.PerlinNoise((((chunk.position.x * chunkSize) + x - 1) * noiseScale) + SeedX.Value,
                (((chunk.position.y * chunkSize) + y - 1) * noiseScale) + SeedY.Value);

            // Déterminer le type de tuile selon la valeur de bruit (sample)
            if (sample < 0.2f)
            {
                if (!IsWater(right) && !IsWater(up) && IsWater(down) && IsWater(left))
                    return tiles[2];
                if (IsWater(right) && !IsWater(up) && IsWater(down) && !IsWater(left))
                    return tiles[3];
                if (!IsWater(right) && IsWater(up) && !IsWater(down) && IsWater(left))
                    return tiles[4];
                if (IsWater(right) && IsWater(up) && !IsWater(down) && !IsWater(left))
                    return tiles[5];

                if (IsWater(right) && !IsWater(left) && !IsWater(up) && !IsWater(down))
                    return tiles[6];
                if (!IsWater(right) && IsWater(left) && !IsWater(up) && !IsWater(down))
                    return tiles[7];
                if (!IsWater(right) && !IsWater(left) && IsWater(up) && !IsWater(down))
                    return tiles[8];
                if (!IsWater(right) && !IsWater(left) && !IsWater(up) && IsWater(down))
                    return tiles[9];

                if (IsWater(up) && !IsWater(down))
                    return tiles[10];
                if (IsWater(left) && !IsWater(right))
                    return tiles[11];
                if (!IsWater(left) && IsWater(right))
                    return tiles[12];
                if (!IsWater(up) && IsWater(down))
                    return tiles[13];

                if (IsWater(left) && IsWater(down) && !IsWater(bottomLeft))
                    return tiles[14];
                if (IsWater(right) && IsWater(down) && !IsWater(bottomRight))
                    return tiles[15];
                if (IsWater(right) && IsWater(up) && !IsWater(topRight))
                    return tiles[16];
                if (IsWater(left) && IsWater(up) && !IsWater(topLeft))
                    return tiles[17];
            }

            if (sample < 0.2f)
                return tiles[0];
            return tiles[1];

            // Fonction locale pour vérifier si une valeur correspond à de l'eau
            bool IsWater(float val)
            {
                return val < 0.2f;
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
                    Vector3Int tilePos = new Vector3Int(chunk.position.x * chunkSize + x,
                        chunk.position.y * chunkSize + y, 0);
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