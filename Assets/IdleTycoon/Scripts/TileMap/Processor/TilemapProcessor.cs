using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.TileMap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using R3;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.TileMap.Processor
{
    public sealed class TilemapProcessor : IDisposable
    {
        private readonly TilemapSubProcessor<TileTerrainDefinition>[] _subProcessors;
        private readonly CompositeDisposable _disposables;

        private readonly HashQueue<int2> _toResolve;

        public TilemapProcessor(GameSession.Context context, params TilemapSubProcessor<TileTerrainDefinition>[] subProcessors)
        {
            _subProcessors = subProcessors;
            _disposables = new CompositeDisposable();

            _toResolve = new HashQueue<int2>();
            
            context.observables.OnTilesUpdated.Subscribe(OnTilesUpdated).AddTo(_disposables);
            context.observables.OnTilesCleaned.Subscribe(OnTilesCleaned).AddTo(_disposables);
            context.observables.OnWorldLoaded.Subscribe(OnWorldMapLoaded).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        private void OnTilesUpdated(int2[] tiles)
        {
            foreach (int2 tile in tiles)
                _toResolve.Enqueue(tile);
        }

        private void OnTilesCleaned(int2[] tiles)
        {

        }

        private void OnWorldMapLoaded(WorldMap.ReadOnly worldMap)
        {
            for (int x = 0; x < worldMap.Size.x; x++)
            for (int y = 0; y < worldMap.Size.y; y++)
                _toResolve.Enqueue(new int2(x, y));
        }

        public int UpdateTiles(int limit)
        {
            if (_subProcessors.Length == 0) return _toResolve.Count;

            HashSet<int2> processed = new(limit);
            for (int i = 0; i < limit && _toResolve.Count > 0; i++)
            {
                if (!_toResolve.TryDequeue(out int2 tile)) break;
                foreach (TilemapSubProcessor<TileTerrainDefinition> subProcessor in _subProcessors)
                {
                    if (!subProcessor.TryLazyResolveTile(tile)) continue;
                    processed.Add(tile);
                    foreach (int2 affected in subProcessor.GetAffectedTiles(tile))
                        if (!processed.Contains(affected))
                            _toResolve.Enqueue(affected);
                }
            }

            foreach (TilemapSubProcessor<TileTerrainDefinition> subProcessor in _subProcessors)
                subProcessor.Apply();
            
            return _toResolve.Count;
        }
    }
}