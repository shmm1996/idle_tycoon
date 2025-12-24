using System;
using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.PlayerInput.Tilemap.InputEvents;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.InputSources
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
        
        private const float TileSize = 1.0f; //TODO: move to global constants.
        
        private readonly UnityEngine.Camera _camera;
        private readonly TilemapController _tilemap;
        private readonly InputAction _point;
        private readonly InputAction _press;

        private bool _pressed;
        private int2 _lastTile;

        public PointerInputSource(UnityEngine.Camera camera, TilemapController tilemap, InputActions input)
        {
            _camera = camera;
            _tilemap = tilemap;
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
            float2 screen = callback.ReadValue<Vector2>();
            
            if (!TryGetTileUnderMouse(screen, out int2 tile) || tile.Equals(_lastTile)) return;
            
            _tilemap.Process(new PointerInputEvent(_pressed ? PointerInputEvent.Type.Drag : PointerInputEvent.Type.Hover, tile));
            
            _lastTile = tile;
        }
        
        private void OnPressDown(InputAction.CallbackContext _)
        {
            _pressed = true;
            
            float2 screen = _point.ReadValue<Vector2>();
            
            if (!TryGetTileUnderMouse(screen, out int2 tile)) return;
            
            _tilemap.Process(new PointerInputEvent(PointerInputEvent.Type.Down, tile));
            
            _lastTile = tile;
        }

        private void OnPressUp(InputAction.CallbackContext _)
        {
            if (!_pressed) return;

            _pressed = false;
            
            float2 screen = _point.ReadValue<Vector2>();

            if (!TryGetTileUnderMouse(screen, out int2 tile)) 
                tile = _lastTile;
            
            _tilemap.Process(new PointerInputEvent(PointerInputEvent.Type.Up, tile));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetTileUnderMouse(float2 screen, out int2 tile)
        {
            if (!_camera.pixelRect.Contains(screen))
            {
                tile = default;
                return false;
            }
            
            Vector3 world = _camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0f));
            tile = new int2(Mathf.FloorToInt(world.x / TileSize), Mathf.FloorToInt(world.y / TileSize));
            
            return true;
        }
    }
}