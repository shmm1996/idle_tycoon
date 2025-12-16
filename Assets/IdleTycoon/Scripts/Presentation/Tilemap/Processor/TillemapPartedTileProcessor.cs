using System;
using System.Linq;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public sealed class TilemapPartedTileProcessor<TRuleDefinition, TTileDefinition> : TilemapSubProcessor<TRuleDefinition, TTileDefinition, PartedTileDefinition.PartedTileView>
        where TRuleDefinition : TilemapPartedTileRuleDefinition<TTileDefinition>
        where TTileDefinition : PartedTileDefinition
    {
        private readonly TRuleDefinition _anyRule;
        private readonly int2[] _anySubTileViewOffsets;
        
        public TilemapPartedTileProcessor(
            UnityEngine.Tilemaps.Tilemap tilemap,
            GameSession.Context context,
            TRuleDefinition[] rules,
            Func<PartedTileDefinition.PartedTileView, int> getWeight)
            : base(tilemap, context, rules, getWeight)
        {
            _anyRule = rules[0];
            _anySubTileViewOffsets =_anyRule.Target.Tiles.First().views.Select(v => v.offset).ToArray();

            var anyTile = new int2(1, 3);
            if (rules.Any(r => !r.TileToSubTile(anyTile).Equals(_anyRule.TileToSubTile(anyTile))))
                Debug.LogError("There are rules with different TileToSubTile() implementations.");
            
            /*
            int[][] subTilesViewOffsets = rules.SelectMany(r => r.Target.Tiles)
                .Select(t => t.views.Select(v => v.offset.GetHashCode()).OrderBy(o => o).ToArray())
                .ToArray();
                */
        }

        protected override bool TryLazyAddTile(int2 tile, TRuleDefinition matchedRule)
        {
            WeightedSet<PartedTileDefinition.PartedTileView> viewWeightedSet = tileViewWeightedSets[matchedRule.Target.Name];
            int mod = HashFunctions.ClusteredHash(tile);
            if (!viewWeightedSet.TryPick(mod, out PartedTileDefinition.PartedTileView view)) return false;
            
            tileNames[tile.x, tile.y] = matchedRule.Target.Name;
            
            int2 subTile = matchedRule.TileToSubTile(tile);
            foreach (PartedTileDefinition.SubTileView subView in view.views)
            {
                lazyPositions.Add((subTile + subView.offset).ToVector3Int());
                lazyTiles.Add(subView.tile);
                lazyTransform.Add(subView.transformation.rotation.ToMatrix() * subView.transformation.flip.ToMatrix());
            }
            
            return true;
        }

        protected override void LazyRemoveTile(int2 tile)
        {
            tileNames[tile.x, tile.y] = null;
            
            int2 subTile = _anyRule.TileToSubTile(tile);
            foreach (int2 offset in _anySubTileViewOffsets)
            {
                lazyPositions.Add((subTile + offset).ToVector3Int());
                lazyTiles.Add(null);
                lazyTransform.Add(Matrix4x4.identity);
            }
        }
    }
}