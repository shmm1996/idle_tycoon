using System.Collections.Generic;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Utils
{
    public static class RectUtils
    {
        public static IEnumerable<int2> GetRectEnumerable(int2 a, int2 b)
        {
            int minX = math.min(a.x, b.x);
            int maxX = math.max(a.x, b.x);
            int minY = math.min(a.y, b.y);
            int maxY = math.max(a.y, b.y);

            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
                yield return new int2(x, y);
        }
        
        public static int2[] GetRectArray(int2 a, int2 b)
        {
            int minX = math.min(a.x, b.x);
            int maxX = math.max(a.x, b.x);
            int minY = math.min(a.y, b.y);
            int maxY = math.max(a.y, b.y);
            
            int count = (maxX - minX + 1) * (maxY - minY + 1);
            var array = new int2[count];
            int index = 0;
            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
                array[index++] = new int2(x, y);
            
            return array;
        }
    }
}