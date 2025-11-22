using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Commands
{
    public struct RoadCommand
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