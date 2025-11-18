using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.CustomEditor.Attributes;
using IdleTycoon.Scripts.Data.Enums;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles
{
    public abstract class TileDefinitionBase<TTileView> : ScriptableObject
        where TTileView : struct
    {
        [SerializeField, FormerlySerializedAs("name")] protected string tileName;
        
        public string Name => tileName;
        
        [SerializeField] protected TTileView[] tiles;
        
        public IEnumerable<TTileView> Tiles => tiles;
    }
    
    public abstract class TileDefinition : TileDefinitionBase<TileDefinition.TileView>
    {
        [Serializable]
        public struct TileView
        {
            public TileBase tile;
            public int weight;
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
    
    public abstract class PartedTileDefinition : TileDefinitionBase<PartedTileDefinition.PartedTileView>
    {
        [Serializable]
        public struct PartedTileView
        {
            public SubTileView[] views;
            public int weight;
        }
        
        [Serializable]
        public struct SubTileView
        {
            public TileBase tile;
            public int2 offset;
            public int weight;
            public Transformation transformation;
            
            [Serializable]
            public struct Transformation
            {
                public TileTransformation.Rotation rotation;
                public TileTransformation.Flip flip;
            }
        }
    }
}