using System;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Serialization.Json.Models
{
    [Serializable]
    public class WorldMap
    {
        public Vector2Int size;
        public Chunk8X8[] chunks;
    }
}