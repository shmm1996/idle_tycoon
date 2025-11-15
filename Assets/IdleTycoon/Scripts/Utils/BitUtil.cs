using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace IdleTycoon.Scripts.Utils
{
    [BurstCompile]
    public static unsafe class BitUtil
    {
        private static readonly byte* TrailingZeroTablePtr = InitTrailingZeroTable();

        private static unsafe byte* InitTrailingZeroTable()
        {
            byte* table = (byte*)UnsafeUtility.Malloc(32, 1, Allocator.Persistent);
            byte[] src = new byte[32]
            {
                0, 1, 28, 2, 29, 14, 24, 3,
                30, 22, 20, 15, 25, 17, 4, 8,
                31, 27, 13, 23, 21, 19, 16, 7,
                26, 12, 18, 6, 11, 5, 10, 9
            };

            for (int i = 0; i < 32; i++)
                table[i] = src[i];

            return table;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount(ulong v) => 
            (uint)v == 0 
                ? 32 + TrailingZeroCount32((uint)(v >> 32)) 
                : TrailingZeroCount32((uint)v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int TrailingZeroCount32(uint v)
        {
            if (v == 0)
                return 32;

            uint isolated = v & (uint)(-v);
            uint index = (isolated * 0x077CB531u) >> 27;

            return TrailingZeroTablePtr[index];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount32(uint v)
        {
            v = v - ((v >> 1) & 0x55555555u);
            v = (v & 0x33333333u) + ((v >> 2) & 0x33333333u);
            v = (v + (v >> 4)) & 0x0F0F0F0Fu;
            v = v * 0x01010101u;

            return (int)(v >> 24);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Cleanup() => UnsafeUtility.Free(TrailingZeroTablePtr, Allocator.Persistent);
    }
}