using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap
{
    public readonly struct InputEvent
    {
        public readonly Type type;
        public readonly int2 tile;

        public InputEvent(Type type, int2 tile)
        {
            this.type = type;
            this.tile = tile;
        }
        
        public enum Type
        {
            Hover,
            PrimaryDown,
            PrimaryDrag,
            PrimaryUp
        }
    }
}