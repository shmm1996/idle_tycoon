using System.Collections.Generic;
using IdleTycoon.Scripts.TileMap.Definitions.Tiles;
using IdleTycoon.Scripts.TileMap.Processor;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.TileMap.Definitions.Rules
{
    public abstract class TilemapRuleDefinition<TTarget> : ScriptableObject
        where TTarget : TileDefinition 
    {
        [SerializeField] protected TTarget target;
        
        public TTarget Target => target;
        
        [SerializeField, Range(0, 99)] protected int priority;
        
        public int Priority => priority;
        
        public abstract IEnumerable<int2> DependentOnTileOffsets { get; }

        public abstract bool IsValid();

        public abstract bool IsMatch(int2 tile, SessionTileProvider provider);
    }
}