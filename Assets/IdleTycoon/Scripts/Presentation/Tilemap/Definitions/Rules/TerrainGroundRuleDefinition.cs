using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Presentation.Tilemap.Processor;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules
{
    [CreateAssetMenu(fileName = "GroundRule", menuName = "Definitions/TileMap/Rules/Terrain/GroundRule", order = 0)]
    public class TerrainGroundRuleDefinition : TilemapTileRuleDefinition<TileTerrainDefinition>
    {
        public override IEnumerable<int2> DependentOnTileOffsets { get; } = Array.Empty<int2>();

        public override bool IsValid() => target;

        public override bool IsMatch(int2 tile, SessionTileProvider provider) => provider.HasAttribute(tile, TileAttributeFlag.IsGround);
    }
}