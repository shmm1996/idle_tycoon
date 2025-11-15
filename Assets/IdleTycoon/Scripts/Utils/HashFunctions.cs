using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Utils
{
    public static class HashFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastHash(int2 pos)
        {
            uint hash = (uint)pos.x * 0x8DA6B343u ^ (uint)pos.y * 0xD8163841u;
            hash ^= hash >> 13;
            hash *= 0x85EBCA6Bu;
            hash ^= hash >> 16;
            
            return (int)hash;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClusteredHash(int2 pos)
        {
            uint hash = 0x9E3779B9u;
            hash ^= (uint)pos.x + 0x85EBCA6Bu + (hash << 6) + (hash >> 2);
            hash ^= (uint)pos.y + 0xC2B2AE35u + (hash << 6) + (hash >> 2);
            hash ^= hash >> 13;
            hash *= 0x85EBCA6Bu;
            hash ^= hash >> 16;

            return (int)hash;
        }
    }
}