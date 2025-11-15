using System;
using System.Runtime.CompilerServices;

namespace IdleTycoon.Scripts.Utils
{
    public static class FlagsUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryPickFlag(int mask, int mod, out int flag, int bitsLimit = 32)
        {
            if (mask == 0)
            {
                flag = 0;
                return false;
            }

            int count = BitUtil.PopCount32((uint)mask);
            int index = mod % count;
            if (index < 0) index += count;

            for (int bit = 0; bit < bitsLimit; bit++)
            {
                flag = 1 << bit;
                if ((mask & flag) != 0 && index-- == 0) return true;
            }

            flag = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryPickFlag<TEnum>(int mask, int mod, out TEnum value, int bitsLimit = 32)
            where TEnum : Enum
        {
            bool result = TryPickFlag(mask, mod, out int flag, bitsLimit);
            value = (TEnum)(object)flag;

            return result;
        }
    }
}