using System;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public abstract class Brush
    {
        public TilePreview[] Preview { get; protected  set; } = Array.Empty<TilePreview>();

        public bool CanApply { get; protected  set; } = false;

        public virtual void Hover(int2 tile) {}
        
        public virtual void PrimaryDown(int2 tile) {}
        
        public virtual void PrimaryDrag(int2 tile) {}
        
        public virtual void PrimaryUp(int2 tile) {}
        
        public virtual void Cancel() {}

        protected abstract TilePreview BuildPreview(int2 tile);
    }
}