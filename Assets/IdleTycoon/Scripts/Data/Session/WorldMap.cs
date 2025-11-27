using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Session.TileEntities;
using IdleTycoon.Scripts.Utils;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public unsafe partial struct WorldMap
    {
        private readonly int2 _size;
        private readonly Chunk8X8* _chunks;
        
        //---Buildings---
        private FastPool<Building> _buildings;
        private NativeHashMap<int2, int> _tileToBuilding;
        
        public WorldMap(int2 size)
        {
            this._size = size;
            
            int chunksSize = size.x * size.y * sizeof(Chunk8X8);
            _chunks = (Chunk8X8*)UnsafeUtility.Malloc(chunksSize, 64, Allocator.Persistent);
            UnsafeUtility.MemClear(_chunks, chunksSize);
            for(int y = 0; y < this._size.y; y++)
            for(int x = 0; x < this._size.x; x++)
                _chunks[x + y * this._size.x] = new Chunk8X8(new int2(x, y));
            
            _buildings = new FastPool<Building>();
            _tileToBuilding = new NativeHashMap<int2, int>(_buildings.Capacity, Allocator.Persistent);
        }
        
        public void Dispose()
        {
            for (int x = 0; x < _size.x; x++)
            for (int y = 0; y < _size.y; y++)
                _chunks[x + y * _size.x].Dispose();
            
            UnsafeUtility.Free(_chunks, Allocator.Persistent);
            
            _buildings.Dispose();
            _tileToBuilding.Dispose();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chunk8X8* GetChunk(int2 chunk) => _chunks + (chunk.x + chunk.y * _size.x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Chunk8X8* GetTilesChunk(int2 tile) => GetChunk(Chunk8X8Utils.ToChunk(tile));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBuilding(Building building)
        {
            _tileToBuilding[building.tile] = _buildings.Add(building);
            //TODO:  Add chunk.tileAttributeFlag(position, (int)TileAttributeFlag.Building).
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveBuilding(int2 tile)
        {
            if (!_tileToBuilding.TryGetValue(tile, out int id)) return;
            
            _tileToBuilding.Remove(tile);
            _buildings.Remove(id);
            //TODO:  Add chunk.tileAttributeFlag(position, (int)TileAttributeFlag.Building).
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAttribute(int2 tile, int attribute) =>
            GetTilesChunk(tile)->IsExistTileAttributeFlag(Chunk8X8Utils.ToIndexFromGlobal(tile), attribute);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasTile(int2 tile) => tile.x >= 0 &&  tile.x < _size.x && tile.y >= 0 && tile.y < _size.y;
        
        public readonly struct ReadOnly
        {
            private readonly WorldMap* _worldMap;
            public readonly int2 size;

            public ReadOnly(WorldMap* worldMap)
            {
                _worldMap = worldMap;
                size = worldMap->_size;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk8X8.ReadOnly GetChunk(int2 chunk) => new(_worldMap->GetChunk(chunk));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk8X8.ReadOnly GetTilesChunk(int2 tile) => new(_worldMap->GetTilesChunk(tile));
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasTile(int2 tile) => _worldMap->HasTile(tile);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasAttribute(int2 tile, int attribute) => _worldMap->HasAttribute(tile, attribute);
        }
    }
}