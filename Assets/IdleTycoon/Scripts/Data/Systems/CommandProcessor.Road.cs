using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Commands;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Systems
{
    public unsafe partial class CommandProcessor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnRoadSet(RoadCommand.Set command)
        {
            int2 tile = command.tile;
            
            if (!_worldMap->TrySetRoad(tile, out ulong flags)) return;
            
            //TODO: scan flags and mark for validation;
            
            _session.ToUpdate(tile);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnRoadRemove(RoadCommand.Remove command)
        {
            int2 tile = command.tile;
            
            if (!_worldMap->TryRemoveRoad(tile, out ulong flags)) return;
            
            //TODO: scan flags and mark for validation;
            
            _session.ToUpdate(tile);
        }
    }
}