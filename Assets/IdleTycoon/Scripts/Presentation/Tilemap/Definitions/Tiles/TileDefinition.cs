using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.CustomEditor.Attributes;
using IdleTycoon.Scripts.Data.Enums;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles
{
    public abstract class TileDefinition : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("name")] protected string tileName;
        
        public string Name => tileName;
        
        [SerializeField] protected TileView[] tiles;
        
        public IEnumerable<TileView> Tiles => tiles;
        
        [Serializable]
        public struct TileView
        {
            public TileBase tile;
            public byte weight;
            public Transformation transformation;
            
            [Serializable]
            public struct Transformation
            {
                [SerializeField, FormerlySerializedAs("rotation"), EnumFlags]
                private TileTransformation.Rotation rotationFlags;

                [SerializeField, FormerlySerializedAs("flip"), EnumFlags]
                private TileTransformation.Flip flipFlags;

                public int RotationFlags => (int)rotationFlags & 0b1111;
                
                public int FlipFlags => (int)flipFlags & 0b111;
            }
        }
    }
    
    public abstract class TileDefinition<TData> : TileDefinition
        where TData : struct
    {
        [SerializeField] protected TData data;
        
        public TData Data => data;
    }
}