using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public unsafe partial struct WorldMap
    {
        private const int AttributesMaskIsGround = (int)TileAttributeBit.IsGround;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTerrainGround(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            chunk->AddTileAttributeFlag(index, (int)TileAttributeBitPosition.IsGround);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetTerrainGround(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            bool isGround = chunk->IsExistTileAttributeFlag(index, AttributesMaskIsGround);
            if (isGround) return false;

            chunk->AddTileAttributeFlag(index, (int)TileAttributeBitPosition.IsGround);

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetTerrainGround(int2 tile, out ulong oldFlags)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            oldFlags = chunk->GetTileAttributeFlags(index);
            bool isGround = (oldFlags & AttributesMaskIsGround) != 0;
            if (isGround) return false;

            chunk->AddTileAttributeFlag(index, (int)TileAttributeBitPosition.IsGround);

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveTerrainGround(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            chunk->ClearTileAttributeFlag(index, (int)TileAttributeBitPosition.IsGround);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveTerrainGround(int2 tile)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            bool isGround = chunk->IsExistTileAttributeFlag(index, AttributesMaskIsGround);
            if (!isGround) return false;

            chunk->ClearTileAttributeFlag(index, (int)TileAttributeBitPosition.IsGround);

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveTerrainGround(int2 tile, out ulong oldFlags)
        {
            Chunk8X8* chunk = GetTilesChunk(tile);
            int index = Chunk8X8Utils.ToIndexFromGlobal(tile);
            oldFlags = chunk->GetTileAttributeFlags(index);
            bool isGround = (oldFlags & AttributesMaskIsGround) != 0;
            if (!isGround) return false;

            chunk->ClearTileAttributeFlag(index, (int)TileAttributeBitPosition.IsGround);

            return true;
        }
    }
}