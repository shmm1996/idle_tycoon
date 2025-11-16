using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public sealed class TilemapSubProcessor<TTileDefinition>
        where TTileDefinition : TileDefinition
    {
        private readonly UnityEngine.Tilemaps.Tilemap _tilemap;
        private readonly SessionTileProvider _sessionTiles;
        private readonly TilemapRuleDefinition<TTileDefinition>[] _rules;
        
        private readonly string[,] _tileNames;
        private readonly Dictionary<string, WeightedSet<TileDefinition.TileView>> _tileViewWeightedSets;
        private readonly int2[] _dependentOnTileOffsets;

        private readonly List<Vector3Int> _lazyPositions;
        private readonly List<TileBase> _lazyTiles;
        private readonly List<Matrix4x4> _lazyTransform;
        
        private readonly TilemapRuleDefinition<TTileDefinition>[] _matchedBuffer;
        private readonly Dictionary<(int2, TilemapRuleDefinition<TTileDefinition>), bool> _matchCache;
        private readonly int2[] _affectedBuffer;
        
        public TilemapSubProcessor(UnityEngine.Tilemaps.Tilemap tilemap, SessionTileProvider sessionTiles, TilemapRuleDefinition<TTileDefinition>[] rules)
        {
            _tilemap = tilemap;
            _sessionTiles = sessionTiles;
            _rules = rules;
            
            _tileNames = new string[sessionTiles.WorldMapSize.x, sessionTiles.WorldMapSize.y];
            _tileViewWeightedSets = rules
                .ToDictionary(
                    r => r.Target.Name,
                    r => new WeightedSet<TileDefinition.TileView>(
                        r.Target.Tiles.ToArray(), t => t.weight));
            _dependentOnTileOffsets = rules
                .SelectMany(r => r.DependentOnTileOffsets)
                .Distinct()
                .ToArray();
            
            _lazyPositions = new List<Vector3Int>();
            _lazyTiles = new List<TileBase>();
            _lazyTransform = new List<Matrix4x4>();
            
            _matchCache = new Dictionary<(int2, TilemapRuleDefinition<TTileDefinition>), bool>();
            _matchedBuffer = new TilemapRuleDefinition<TTileDefinition>[_rules.Length];
            _affectedBuffer = new int2[_dependentOnTileOffsets.Length];
        }
        
        public bool TryLazyResolveTile(int2 tile)
        {
            if (_rules.Length == 0) return false;
            
            int matchedCount = FillMatchedRulesBuffer(tile);
            if (matchedCount == 0) return false;
            
            TilemapRuleDefinition<TTileDefinition> matchedRule = _matchedBuffer[0];
            for (int i = 1; i < matchedCount; i++)
                if (_matchedBuffer[i].Priority > matchedRule.Priority)
                    matchedRule = _matchedBuffer[i];
            if (matchedRule.Target.Name == _tileNames[tile.x, tile.y]) return false;

            WeightedSet<TileDefinition.TileView> tileViewWeightedSet = _tileViewWeightedSets[matchedRule.Target.Name];
            //int mod = HashFunctions.FastHash(tile);
            int mod = HashFunctions.ClusteredHash(tile);//.ClusteredHash(tile * 73856093);//
            if (!tileViewWeightedSet.TryPick(mod, out TileDefinition.TileView tileView)) return false;
            
            _tileNames[tile.x, tile.y] = matchedRule.Target.Name;
            _lazyPositions.Add(tile.ToVector3Int());
            _lazyTiles.Add(tileView.tile);
            
            var transform = Matrix4x4.identity;
            if (FlagsUtils.TryPickFlag(tileView.transformation.RotationFlags, mod, out TileTransformation.Rotation rotation, 4) && 
                rotation != TileTransformation.Rotation.Angle0)
                transform *= rotation.ToMatrix();
            int flipFlags = tileView.transformation.FlipFlags;
            if (flipFlags != (int)TileTransformation.Flip.None &&
                FlagsUtils.TryPickFlag(tileView.transformation.FlipFlags, mod, out TileTransformation.Flip flip, 3) &&
                flip != TileTransformation.Flip.None)
                transform *= flip.ToMatrix();
            _lazyTransform.Add(transform);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FillMatchedRulesBuffer(int2 tile)
        {
            int matchedCount = 0;
            foreach (TilemapRuleDefinition<TTileDefinition> rule in _rules)
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
            foreach (int2 offset in _dependentOnTileOffsets)
            {
                int2 t = tile - offset;
                if (_sessionTiles.OnWorldMap(t))
                    _affectedBuffer[count++] = t;
            }

            return new ReadOnlySpan<int2>(_affectedBuffer, 0, count);
        }

        public void Apply()
        {
            if (_lazyPositions.Count > 0)
            {
                _tilemap.SetTiles(_lazyPositions.ToArray(), _lazyTiles.ToArray());
                for (int i = 0; i < _lazyPositions.Count; i++)
                {
                    if (_lazyTransform[i] == Matrix4x4.identity) continue;
                    _tilemap.SetTransformMatrix(_lazyPositions[i], _lazyTransform[i]);
                }
                
                _lazyPositions.Clear();
                _lazyTiles.Clear();
                _lazyTransform.Clear();
            }

            _matchCache.Clear();
        }
    }
}