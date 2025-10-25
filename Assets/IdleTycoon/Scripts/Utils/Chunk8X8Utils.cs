using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Utils
{
    public static class Chunk8X8Utils
    {
        private const int ChunkSize = 8;
        private const int ChunkMask = ChunkSize - 1; // 0b0111
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 ToLocal(int2 global) => new int2(global.x & ChunkMask, global.y & ChunkMask);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndex(int2 local) => local.y * ChunkSize + local.x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 ToLocal(int index) => new int2(index & ChunkSize, index / ChunkSize);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 ToGlobal(int2 chunk, int2 local) => chunk * ChunkSize + local;
    }
}