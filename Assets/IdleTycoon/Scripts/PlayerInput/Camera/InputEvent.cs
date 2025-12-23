using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Camera
{
    public readonly struct InputEvent
    {
        public readonly float2 screen;
        public readonly float unscaledTime;
        
        public enum Type { Down, Dragging, Up }
        public readonly Type type;

        public InputEvent(Type type, float2 screen, float unscaledTime)
        {
            this.type = type;
            this.screen = screen;
            this.unscaledTime = unscaledTime;
        }
    }
}