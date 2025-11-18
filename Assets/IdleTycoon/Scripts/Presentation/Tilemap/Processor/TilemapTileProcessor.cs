using System;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Processor
{
    public sealed class TilemapTileProcessor<TRuleDefinition, TTileDefinition> : TilemapSubProcessor<TRuleDefinition, TTileDefinition, TileDefinition.TileView>
        where TRuleDefinition : TilemapTileRuleDefinition<TTileDefinition>
        where TTileDefinition : TileDefinition
    {
        public TilemapTileProcessor(
            UnityEngine.Tilemaps.Tilemap tilemap, 
            SessionTileProvider sessionTiles, 
            TRuleDefinition[] rules, 
            Func<TileDefinition.TileView, int> getWeight) 
            : base(tilemap, sessionTiles, rules, getWeight)
        {
        }

        protected override bool TryLazyAddTile(int2 tile, TRuleDefinition matchedRule)
        {
            WeightedSet<TileDefinition.TileView> viewWeightedSet = tileViewWeightedSets[matchedRule.Target.Name];
            int mod = HashFunctions.ClusteredHash(tile);
            if (!viewWeightedSet.TryPick(mod, out TileDefinition.TileView view)) return false;
            
            tileNames[tile.x, tile.y] = matchedRule.Target.Name;
            
            lazyPositions.Add(tile.ToVector3Int());
            lazyTiles.Add(view.tile);
            
            var transform = Matrix4x4.identity;
            if (FlagsUtils.TryPickFlag(view.transformation.RotationFlags, mod, out TileTransformation.Rotation rotation, 4) && 
                rotation != TileTransformation.Rotation.Angle0)
                transform *= rotation.ToMatrix();
            int flipFlags = view.transformation.FlipFlags;
            if (flipFlags != (int)TileTransformation.Flip.None &&
                FlagsUtils.TryPickFlag(view.transformation.FlipFlags, mod, out TileTransformation.Flip flip, 3) &&
                flip != TileTransformation.Flip.None)
                transform *= flip.ToMatrix();
            lazyTransform.Add(transform);

            return true;
        }
    }
}

