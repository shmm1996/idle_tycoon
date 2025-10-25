using System;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Serialization.Json.Models
{
    [Serializable]
    public class TileEntity
    {
        public int id;
        public Vector2Int position;
    }
}