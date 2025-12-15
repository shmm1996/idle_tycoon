using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.PreviewProjection;
using IdleTycoon.Scripts.Utils;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.PreviewProjection
{
    public class TilemapPreviewRenderer : IDisposable
    {
        private readonly BrushManager _brushManager;
        private readonly UnityEngine.Tilemaps.Tilemap _tilemap;
        private readonly IDictionary<(TilePreviewStyle, bool), TilePreviewDefinition.TileView> _tileViews;
        
        private readonly CompositeDisposable _disposables = new();
        
        public TilemapPreviewRenderer(UnityEngine.Tilemaps.Tilemap tilemap, TilePreviewDefinition[] definitions, BrushManager brushManager)
        {
            _tilemap = tilemap;
            _tileViews = definitions.ToDictionary(p => (Projection: p.Style, p.IsValid), p => p.View);
            _brushManager = brushManager;
            
            _brushManager.OnPreviewUpdated
                .Subscribe(_ => Render())
                .AddTo(_disposables);
        }
        
        public void Dispose() => _disposables.Dispose();
        
        private void Render()
        {
            _tilemap.ClearAllTiles();

            TilePreview[] previews = _brushManager.Active?.Preview;
            if (previews == null || previews.Length == 0) return;

            var tiles = new TileBase[previews.Length];
            var positions = new Vector3Int[previews.Length];
            var transforms = new Matrix4x4[previews.Length];
            for (int i = 0; i < previews.Length; i++)
            {
                TilePreviewDefinition.TileView view = GetTileView(previews[i]);
                tiles[i] = view.tile;
                positions[i] = previews[i].tile.ToVector3Int();
                transforms[i] = view.transformation.rotation.ToMatrix() * view.transformation.flip.ToMatrix();
            }
            
            _tilemap.SetTiles(positions, tiles);
            for (int i = 0; i < previews.Length; i++)
            {
                /*
                TilePreviewDefinition.TileView view = GetTileView(previews[i]);
                if (view.color != Color.white) 
                    _tilemap.SetColor(positions[i], view.color);
                    */
                if (transforms[i] == Matrix4x4.identity) continue;
                _tilemap.SetTransformMatrix(positions[i], transforms[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TilePreviewDefinition.TileView GetTileView(TilePreview preview) => _tileViews[(preview.style, preview.isValid)];
    }
}