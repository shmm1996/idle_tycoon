using System;
using System.Runtime.CompilerServices;

namespace IdleTycoon.Scripts.Utils
{
    public class WeightedSet<T>
    {
        private readonly T[] _source;
        private readonly int[] _prefixSums;
        
        public readonly int totalWeight;

        public WeightedSet(T[] source, Func<T, int> getWeight)
        {
            _source = source;
            _prefixSums = new int[source.Length];
            totalWeight = 0;
            
            for (int i = 0; i < source.Length; i++)
            {
                int weight = getWeight.Invoke(source[i]);
                if (weight < 0) 
                    weight = 0;
                totalWeight += weight;
                _prefixSums[i] = totalWeight;
            }
        }
        
        public bool TryPick(int mod, out T entity)
        {
            if (totalWeight == 0)
            {
                entity = default;
                return false;
            }
            
            entity = Search(mod);
            
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Search(int mod)
        {
            int normalized = mod % totalWeight;
            if (normalized < 0) 
                normalized += totalWeight;

            int i = 0;
            for (; i < _source.Length; i++)
                if (normalized < _prefixSums[i]) break;

            return _source[i];
        }
    }
}