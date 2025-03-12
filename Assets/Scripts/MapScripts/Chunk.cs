using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapScripts
{
    public class Chunk
    {
        public Vector2Int position;
        public GameObject chunkObject;
        public TileData[,] tiles;

        public Chunk(Vector2Int pos, int size)
        {
            position = pos;
            tiles = new TileData[size, size];
        }
    }

}