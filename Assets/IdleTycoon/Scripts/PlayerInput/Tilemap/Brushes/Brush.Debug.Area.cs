using System;
using System.Linq;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using IdleTycoon.Scripts.Utils;
using UnityEngine;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public sealed class BrushDebugArea : Brush
    {
        private int2 _start;
        private int2 _end;
        private bool _isSelecting;
        
        public override void Hover(int2 tile)
        {
            if (_isSelecting) return;

            Preview = new[] { new TilePreview(tile, true, TilePreviewProjection.Default) };

            Debug.Log($"[BrushDebugArea] Hover at {tile}");
        }

        public override void PrimaryDown(int2 tile)
        {
            _isSelecting = true;
            _start = tile;
            _end = tile;

            UpdateAreaPreview();

            Debug.Log($"[BrushDebugArea] PrimaryDown at {tile}");
        }

        public override void PrimaryDrag(int2 tile)
        {
            if (!_isSelecting) return;

            _end = tile;
            UpdateAreaPreview();

            Debug.Log($"[BrushDebugArea] PrimaryDrag at {tile}");
        }

        public override void PrimaryUp(int2 tile)
        {
            if (!_isSelecting) return;

            _end = tile;
            UpdateAreaPreview();
            _isSelecting = false;

            Debug.Log($"[BrushDebugArea] PrimaryUp at {tile}");
            Debug.Log($"[BrushDebugArea] Final area contains {Preview.Count()} tiles");
        }

        public override void Cancel()
        {
            _isSelecting = false;
            Preview = Array.Empty<TilePreview>();

            Debug.Log("[BrushDebugArea] Cancel");
        }

        private void UpdateAreaPreview()
        {
            Preview = RectUtils.GetRectEnumerable(_start, _end)
                .Select(tile => new TilePreview(tile, true, TilePreviewProjection.Default))
                .ToArray();
        }
    }
}