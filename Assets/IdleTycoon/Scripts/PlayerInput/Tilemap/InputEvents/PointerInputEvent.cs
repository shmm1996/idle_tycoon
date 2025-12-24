using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.InputEvents
{
    public readonly struct PointerInputEvent
    {
        public readonly int2 tile;
        
        public enum Type { Hover, Down, Drag, Up }
        public readonly Type type;

        public PointerInputEvent(Type type, int2 tile)
        {
            this.type = type;
            this.tile = tile;
        }
    }
}