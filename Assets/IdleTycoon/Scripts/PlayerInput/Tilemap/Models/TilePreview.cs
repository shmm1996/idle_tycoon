using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Models
{
    public readonly struct TilePreview
    {
        public readonly int2 tile;
        public readonly bool isValid;
        public readonly TilePreviewProjection projection;

        public TilePreview(int2 tile, bool isValid, TilePreviewProjection projection)
        {
            this.tile = tile;
            this.isValid = isValid;
            this.projection = projection;
        }
    }
}