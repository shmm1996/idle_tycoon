using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public sealed class SessionTileProvider
    {
        private readonly WorldMap.ReadOnly _worldMap;

        public int2 WorldMapSize => _worldMap.Size;
        
        public SessionTileProvider(WorldMap.ReadOnly worldMap)
        {
            _worldMap = worldMap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool OnWorldMap(int2 tile) =>
            tile.x >= 0 && tile.y >= 0 && tile.x < _worldMap.Size.x && tile.y < _worldMap.Size.y;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAttribute(int2 tile, TileAttributeFlag attribute) =>
            _worldMap
                .GetTilesChunk(tile)
                .IsExistTileAttributeFlag(Chunk8X8Utils.ToIndexFromGlobal(tile), (int)attribute);
    }
}