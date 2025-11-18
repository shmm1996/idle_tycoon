using System;
using System.Runtime.CompilerServices;

namespace IdleTycoon.Scripts.Utils
{
    public sealed class WeightedSet<T>
    {
        private readonly T[] _source;
        private readonly int[] _indices;
        private readonly int _totalWeight;

        public WeightedSet(T[] source, Func<T, int> getWeight)
        {
            _source = source;
            
            _totalWeight = 0;
            for (int i = 0; i < source.Length; i++)
            {
                int weight = getWeight.Invoke(source[i]);
                if (weight > 0)
                    _totalWeight += weight;
            }
            
            _indices = new int[_totalWeight];
            for (int i = 0, j = 0; i < source.Length; i++)
            {
                int weight = getWeight.Invoke(source[i]);
                for (int k = 0; k < weight; k++, j++)
                    _indices[j] = i;
            }
            Shuffle();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Shuffle()
        {
            uint state = 0xA511E9B3u;
            for (int i = _totalWeight - 1; i > 0; i--)
            {
                state ^= state << 13; state ^= state >> 17; state ^= state << 5;
                int j = (int)(state % (uint)(i + 1));
                (_indices[i], _indices[j]) = (_indices[j], _indices[i]);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPick(int mod, out T entity)
        {
            if (_totalWeight == 0)
            {
                entity = default;
                return false;
            }
            
            int normalized = mod % _indices.Length;
            if (normalized < 0) 
                normalized += _indices.Length;
            entity = _source[_indices[normalized]];
            
            return true;
        }
    }
}