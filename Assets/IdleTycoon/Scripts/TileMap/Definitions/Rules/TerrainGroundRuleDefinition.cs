using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.TileMap.Definitions.Tiles;
using IdleTycoon.Scripts.TileMap.Processor;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.TileMap.Definitions.Rules
{
    [CreateAssetMenu(fileName = "GroundRule", menuName = "Definitions/TileMap/Rules/GroundRule", order = 0)]
    public class TerrainGroundRuleDefinition : TilemapRuleDefinition<TileTerrainDefinition>
    {
        public override IEnumerable<int2> DependentOnTileOffsets { get; } = Array.Empty<int2>();

        public override bool IsValid() => target && target.Data.isGround;

        public override bool IsMatch(int2 tile, SessionTileProvider provider) => provider.HasAttribute(tile, TileAttributeFlag.IsGround);
    }
}