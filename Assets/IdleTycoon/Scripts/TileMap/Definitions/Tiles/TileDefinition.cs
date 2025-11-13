using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.TileMap.Definitions.Tiles
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
        }
    }
    
    public abstract class TileDefinition<TData> : TileDefinition
        where TData : struct
    {
        [SerializeField] protected TData data;
        
        public TData Data => data;
    }
}