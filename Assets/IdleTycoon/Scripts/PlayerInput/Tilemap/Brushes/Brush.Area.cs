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

        public override void PrimaryDown(int2 tile)
        {
            isSelecting = true;
            start = tile;
            end = tile;

            UpdateAreaPreview();
        }

        public override void PrimaryDrag(int2 tile)
        {
            if (!isSelecting) return;

            end = tile;
            
            UpdateAreaPreview();
        }

        public override void PrimaryUp(int2 tile)
        {
            if (!isSelecting) return;

            isSelecting = false;
            start = tile;
            end = tile;

            UpdateAreaPreview();
        }

        public override void Cancel()
        {
            isSelecting = false;
            
            ClearAreaPreview();
        }

        private void UpdateAreaPreview() =>
            Preview = RectUtils.GetRectEnumerable(start, end)
                .Select(BuildPreview)
                .ToArray();
        
        private void ClearAreaPreview() =>
            Preview = Array.Empty<TilePreview>();
    }
}