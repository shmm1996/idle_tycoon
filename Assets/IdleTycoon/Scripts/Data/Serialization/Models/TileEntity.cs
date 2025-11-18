using System;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Serialization.Models
{
    [Serializable]
    public sealed class TileEntity
    {
        public int id;
        public Vector2Int position;
    }
}