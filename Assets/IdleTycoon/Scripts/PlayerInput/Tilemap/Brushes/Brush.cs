using System;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public abstract class Brush
    {
        public TilePreview[] Preview { get; protected  set; } = Array.Empty<TilePreview>();

        public bool CanApply { get; protected  set; } = false;

        public abstract void Hover(int2 tile);
        
        public abstract void PrimaryDown(int2 tile);
        
        public abstract void PrimaryDrag(int2 tile);
        
        public abstract void PrimaryUp(int2 tile);
        
        public abstract void Cancel();
    }
}