using System;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles
{
    [CreateAssetMenu(fileName = "TileTerrain", menuName = "Definitions/TileMap/Tiles/Terrain", order = 0)]
    public class TileTerrainDefinition : TileDefinition<TileTerrainData>
    {
        
    }
    
    [Serializable]
    public struct TileTerrainData
    {
        public bool isGround;
    }
}