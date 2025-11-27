using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.InputSources
{
    public sealed class MouseInputSource
    {
        private readonly Camera _camera;
        private readonly InputController _controller;

        private readonly InputAction _pointerPosition;
        private readonly InputAction _pointerPressed;

        private bool _primaryHeld;
        private int2 _lastTile;

        public MouseInputSource(
            Camera camera, 
            InputController controller, 
            InputAction pointerPosition,
            InputAction pointerPressed)
        {
            _camera = camera;
            _controller = controller;

            _pointerPosition = pointerPosition;
            _pointerPressed = pointerPressed;

            _pointerPosition.Enable();
            _pointerPressed.Enable();
        }

        public void Dispose()
        {
            _pointerPosition.Disable();
            _pointerPressed.Disable();
        }

        public void Update()
        {
            if (!IsReady()) return;
            
            int2 tile = GetTileUnderMouse();

            _controller.Process(new InputEvent(InputEvent.Type.Hover, tile));

            var press = _pointerPressed.ReadValue<float>();
            switch (_primaryHeld)
            {
                case false when press > 0.5f:
                    _primaryHeld = true;
                    _controller.Process(new InputEvent(InputEvent.Type.PrimaryDown, tile));
                    break;
                case true when press > 0.5f && !tile.Equals(_lastTile):
                    _controller.Process(new InputEvent(InputEvent.Type.PrimaryDrag, tile));
                    break;
                case true when press <= 0.5f:
                    _primaryHeld = false;
                    _controller.Process(new InputEvent(InputEvent.Type.PrimaryUp, tile));
                    break;
            }

            _lastTile = tile;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsReady() =>
            Application.isFocused &&
            _camera.pixelRect.Contains(_pointerPosition.ReadValue<Vector2>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int2 GetTileUnderMouse()
        {
            Vector2 screen = _pointerPosition.ReadValue<Vector2>();
            Vector3 world = _camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0f));
            int2 tile = new(Mathf.FloorToInt(world.x), Mathf.FloorToInt(world.y));
            
            return tile;
        }
    }
}