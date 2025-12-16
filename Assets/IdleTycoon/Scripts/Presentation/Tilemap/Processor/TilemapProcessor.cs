using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Utils;
using R3;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public sealed class TilemapProcessor : IDisposable
    {
        private readonly ITilemapSubProcessor[] _processors;
        private readonly CompositeDisposable _disposables;

        private readonly HashQueue<int2> _toResolve = new();

        public TilemapProcessor(GameSession.Context context, params ITilemapSubProcessor[] processors)
        {
            _processors = processors;
            _disposables = new CompositeDisposable();
            
            context.Observables.OnTilesUpdated.Subscribe(OnTilesUpdated).AddTo(_disposables);
            context.Observables.OnTilesCleaned.Subscribe(OnTilesCleaned).AddTo(_disposables);
            context.Observables.OnWorldLoaded.Subscribe(OnWorldMapLoaded).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        private void OnTilesUpdated(int2[] tiles) => _toResolve.EnqueueRange(tiles.AsSpan());

        private void OnTilesCleaned(int2[] tiles)
        {

        }

        private void OnWorldMapLoaded(WorldMap.ReadOnly worldMap)
        {
            for (int y = 0; y < worldMap.size.y; y++)
            for (int x = 0; x < worldMap.size.x; x++)
                _toResolve.Enqueue(new int2(x, y));
        }

        public int UpdateTiles(int limit)
        {
            if (_processors.Length == 0) return _toResolve.Count;

            HashSet<int2> processed = new(limit);
            for (int i = 0; i < limit && _toResolve.Count > 0;)
            {
                if (!_toResolve.TryDequeue(out int2 tile)) break;
                processed.Add(tile);
                foreach (ITilemapSubProcessor processor in _processors)
                {
                    i++;
                    if (!processor.TryLazyResolveTile(tile)) continue;
                    foreach (int2 affected in processor.GetAffectedTiles(tile))
                        if (!processed.Contains(affected)) 
                            _toResolve.Enqueue(affected);
                }
            }

            foreach (ITilemapSubProcessor processor in _processors)
                processor.Apply();
            
            return _toResolve.Count;
        }
    }
}