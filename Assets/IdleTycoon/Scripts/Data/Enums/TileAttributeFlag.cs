using System;

namespace IdleTycoon.Scripts.Data.Enums
{
    [Flags]
    public enum TileAttributeFlag : int
    {
        IsGround = 1,
        
        IsRoad = 1 << 4,
        /*
        //---Building bits--- offset: 1bit
        BuildingResidential = 1 << (BuildingType.Residential + 1),
        BuildingProduction = 1 << (BuildingType.Production + 1),
        BuildingInfrastructure = 1 << (BuildingType.Infrastructure + 1),
        BuildingDecoration = 1 << (BuildingType.Decoration + 1),
        
        */
        
    }
}