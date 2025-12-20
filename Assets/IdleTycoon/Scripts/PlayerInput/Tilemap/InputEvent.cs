using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap
{
    public readonly struct InputEvent
    {
        public readonly int2 tile;
        
        public enum Type { Hover, Down, Drag, Up }
        public readonly Type type;

        public InputEvent(Type type, int2 tile)
        {
            this.type = type;
            this.tile = tile;
        }
    }
}