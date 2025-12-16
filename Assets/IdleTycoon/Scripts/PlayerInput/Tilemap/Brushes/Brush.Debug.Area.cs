using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using UnityEngine;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public sealed class BrushDebugArea : BrushArea
    {
        public override void Hover(int2 tile)
        {
            base.Hover(tile);

            Debug.Log($"[BrushDebugArea] Hover at {tile}");
        }

        public override void PrimaryDown(int2 tile)
        {
            base.PrimaryDown(tile);

            Debug.Log($"[BrushDebugArea] PrimaryDown at {tile}");
        }

        public override void PrimaryDrag(int2 tile)
        {
            base.PrimaryDrag(tile);

            Debug.Log($"[BrushDebugArea] PrimaryDrag at {tile}");
        }

        public override void PrimaryUp(int2 tile)
        {
            base.PrimaryUp(tile);

            Debug.Log($"[BrushDebugArea] PrimaryUp at {tile}");
            Debug.Log($"[BrushDebugArea] Final area contains {Preview.Length} tiles");
        }

        public override void Cancel()
        {
            base.Cancel();

            Debug.Log("[BrushDebugArea] Cancel");
        }
        
        protected override TilePreview BuildPreview(int2 tile) => new(tile, true, TilePreviewStyle.Default);
    }
}