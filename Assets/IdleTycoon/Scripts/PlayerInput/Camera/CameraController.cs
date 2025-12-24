using IdleTycoon.Scripts.PlayerInput.Camera.InputEvents;
using Unity.Mathematics;
using UnityEngine;

namespace IdleTycoon.Scripts.PlayerInput.Camera
{
    public sealed class CameraController
    {
        private const float DeadZonePixels = 6f; //TODO: move to global constants.
        private const float DeadZonePixelsSq = DeadZonePixels * DeadZonePixels;
        private const float DragAfterTimeSeconds = 0.08f;
        private const float TinyMovePixels = 1f;
        private const float TinyMoveSq = TinyMovePixels * TinyMovePixels;
        private const float ReleaseOnDistance = 0.05f;
        private const float ReleaseOnDistanceSq = ReleaseOnDistance * ReleaseOnDistance;
        
        private const float FollowSharpness = 35f;
        
        private readonly UnityEngine.Camera _camera;
        private readonly Transform _transform;

        private enum State { Idle, Pressing, Drag, Inertia }
        private State _state;
        private float2 _pressScreen;
        private float _pressTime;
        private float2 _cursorScreen;
        private Vector3 _anchorWorldUnderCursor;
        private Vector3 _dragStartCameraPosition;
        private Vector3 _targetPosition;
        
        public CameraController(UnityEngine.Camera camera)
        {
            _camera = camera;
            _transform = camera.transform;
        }

        public void Process(PointerInputEvent pointerInput) //TODO: Make async UniTask.
        {
            switch (pointerInput.type)
            {
                case PointerInputEvent.Type.Down when _state is State.Idle:
                    _state = State.Pressing;
                    _pressScreen = pointerInput.screen;
                    _pressTime = pointerInput.unscaledTime;
                    break;
                case PointerInputEvent.Type.Dragging
                    when _state is State.Pressing && 
                         ((pointerInput.unscaledTime - _pressTime > DragAfterTimeSeconds && math.lengthsq(_pressScreen - pointerInput.screen) > TinyMoveSq) ||
                          math.lengthsq(_pressScreen - pointerInput.screen) > DeadZonePixelsSq):
                    _state = State.Drag;
                    _cursorScreen = pointerInput.screen;
                    _anchorWorldUnderCursor = ScreenToWorld(pointerInput.screen);
                    _dragStartCameraPosition = _transform.position;
                    UpdateTargetPosition();
                    break;
                case PointerInputEvent.Type.Dragging when _state is State.Drag:
                    _cursorScreen = pointerInput.screen;
                    break;
                case PointerInputEvent.Type.Up when _state is State.Pressing: 
                    _state = State.Idle; 
                    break;
                case PointerInputEvent.Type.Up when _state is State.Drag: 
                    _state = State.Inertia; 
                    UpdateTargetPosition();
                    break;
            }
        }

        public void Update(float deltaTime)
        {
            if (_state is not (State.Drag or State.Inertia)) return;
            
            if (_state is State.Drag)
                UpdateTargetPosition();

            float a = 1f - Mathf.Exp(-FollowSharpness * deltaTime);
            Vector3 nextPosition = Vector3.LerpUnclamped(_transform.position, _targetPosition, a);
            _transform.position = nextPosition;

            if (_state is State.Inertia && Vector3.SqrMagnitude(nextPosition - _targetPosition) < ReleaseOnDistanceSq)
                _state = State.Idle;
        }

        private void UpdateTargetPosition()
        {
            Vector3 world = ScreenToWorld(_cursorScreen);
            Vector3 cameraDelta = _dragStartCameraPosition - _transform.position;
            Vector3 worldNowAtDragStartCamera = world + cameraDelta;
            Vector3 deltaWorld = worldNowAtDragStartCamera - _anchorWorldUnderCursor;
                
            _targetPosition = _dragStartCameraPosition - deltaWorld;
            _targetPosition.z = _dragStartCameraPosition.z;
        }
        
        private Vector3 ScreenToWorld(float2 screen) => _camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0f));
    }
}