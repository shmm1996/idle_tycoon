using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules
{
    [CreateAssetMenu(fileName = "GroundRule", menuName = "Definitions/TileMap/Rules/Terrain/GroundRule", order = 0)]
    public class TerrainGroundRuleDefinition : TilemapTileRuleDefinition<TileTerrainDefinition>
    {
        public override IEnumerable<int2> DependentOnTileOffsets { get; } = Array.Empty<int2>();

        public override bool IsValid() => target;

        public override bool IsMatch(int2 tile, WorldMap.ReadOnly worldMap) => worldMap.HasAttribute(tile, (int)TileAttributeFlag.IsGround);
    }
}