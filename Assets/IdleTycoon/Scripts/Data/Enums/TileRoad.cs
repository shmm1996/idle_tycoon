using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Enums
{
    public static class TileRoad
    {
        public enum Variant
        {
            Island = 0,
            DeadEnd = 1,
            Straight = 2,
            Corner = 3,
            CrossroadT = 4,
            CrossroadX = 5,
        }
        
        [Flags]
        public enum Neighbour
        {
            UpLeft = 1,
            Up = 1 << 1,
            UpRight = 1 << 2,
            Left = 1 << 3,
            Right = 1 << 4,
            DownLeft = 1 << 5,
            Down = 1 << 6,
            DownRight = 1 << 7,
        }

        public static int2 ToOffset(this Neighbour neighbour) =>
            neighbour switch
            {
                Neighbour.UpLeft => new int2(-1, 1),
                Neighbour.Up => new int2(0, 1),
                Neighbour.UpRight => new int2(1, 1),
                Neighbour.Left => new int2(-1, 0),
                Neighbour.Right => new int2(1, 0),
                Neighbour.DownLeft => new int2(-1, -1),
                Neighbour.Down => new int2(0, -1),
                Neighbour.DownRight => new int2(1, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(neighbour), neighbour, null)
            };

        public static IEnumerable<int2> ToOffsets(this Neighbour flags) //TODO: optimize.
        {
            var mask = (int)flags;
            if (flags == 0) yield break;
            for (int i = 0; i < 8; i++)
            {
                int v = 1 << i;
                if ((mask & v) != 0)
                    yield return ToOffset((Neighbour)v);
            }
        }
    }
}