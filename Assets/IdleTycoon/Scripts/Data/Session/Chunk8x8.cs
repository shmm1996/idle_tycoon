using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public readonly unsafe struct Chunk8X8
    {
        private const int TileFlagsCount = 4;
        
        public readonly int2 position;
        private readonly ulong* _tileAttributeBitFlags; //[1] - ground ... n - building, k - road ... 
        //TODO: [0] not uses, cause enum TileAttributeFlag uses as bit flag. 
        //public chunk_biome
        //public chunk_temperature
        
        public Chunk8X8(int2 position)
        {
            this.position = position;
            
            int tileFlagsSize = sizeof(ulong) * TileFlagsCount;
            _tileAttributeBitFlags = (ulong*)UnsafeUtility.Malloc(tileFlagsSize, 64, Allocator.Persistent);
            UnsafeUtility.MemClear(_tileAttributeBitFlags, tileFlagsSize);
        }

        public void Dispose()
        {
            UnsafeUtility.Free(_tileAttributeBitFlags, Allocator.Persistent);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong GetTileAttributeFlag(int tile, int attribute) => _tileAttributeBitFlags[attribute] & (1UL << tile);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong GetTileAttributeFlags(int tile)
        {
            ulong flags = 0;
            ulong tileOffset = 1UL << tile;
            if ((_tileAttributeBitFlags[0] & tileOffset) != 0) flags |= 1UL;
            //if ((_tileAttributeBitFlags[1] & tileOffset) != 0) flags |= 1UL << 1;
            //if ((_tileAttributeBitFlags[2] & tileOffset) != 0) flags |= 1UL << 2;
            if ((_tileAttributeBitFlags[3] & tileOffset) != 0) flags |= 1UL << 3;

            return flags;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExistTileAttributeFlag(int tile, int attribute) => GetTileAttributeFlag(tile, attribute) != 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTileAttributeFlag(int tile, int attribute) => _tileAttributeBitFlags[attribute] |= 1UL << tile;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearTileAttributeFlag(int tile, int attribute) => _tileAttributeBitFlags[attribute] &= ~(1UL << tile);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong GetChunkTileAttributeFlag(int attribute) => _tileAttributeBitFlags[attribute];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsChunkExistTileAttributeFlag(int attribute) => GetChunkTileAttributeFlag(attribute) != 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetChunkTileAttributeFlag(int attribute, ulong value) => _tileAttributeBitFlags[attribute] = value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearChunkTileAttributeFlag(int attribute) => SetChunkTileAttributeFlag(attribute, 0);
        
        public readonly struct ReadOnly
        {
            private readonly Chunk8X8* _chunk;

            public ReadOnly(Chunk8X8* chunk) => _chunk = chunk;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsExistTileAttributeFlag(int tile, int attribute) => _chunk->IsExistTileAttributeFlag(tile, attribute);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ulong GetTileAttributeFlags(int tile) => _chunk->GetTileAttributeFlags(tile);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsChunkExistTileAttributeFlag(int attribute) => _chunk->IsChunkExistTileAttributeFlag(attribute);
        }
    }
}