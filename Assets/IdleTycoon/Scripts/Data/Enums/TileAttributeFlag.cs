using System;

namespace IdleTycoon.Scripts.Data.Enums
{
    [Flags]
    public enum TileAttributeBit : int
    {
        IsGround = 1 << TileAttributeBitPosition.IsGround,
        IsRoad = 1 << TileAttributeBitPosition.IsRoad,
    }

    public enum TileAttributeBitPosition : int
    {
        IsGround = 0,
        IsRoad = 3,
    }
}