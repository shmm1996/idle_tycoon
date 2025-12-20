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
        
        public virtual void Down(int2 tile) {}
        
        public virtual void Drag(int2 tile) {}
        
        public virtual void Up(int2 tile) {}
        
        public virtual void Cancel() {}

        protected abstract TilePreview BuildPreview(int2 tile);
    }
}