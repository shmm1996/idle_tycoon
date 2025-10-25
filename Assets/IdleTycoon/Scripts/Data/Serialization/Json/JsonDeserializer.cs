using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Enums;
using IdleTycoon.Scripts.Data.Serialization.Json.Models;
using IdleTycoon.Scripts.Utils;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.Data.Serialization.Json
{
    public static class JsonDeserializer
    {
        public static unsafe Session.WorldMap* DeserializeWorldMap(string json)
        {
            var worldMap = JsonUtility.FromJson<WorldMap>(json);

            Session.WorldMap* sessionWorldMap = AllocWorldMap(worldMap.size.ToInt2());
            foreach (Chunk8X8 chunk in worldMap.chunks)
            {
                SetChunkTileAttributeFlags(sessionWorldMap, chunk);
            }
            
            return sessionWorldMap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe Session.WorldMap* AllocWorldMap(int2 size)
        {
            var worldMap = (Session.WorldMap*)UnsafeUtility.Malloc(sizeof(Session.WorldMap), 64, Allocator.Persistent);
            *worldMap = new Session.WorldMap(size);
            
            return worldMap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void SetChunkTileAttributeFlags(Session.WorldMap* sessionWorldMap, Chunk8X8 chunk)
        {
            ref Session.Chunk8X8 sessionChunk = ref sessionWorldMap->GetChunk(chunk.position.ToInt2());
            foreach (Chunk8X8TileAttributeFlags chunkTileAttribute in chunk.tileAttributeFlags)
            {
                if (!IsSerializable((TileAttributeFlag)chunkTileAttribute.id)) continue;
                sessionChunk.SetChunkTileAttributeFlag(chunkTileAttribute.id, chunkTileAttribute.value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSerializable(TileAttributeFlag flag) => flag == TileAttributeFlag.Ground;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void SetChunkTileEntities(Session.WorldMap* sessionWorldMap, TileEntity[] entities)
        {

        }
    }
}