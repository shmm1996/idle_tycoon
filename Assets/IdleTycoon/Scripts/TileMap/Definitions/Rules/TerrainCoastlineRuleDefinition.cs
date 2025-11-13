using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.TileMap.Definitions.Tiles;
using IdleTycoon.Scripts.TileMap.Processor;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.TileMap.Definitions.Rules
{
    [CreateAssetMenu(fileName = "CoastlineRule", menuName = "Definitions/TileMap/Rules/CoastlineRule", order = 0)]
    public class TerrainCoastlineRuleDefinition : TilemapRuleDefinition<TileTerrainDefinition>
    {
        [SerializeField] protected TileTerrainDefinition topTile;

        public override IEnumerable<int2> DependentOnTileOffsets { get; } = new int2[] { new(0, 1) };

        public override bool IsValid() => target && topTile && !target.Data.isGround && topTile.Data.isGround;

        public override bool IsMatch(int2 tile, SessionTileProvider provider) =>
            tile.y < provider.WorldMapSize.y - 1 &&
            !provider.HasAttribute(tile, TileAttributeFlag.IsGround) &&
            provider.HasAttribute(tile + new int2(0, 1), TileAttributeFlag.IsGround);
    }
}