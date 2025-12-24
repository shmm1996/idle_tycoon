using System;
using IdleTycoon.Scripts.PlayerInput.Camera.InputEvents;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdleTycoon.Scripts.PlayerInput.Camera.InputSources
{
    public sealed class PointerInputSource : IInputSource
    {
        [Serializable]
        public class InputActions
        {
            [SerializeField] private InputActionReference point;
            [SerializeField] private InputActionReference press;
            
            public InputAction Point => point;
            public InputAction Press => press;
        }
        
        private readonly CameraController _camera;
        private readonly InputAction _point;
        private readonly InputAction _press;

        private bool _pressed;

        public PointerInputSource(CameraController camera, InputActions input)
        {
            _camera = camera;
            _point = input.Point;
            _press = input.Press;
        }

        public void Enable()
        {
            _point.performed += OnPoint;
            _press.started += OnPressDown;
            _press.canceled += OnPressUp;

            _point.Enable();
            _press.Enable();
        }

        public void Disable()
        {
            _point.performed -= OnPoint;
            _press.started -= OnPressDown;
            _press.canceled -= OnPressUp;

            _point.Disable();
            _press.Disable();

            _pressed = false;
        }
        
        private void OnPoint(InputAction.CallbackContext callback)
        {
            if (!_pressed) return;

            float2 screen = callback.ReadValue<Vector2>();

            _camera.Process(new PointerInputEvent(PointerInputEvent.Type.Dragging, screen, Time.unscaledTime));
        }
        
        private void OnPressDown(InputAction.CallbackContext _)
        {
            _pressed = true;

            float2 screen = _point.ReadValue<Vector2>();

            _camera.Process(new PointerInputEvent(PointerInputEvent.Type.Down, screen, Time.unscaledTime));
        }

        private void OnPressUp(InputAction.CallbackContext _)
        {
            if (!_pressed) return;

            _pressed = false;
            
            float2 screen = _point.ReadValue<Vector2>();

            _camera.Process(new PointerInputEvent(PointerInputEvent.Type.Up, screen, Time.unscaledTime));
        }
    }
}