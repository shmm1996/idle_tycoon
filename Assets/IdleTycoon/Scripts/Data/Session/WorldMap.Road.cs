using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public unsafe partial struct WorldMap
    {
        private const int AttributesMaskIsRoad = (int)TileAttributeBit.IsRoad;
        private const int AttributesMaskGroundWithRoad = AttributesMaskIsGround | AttributesMaskIsRoad;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRoad(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            chunk->AddTileAttributeFlag(index, (int)TileAttributeBitPosition.IsRoad);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetRoad(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            ulong flags = chunk->GetTileAttributeFlags(index);
            if ((flags & AttributesMaskGroundWithRoad) != AttributesMaskIsGround) return false; //IsGround but IsNotRoad.

            chunk->AddTileAttributeFlag(index, (int)TileAttributeBitPosition.IsRoad);

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetRoad(int2 tile, out ulong oldFlags)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            oldFlags = chunk->GetTileAttributeFlags(index);
            if ((oldFlags & AttributesMaskGroundWithRoad) != AttributesMaskIsGround) return false;

            chunk->AddTileAttributeFlag(index, (int)TileAttributeBitPosition.IsRoad);

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRoad(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            chunk->ClearTileAttributeFlag(index, (int)TileAttributeBitPosition.IsRoad);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveRoad(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            bool isRoad = chunk->IsExistTileAttributeFlag(index, AttributesMaskIsRoad);
            if (!isRoad) return false;

            chunk->ClearTileAttributeFlag(index, (int)TileAttributeBitPosition.IsRoad);

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveRoad(int2 tile, out ulong oldFlags)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            oldFlags = chunk->GetTileAttributeFlags(index);
            bool isRoad = (oldFlags & AttributesMaskIsRoad) != 0;
            if (!isRoad) return false;

            chunk->ClearTileAttributeFlag(index, (int)TileAttributeBitPosition.IsRoad);

            return true;
        }
    }
}