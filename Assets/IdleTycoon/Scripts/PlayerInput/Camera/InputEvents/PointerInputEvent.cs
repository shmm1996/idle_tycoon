using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Camera.InputEvents
{
    public readonly struct PointerInputEvent
    {
        public readonly float2 screen;
        public readonly float unscaledTime;
        
        public enum Type { Down, Dragging, Up }
        public readonly Type type;

        public PointerInputEvent(Type type, float2 screen, float unscaledTime)
        {
            this.type = type;
            this.screen = screen;
            this.unscaledTime = unscaledTime;
        }
    }
}