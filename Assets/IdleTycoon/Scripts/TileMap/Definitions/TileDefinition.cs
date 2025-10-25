using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts.TileMap.Definitions
{
    public abstract class TileDefinition<TData> : ScriptableObject
        where TData : struct
    {
        [SerializeField, FormerlySerializedAs("name")] protected string tileName;
        
        [SerializeField] protected TileView[] tiles;
        
        [SerializeField] protected TData data;
        
        [Serializable]
        public struct TileView
        {
            public TileBase tile;
            public byte weight;
        }
    }
}