using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session.Proxy
{
    public readonly struct TileProxy
    {
        public readonly int2 position;
        public readonly Chunk8X8.ReadOnly chunk;
        public readonly int index;

        public TileProxy(int2 position, Chunk8X8.ReadOnly chunk)
        {
            this.position = position;
            this.chunk = chunk;
            index = Chunk8X8Utils.ToIndexFromGlobal(position);
        }

        public TileProxy(int2 position, WorldMap.ReadOnly world)
            : this(position, world.GetChunkAsReadOnly(Chunk8X8Utils.ToChunk(position))) { }
    }
}