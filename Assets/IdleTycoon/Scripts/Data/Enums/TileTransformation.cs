using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Enums
{
    public static class TileTransformation
    {
        [Flags]
        public enum Rotation
        {
            Angle0 = 1,
            Angle90 = 1 << 1,
            Angle180 = 1 << 2,
            Angle270 = 1 << 3,
        }

        [Flags]
        public enum Flip
        {
            None = 1,
            Vertical = 1 << 1,
            Horizontal = 1 << 2,
            Both = 1 << 3
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 ToMatrix(this Rotation rotation) =>
            rotation switch
            {
                Rotation.Angle0 => Matrix4x4.identity,
                Rotation.Angle90 => Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90)),
                Rotation.Angle180 => Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180)),
                Rotation.Angle270 => Matrix4x4.Rotate(Quaternion.Euler(0, 0, 270)),
                _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
            };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 ToMatrix(this Flip flip) =>
            flip switch
            {
                Flip.None => Matrix4x4.identity,
                Flip.Vertical => Matrix4x4.Scale(new Vector3(-1, 1, 1)),
                Flip.Horizontal => Matrix4x4.Scale(new Vector3(1, -1, 1)),
                Flip.Both => Matrix4x4.Scale(new Vector3(-1, -1, 1)),
                _ => throw new ArgumentOutOfRangeException(nameof(flip), flip, null)
            };
    }
}