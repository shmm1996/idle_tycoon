using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace IdleTycoon.Scripts.Utils
{
    public static unsafe class UnsafeUtilityUtils
    {
        public const int MemoryBlockAlignment = 64;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* MallocArray<T>(int capacity, Allocator allocator = Allocator.Persistent) where T : unmanaged
        {
            long size = (long)UnsafeUtility.SizeOf<T>() * capacity;
            return (T*)UnsafeUtility.Malloc(size, MemoryBlockAlignment, allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* ReAllocArray<T>(T* oldPtr, int oldLen, int newLen, Allocator allocator = Allocator.Persistent) where T : unmanaged
        {
            T* newPtr = MallocArray<T>(newLen);

            if (oldPtr != null && oldLen > 0)
            {
                long copyBytes = (long)UnsafeUtility.SizeOf<T>() * oldLen;
                UnsafeUtility.MemCpy(newPtr, oldPtr, copyBytes);
                UnsafeUtility.Free(oldPtr, allocator);
            }

            return newPtr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* p, Allocator allocator = Allocator.Persistent)
        {
            if (p != null) UnsafeUtility.Free(p, allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundUpPow2(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            return v + 1;
        }
    }
}