using System.Collections.Generic;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Brushes
{
    public abstract class Brush
    {
        public IEnumerable<TilePreview> Preview { get; protected  set; }

        public bool CanApply { get; protected  set; }
        
        public abstract void OnPointerDown(int2 tile);
        
        public abstract void OnPointerMove(int2 tile);
        
        public abstract void OnPointerUp(int2 tile);

        public abstract void Cancel();
        
        public struct TilePreview
        {
            public int2 tile;
            public bool isValid;
            public int projectionId;
        }
    }
}