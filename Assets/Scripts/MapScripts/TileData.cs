using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapScripts
{
    public class TileData
    {
        public float Height;
        public TileBase Tilebase;

        public TileData(float height, TileBase tilebase)
        {
            Height = height;
            Tilebase = tilebase;
        }
    }
}