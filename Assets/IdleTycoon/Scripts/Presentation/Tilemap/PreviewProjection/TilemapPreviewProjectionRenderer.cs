using System.Collections.Generic;
using System.Linq;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.PreviewProjection;

namespace IdleTycoon.Scripts.Presentation.Tilemap.PreviewProjection
{
    public class TilemapPreviewProjectionRenderer
    {
        private readonly UnityEngine.Tilemaps.Tilemap _tilemap;
        private readonly IDictionary<TilePreviewProjection, TilePreviewDefinition.TileView> _projections;

        public TilemapPreviewProjectionRenderer(UnityEngine.Tilemaps.Tilemap tilemap, TilePreviewDefinition[] projections)
        {
            _tilemap = tilemap;
            _projections = projections.ToDictionary(p => p.Projection, p => p.View);
        }

        public void Render(TilePreview[] previews)
        {
            Clear();
            
            //_tilemap.SetTiles(...)
        }

        public void Clear() => _tilemap.ClearAllTiles();
    }
}