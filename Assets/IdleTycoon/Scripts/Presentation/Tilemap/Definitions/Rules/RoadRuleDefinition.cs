using System.Collections.Generic;
using IdleTycoon.Scripts.CustomEditor.Attributes;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Presentation.Tilemap.Processor;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules
{
    [CreateAssetMenu(fileName = "Road(N)", menuName = "Definitions/TileMap/Rules/Roads", order = 0)]
    public sealed class RoadRuleDefinition : TilemapPartedTileRuleDefinition<TileRoadDefinition>
    {
        [SerializeField, EnumFlags] private TileRoad.Neighbour isRoadNeighborFlags;
        [SerializeField, EnumFlags] private TileRoad.Neighbour isNotRoadNeighborFlags;

        public override IEnumerable<int2> DependentOnTileOffsets
        {
            get
            {
                int mask = (int)isRoadNeighborFlags & (int)isNotRoadNeighborFlags;
                for (int n = 0; n < 8; n++)
                {
                    int bit = 1 << n;
                    if ((mask & bit) != 0)
                        yield return ((TileRoad.Neighbour)bit).ToOffset();
                }
            }
        }

        public override bool IsValid() => target && ((int)isRoadNeighborFlags & (int)isNotRoadNeighborFlags & 0b11111111) == 0;

        public override bool IsMatch(int2 tile, SessionTileProvider provider)
        {
            if (!provider.HasAttribute(tile, TileAttributeFlag.IsRoad)) return false;

            int isRoadMask = (int)isRoadNeighborFlags;
            int isNotRoadMask = (int)isNotRoadNeighborFlags;
            for (int n = 0; n < 8; n++)
            {
                int bit = 1 << n;
                if ((isRoadMask & bit) != 0)
                {
                    var neighbour = (TileRoad.Neighbour)bit;
                    int2 nextTile = tile + neighbour.ToOffset();
                    if (!provider.OnWorldMap(nextTile) || !provider.HasAttribute(nextTile, TileAttributeFlag.IsRoad)) return false;
                }
                else if ((isNotRoadMask & bit) != 0)
                {
                    var neighbour = (TileRoad.Neighbour)bit;
                    int2 nextTile = tile + neighbour.ToOffset();
                    if (provider.OnWorldMap(nextTile) && provider.HasAttribute(nextTile, TileAttributeFlag.IsRoad)) return false;
                }
            }

            return true;
        }

        public override int2 TileToSubTile(int2 tile) => tile * 2;
    }
}