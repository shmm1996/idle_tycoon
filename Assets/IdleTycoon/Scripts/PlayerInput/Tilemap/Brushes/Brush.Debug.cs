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
            Debug.Log($"[BrushDebug] Hover at {tile}");

            Preview = new[] { new TilePreview(tile, true, TilePreviewProjection.Default) };
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
            Debug.Log("[BrushDebug] Cancel()");
            _pressed = false;

            Preview = Array.Empty<TilePreview>();
        }
    }
}