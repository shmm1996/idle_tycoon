using System;
using System.Collections.Generic;
using System.Linq;

namespace IdleTycoon.Scripts.Utils
{
    public sealed class RandomProvider
    {
        private readonly Dictionary<string, Random> _randoms;
        
        public RandomProvider(Dictionary<string, int> seeds)
        {
            _randoms = seeds.ToDictionary(s => s.Key, s => new Random(s.Value));
        }

        private Random Get(string name)
        {
            if (_randoms.TryGetValue(name, out Random random)) return random;
            random = new Random();
            _randoms.Add(name, random);
            
            return random;
        }
        
        public int Next(string random, int max) => Next(random, 0, max);
        
        public int Next(string random, int min, int max) => Get(random).Next(min, max);
    }
}