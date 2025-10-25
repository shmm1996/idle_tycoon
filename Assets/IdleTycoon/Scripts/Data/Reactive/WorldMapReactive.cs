using IdleTycoon.Scripts.Data.Session;
using R3;

namespace IdleTycoon.Scripts.Data.Reactive
{
    public class WorldMapReactive
    {
        private readonly Subject<WorldMap.ReadOnly> _onWorldLoaded = new();
        private readonly Subject<Chunk8X8.ReadOnly> _onChunkLoaded = new();
        
        public Observable<WorldMap.ReadOnly> OnWorldLoaded => _onWorldLoaded;
        
        public Observable<Chunk8X8.ReadOnly> OnChunk8Loaded => _onChunkLoaded;
        
        public void WorldMapLoaded(WorldMap.ReadOnly worldMap) => _onWorldLoaded.OnNext(worldMap);
        
        public void ChunkLoaded(Chunk8X8.ReadOnly chunk) => _onChunkLoaded.OnNext(chunk);
        
        
    }
}