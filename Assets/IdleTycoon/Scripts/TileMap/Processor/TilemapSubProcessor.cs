using System.Collections.Generic;
using System.Linq;
using IdleTycoon.Scripts.TileMap.Definitions.Rules;
using IdleTycoon.Scripts.TileMap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.TileMap.Processor
{
    public sealed class TilemapSubProcessor<TTileDefinition>
        where TTileDefinition : TileDefinition
    {
        private const string RandomName = "tilemap";
        
        private readonly Tilemap _tilemap;
        private readonly SessionTileProvider _sessionTiles;
        private readonly TilemapRuleDefinition<TTileDefinition>[] _rules;
        private readonly RandomProvider _random;
        
        private readonly string[,] _tileNames;
        private readonly Dictionary<string, WeightedSet<TileDefinition.TileView>> _tileViewWeightedSets;
        private readonly int2[] _dependentOnTileOffsets;
        
        private readonly Dictionary<(int2, TilemapRuleDefinition<TTileDefinition>), bool> _matchCache;
        private readonly List<Vector3Int> _lazyPositions;
        private readonly List<TileBase> _lazyTiles;

        private readonly RectInt _worldMapRect;
        
        public TilemapSubProcessor(Tilemap tilemap, SessionTileProvider sessionTiles, TilemapRuleDefinition<TTileDefinition>[] rules, RandomProvider random)
        {
            _tilemap = tilemap;
            _sessionTiles = sessionTiles;
            _rules = rules;
            _random = random;
            
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
            
            _matchCache = new Dictionary<(int2, TilemapRuleDefinition<TTileDefinition>), bool>();
            _lazyPositions = new List<Vector3Int>();
            _lazyTiles = new List<TileBase>();

            _worldMapRect = new RectInt(0, 0, sessionTiles.WorldMapSize.x, sessionTiles.WorldMapSize.y);
        }
        
        public bool TryLazyResolveTile(int2 tile)
        {
            TilemapRuleDefinition<TTileDefinition>[] matchedRules = _rules.Where(r =>
            {
                var key = (tile, r);
                if (_matchCache.TryGetValue(key, out bool isMatch)) return isMatch;
                isMatch = r.IsMatch(tile, _sessionTiles);
                _matchCache[key] = isMatch;
                return isMatch;
            }).ToArray();

            if (matchedRules.Length == 0) return false;
            
            TilemapRuleDefinition<TTileDefinition> matchedRule = matchedRules.OrderByDescending(r => r.Priority).First();
            if (matchedRule.Target.Name ==  _tileNames[tile.x, tile.y]) return false;

            WeightedSet<TileDefinition.TileView> tileViewWeightedSet = _tileViewWeightedSets[matchedRule.Target.Name];
            int mod = _random.Next(RandomName, tileViewWeightedSet.totalWeight);
            if (!tileViewWeightedSet.TryPick(mod, out TileDefinition.TileView tileView)) return false;
            
            _tileNames[tile.x, tile.y] = matchedRule.Target.Name;
            _lazyPositions.Add(tile.ToVector3Int());
            _lazyTiles.Add(tileView.tile);

            return true;
        }
        
        public IEnumerable<int2> GetAffectedTiles(int2 tile) => 
            _dependentOnTileOffsets
                .Select(o => tile - o)
                .Where(t => _worldMapRect.Contains(t.ToVector2Int()));

        public void Apply()
        {
            if (_lazyPositions.Count > 0)
            {
                _tilemap.SetTiles(_lazyPositions.ToArray(), _lazyTiles.ToArray());
                _lazyPositions.Clear();
                _lazyTiles.Clear();
            }

            _matchCache.Clear();
        }
    }
}