using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Utils
{
    public static class MathConvertExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 ToInt2(this Vector2Int v) => new(v.x, v.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ToVector2Int(this int2 v) => new(v.x, v.y);
    }
}