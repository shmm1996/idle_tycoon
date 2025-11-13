using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.TileMap.Processor
{
    public sealed class SessionTileProvider
    {
        private readonly WorldMap.ReadOnly _worldMap;

        public int2 WorldMapSize => _worldMap.Size;
        
        public SessionTileProvider(WorldMap.ReadOnly worldMap)
        {
            _worldMap = worldMap;
        }
        
        public bool HasAttribute(int2 tile, TileAttributeFlag attribute)
        {
            Chunk8X8.ReadOnly chunk = _worldMap.GetChunkAsReadOnly(Chunk8X8Utils.ToChunk(tile));
            bool hasAttribute = chunk.IsExistTileAttributeFlag(Chunk8X8Utils.ToIndexFromGlobal(tile), (int)attribute);

            return hasAttribute;
        }
    }
}