using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public abstract class TilemapSubProcessor<TRuleDefinition, TTileDefinition, TTileView> : ITilemapSubProcessor
        where TRuleDefinition : TilemapRuleDefinitionBase<TTileDefinition>
        where TTileDefinition : TileDefinitionBase<TTileView>
        where TTileView : struct
    {
        private readonly UnityEngine.Tilemaps.Tilemap _tilemap;
        private readonly SessionTileProvider _sessionTiles;
        private readonly TRuleDefinition[] _rules;
        
        private readonly TRuleDefinition[] _matchedBuffer;
        private readonly Dictionary<(int2, TRuleDefinition), bool> _matchCache;
        private readonly int2[] _affectedBuffer;
        
        protected readonly int2[] dependentOnTileOffsets;
        protected readonly string[,] tileNames;
        protected readonly Dictionary<string, WeightedSet<TTileView>> tileViewWeightedSets;
        
        protected readonly List<Vector3Int> lazyPositions;
        protected readonly List<TileBase> lazyTiles;
        protected readonly List<Matrix4x4> lazyTransform;

        protected TilemapSubProcessor(
            UnityEngine.Tilemaps.Tilemap tilemap, 
            SessionTileProvider sessionTiles, 
            TRuleDefinition[] rules,
            Func<TTileView, int> getWeight)
        {
            _tilemap = tilemap;
            _sessionTiles = sessionTiles;
            _rules = rules;
            
            tileNames = new string[sessionTiles.WorldMapSize.x, sessionTiles.WorldMapSize.y];
            tileViewWeightedSets = rules
                .Where(r =>
                {
                    bool isValid = r.IsValid();
                    if(isValid) return true;
                    Debug.LogWarning($"[{r.GetType().Name}.{nameof(r.IsValid)}] Invalid tilemap rule definition.");
                    return false;
                })
                .ToDictionary(
                    r => r.Target.Name,
                    r => new WeightedSet<TTileView>(
                        r.Target.Tiles.ToArray(), getWeight));
            dependentOnTileOffsets = rules
                .SelectMany(r => r.DependentOnTileOffsets)
                .Distinct()
                .ToArray();
            
            lazyPositions = new List<Vector3Int>();
            lazyTiles = new List<TileBase>();
            lazyTransform = new List<Matrix4x4>();
            
            _matchCache = new Dictionary<(int2, TRuleDefinition), bool>();
            _matchedBuffer = new TRuleDefinition[_rules.Length];
            _affectedBuffer = new int2[dependentOnTileOffsets.Length];
        }
        
        public bool TryLazyResolveTile(int2 tile)
        {
            if (_rules.Length == 0) return false;
            
            int matchedCount = FillMatchedRulesBuffer(tile);
            if (matchedCount == 0) return false;
            
            TRuleDefinition matchedRule = _matchedBuffer[0];
            for (int i = 1; i < matchedCount; i++)
                if (_matchedBuffer[i].Priority > matchedRule.Priority)
                    matchedRule = _matchedBuffer[i];
            if (matchedRule.Target.Name == tileNames[tile.x, tile.y]) return false;
            
            return TryLazyAddTile(tile, matchedRule);
        }
        
        protected abstract bool TryLazyAddTile(int2 tile, TRuleDefinition matchedRule);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FillMatchedRulesBuffer(int2 tile)
        {
            int matchedCount = 0;
            foreach (TRuleDefinition rule in _rules)
            {
                var key = (tile, rule);
                var isMatch = _matchCache.TryGetValue(key, out bool cached)
                    ? cached
                    : _matchCache[key] = rule.IsMatch(tile, _sessionTiles);

                if (isMatch)
                    _matchedBuffer[matchedCount++] = rule;
            }
            
            return matchedCount;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<int2> GetAffectedTiles(int2 tile)
        {
            int count = 0;
            foreach (int2 offset in dependentOnTileOffsets)
            {
                int2 t = tile - offset;
                if (_sessionTiles.OnWorldMap(t))
                    _affectedBuffer[count++] = t;
            }

            return new ReadOnlySpan<int2>(_affectedBuffer, 0, count);
        }

        public void Apply()
        {
            if (lazyPositions.Count > 0)
            {
                _tilemap.SetTiles(lazyPositions.ToArray(), lazyTiles.ToArray());
                for (int i = 0; i < lazyPositions.Count; i++)
                {
                    if (lazyTransform[i] == Matrix4x4.identity) continue;
                    _tilemap.SetTransformMatrix(lazyPositions[i], lazyTransform[i]);
                }
                
                lazyPositions.Clear();
                lazyTiles.Clear();
                lazyTransform.Clear();
            }

            _matchCache.Clear();
        }
    }
}