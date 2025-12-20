using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Camera
{
    public readonly struct InputEvent
    {
        public readonly float2 position;
        
        public enum Type { Down, Drag, Up }
        public readonly Type type;

        public InputEvent(float2 position, Type type)
        {
            this.position = position;
            this.type = type;
        }
    }
}