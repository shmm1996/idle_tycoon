using System;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public interface ITilemapSubProcessor
    {
        bool TryLazyResolveTile(int2 tile);

        ReadOnlySpan<int2> GetAffectedTiles(int2 tile);
        
        void Apply();
    }
}