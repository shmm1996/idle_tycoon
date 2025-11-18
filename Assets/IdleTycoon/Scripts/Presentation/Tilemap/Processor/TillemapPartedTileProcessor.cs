using System;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public sealed class TilemapPartedTileProcessor<TRuleDefinition, TTileDefinition> : TilemapSubProcessor<TRuleDefinition, TTileDefinition, PartedTileDefinition.PartedTileView>
        where TRuleDefinition : TilemapPartedTileRuleDefinition<TTileDefinition>
        where TTileDefinition : PartedTileDefinition
    {
        public TilemapPartedTileProcessor(
            UnityEngine.Tilemaps.Tilemap tilemap,
            SessionTileProvider sessionTiles,
            TRuleDefinition[] rules,
            Func<PartedTileDefinition.PartedTileView, int> getWeight)
            : base(tilemap, sessionTiles, rules, getWeight)
        {
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
    }
}