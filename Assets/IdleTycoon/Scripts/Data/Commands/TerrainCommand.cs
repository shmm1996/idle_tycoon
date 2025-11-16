using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Commands
{
    public struct TerrainCommand
    {
        public struct Ground
        {
            public struct Set : IGameCommand
            {
                public int2 tile;
            }

            public struct Remove : IGameCommand
            {
                public int2 tile;
            }
        }
    }
}