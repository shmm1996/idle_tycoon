using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Commands;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Systems
{
    public unsafe partial class CommandProcessor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnTerrainGroundSet(TerrainCommand.Ground.Set command)
        {
            int2 tile = command.tile;
            
            if (!_worldMap->TrySetTerrainGround(tile, out ulong flags)) return;
            
            //TODO: scan flags and mark for validation;
            
            _session.ToUpdate(tile);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnTerrainGroundRemove(TerrainCommand.Ground.Remove command)
        {
            int2 tile = command.tile;
            
            if (!_worldMap->TryRemoveTerrainGround(tile, out ulong flags)) return;
            
            //TODO: scan flags and mark for validation;
            
            _session.ToUpdate(tile);
        }
    }
}