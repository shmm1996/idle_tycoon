using System;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using UnityEngine;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public sealed class BrushDebug : Brush
    {
        private bool _pressed;
        
        public override void Hover(int2 tile)
        {
            UpdateAreaPreview(tile);
            
            Debug.Log($"[BrushDebug] Hover at {tile}");
        }

        public override void PrimaryDown(int2 tile)
        {
            _pressed = true;
            
            Debug.Log($"[BrushDebug] PrimaryDown at {tile}");
        }

        public override void PrimaryDrag(int2 tile)
        {
            if (!_pressed) return;

            Debug.Log($"[BrushDebug] PrimaryDrag at {tile}");
        }

        public override void PrimaryUp(int2 tile)
        {
            if (!_pressed) return;

            _pressed = false;
            
            Debug.Log($"[BrushDebug] PrimaryUp at {tile}");
        }

        public override void Cancel()
        {
            _pressed = false;

            ClearAreaPreview();
            
            Debug.Log("[BrushDebug] Cancel()");
        }

        private void UpdateAreaPreview(int2 tile)
        {
            TilePreview preview = new(tile, true, TilePreviewStyle.Default);

            if (Preview.Length == 1)
                Preview[0] = preview;
            else
                Preview = new[] { preview };
        }

        private void ClearAreaPreview() =>
            Preview = Array.Empty<TilePreview>();
    }
}