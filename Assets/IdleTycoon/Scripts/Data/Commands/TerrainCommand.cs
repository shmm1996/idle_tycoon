using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Commands
{
    public struct TerrainCommand
    {
        public struct Ground
        {
            public readonly struct Set : IGameCommand
            {
                public readonly int2 tile;

                public Set(int2 tile)
                {
                    this.tile = tile;
                }
            }

            public readonly struct Remove : IGameCommand
            {
                public readonly int2 tile;

                public Remove(int2 tile)
                {
                    this.tile = tile;
                }
            }
        }
    }
}