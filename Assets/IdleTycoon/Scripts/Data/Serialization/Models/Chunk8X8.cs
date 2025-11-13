using System;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Serialization.Models
{
    [Serializable]
    public class Chunk8X8
    {
        public Vector2Int position;
        public Chunk8X8TileAttributeFlags[] tileAttributeFlags;
        public TileEntity[] tileEntities;
    }
}