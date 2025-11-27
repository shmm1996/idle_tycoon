using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules
{
    public abstract class TilemapRuleDefinitionBase<TTarget> : ScriptableObject
    {
        [SerializeField] protected TTarget target;
        
        public TTarget Target => target;
        
        [SerializeField, Range(0, 99)] protected int priority;
        
        public int Priority => priority;
        
        public abstract IEnumerable<int2> DependentOnTileOffsets { get; }

        public abstract bool IsValid();

        public abstract bool IsMatch(int2 tile, WorldMap.ReadOnly worldMap);
    }
    
    public abstract class TilemapTileRuleDefinition<TTarget> : TilemapRuleDefinitionBase<TTarget>
        where TTarget : TileDefinition
    {

    }
    
    public abstract class TilemapPartedTileRuleDefinition<TTarget> : TilemapRuleDefinitionBase<TTarget>
        where TTarget : PartedTileDefinition
    {
        public abstract int2 TileToSubTile(int2 tile);
    }
}