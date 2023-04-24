using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapWallPlacer : MonoBehaviour
{
    public enum SpecialType
    {
        None,
        StartTile,
        SoloTile
    }
    public enum Part
    {
        A, B, C, D, E
    }
    [System.Serializable]
    public struct TileData
    {
        public Sprite sprite;
        public Part part;
        public SpecialType special;
    }
    [SerializeField] TileData[] lowerTiles;
    [SerializeField] TileData[] upperTiles;
    Tile tile;
    private void Start() {
        tile = new Tile();
        //tile.sprite = 
    }
}
