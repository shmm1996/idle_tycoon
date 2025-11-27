using System;
using IdleTycoon.Scripts.CustomEditor.Attributes;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.PreviewProjection
{
    [CreateAssetMenu(fileName = "TilePreviewProjection(N)", menuName = "Definitions/TileMap/TilesPreviewProjection", order = 0)]
    public class TilePreviewDefinition : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("name")] private string tileName;
        
        public string Name => tileName;
        
        [SerializeField] private TilePreviewProjection projection;
        
        public TilePreviewProjection Projection => projection;
        
        [SerializeField] protected TileView view;
        
        public TileView View => view;
        
        [Serializable]
        public struct TileView
        {
            public TileBase tile;
            public Transformation transformation;
            
            [Serializable]
            public struct Transformation
            {
                [SerializeField, FormerlySerializedAs("rotation"), EnumFlags]
                private TileTransformation.Rotation rotationFlags;

                [SerializeField, FormerlySerializedAs("flip"), EnumFlags]
                private TileTransformation.Flip flipFlags;

                public int RotationFlags => (int)rotationFlags & 0b1111;
                
                public int FlipFlags => (int)flipFlags & 0b1111;
            }
        }
    }
}