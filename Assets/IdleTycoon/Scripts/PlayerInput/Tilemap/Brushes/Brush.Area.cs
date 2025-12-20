using System;
using System.Linq;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using IdleTycoon.Scripts.Utils;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public abstract class BrushArea : Brush
    {
        protected int2 start;
        protected int2 end;
        protected bool isSelecting;
        
        public override void Hover(int2 tile)
        {
            if (isSelecting) return;

            Preview = new[] { new TilePreview(tile, true, TilePreviewStyle.Default) };
        }

        public override void Down(int2 tile)
        {
            isSelecting = true;
            start = tile;
            end = tile;

            ComputePreview();
        }

        public override void Drag(int2 tile)
        {
            if (!isSelecting) return;

            end = tile;
            
            ComputePreview();
        }

        public override void Up(int2 tile)
        {
            if (!isSelecting) return;

            isSelecting = false;
            start = tile;
            end = tile;

            ComputePreview();
        }

        public override void Cancel()
        {
            isSelecting = false;
            
            ResetPreview();
        }

        private void ComputePreview() =>
            Preview = RectUtils.GetRectEnumerable(start, end)
                .Select(BuildPreview)
                .ToArray();
        
        private void ResetPreview() =>
            Preview = Array.Empty<TilePreview>();
    }
}