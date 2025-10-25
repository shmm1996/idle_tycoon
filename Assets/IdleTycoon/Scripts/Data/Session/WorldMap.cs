using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Session.TileEntities;
using IdleTycoon.Scripts.Utils;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public unsafe struct WorldMap
    {
        public readonly int2 Size;
        
        private Chunk8X8* _chunks;
        
        //---Buildings---
        private FastPool<Building> _buildings;
        private NativeHashMap<int2, int> _tileToBuilding;
        
        public WorldMap(int2 size)
        {
            Size = size;
            
            int chunksSize = size.x * size.y * sizeof(Chunk8X8);
            _chunks = (Chunk8X8*)UnsafeUtility.Malloc(chunksSize, 64, Allocator.Persistent);
            //UnsafeUtility.MemClear(_chunks, chunksSize);
            for(int y = 0; y < Size.y; y++)
                for(int x = 0; x < Size.x; x++)
                    _chunks[x + y * Size.x] = new Chunk8X8(new int2(x, y));
            
            _buildings = new FastPool<Building>();
            _tileToBuilding = new NativeHashMap<int2, int>(_buildings.Capacity, Allocator.Persistent);
        }
        
        public void Dispose()
        {
            for(int y = 0; y < Size.y; y++)
                for(int x = 0; x < Size.x; x++)
                    _chunks[x + y * Size.x].Dispose();
            
            UnsafeUtility.Free(_chunks, Allocator.Persistent);
            
            _buildings.Dispose();
            _tileToBuilding.Dispose();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnly AsReadOnly() => new ReadOnly(this);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Chunk8X8 GetChunk(int2 chunk) => ref _chunks[Size.x * chunk.y + chunk.x];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddBuilding(Building building)
        {
            _tileToBuilding[building.Position] = _buildings.Add(building);
            //TODO:  Add chunk.tileAttributeFlag(position, (int)TileAttributeFlag.Building).
        }
        
        public struct ReadOnly
        {
            private readonly WorldMap _worldMap;
            public readonly int2 Size;

            public ReadOnly(WorldMap worldMap)
            {
                _worldMap = worldMap;
                Size = worldMap.Size;
            }
        }
    }
}