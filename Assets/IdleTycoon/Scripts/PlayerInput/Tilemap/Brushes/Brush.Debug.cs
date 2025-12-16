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
            UpdatePreview(tile);
            
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

            ClearPreview();
            
            Debug.Log("[BrushDebug] Cancel()");
        }

        protected override TilePreview BuildPreview(int2 tile) => new(tile, true, TilePreviewStyle.Default);

        private void UpdatePreview(int2 tile)
        {
            TilePreview preview = BuildPreview(tile);

            if (Preview.Length == 1)
                Preview[0] = preview;
            else
                Preview = new[] { preview };
        }

        private void ClearPreview() =>
            Preview = Array.Empty<TilePreview>();
    }
}