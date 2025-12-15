using System;
using IdleTycoon.Scripts.CustomEditor.Attributes;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.PreviewProjection
{
    [CreateAssetMenu(fileName = "TilePreview (N)", menuName = "Definitions/TileMap/TilePreviews", order = 0)]
    public class TilePreviewDefinition : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("name")] private string tileName;
        
        public string Name => tileName;
        
        [SerializeField] private TilePreviewStyle style;
        
        public TilePreviewStyle Style => style;
        
        [SerializeField] private bool isValid;
        
        public bool IsValid => isValid;
        
        [SerializeField] protected TileView view;
        
        public TileView View => view;
        
        [Serializable]
        public struct TileView
        {
            public TileBase tile;
            public Transformation transformation;
            //public Color color;
            
            [Serializable]
            public struct Transformation
            {
                public TileTransformation.Rotation rotation;
                public TileTransformation.Flip flip;
            }
        }
    }
}