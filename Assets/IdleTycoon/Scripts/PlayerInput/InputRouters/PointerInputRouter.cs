using System;
using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.PlayerInput.Camera;
using IdleTycoon.Scripts.PlayerInput.Tilemap;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TilemapInputEvent = IdleTycoon.Scripts.PlayerInput.Tilemap.InputEvent;

namespace IdleTycoon.Scripts.PlayerInput.InputRouters
{
    public sealed class PointerInputRouter : IDisposable
    {
        private const float TileSize = 1.0f; //TODO: move to global constants.
        private const float CameraToTilemapTransitionTime = 0.2f;
        private const float CameraDragDeadZonePixels = 6f;
        
        private readonly UnityEngine.Camera _camera;
        
        private readonly InputAction _pointerPosition;
        private readonly InputAction _pointerPressed;
        
        private readonly CameraController _cameraController;
        private readonly TilemapController _tilemapController;

        private bool _held;
        private float _heldAt;
        private int2 _tile;
        
        private enum Lock { None, Tilemap, Camera, UI }
        private Lock _lock = Lock.None;

        public PointerInputRouter(
            UnityEngine.Camera camera, 
            InputAction pointerPosition, 
            InputAction pointerPressed, 
            CameraController cameraController,
            TilemapController tilemapController)
        {
            _camera = camera;
            
            _pointerPosition = pointerPosition;
            _pointerPressed = pointerPressed;
            
            _cameraController = cameraController;
            _tilemapController = tilemapController;

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
            if (!Application.isFocused) return;

            bool press = _pointerPressed.ReadValue<float>() > 0.5f;
            float2 screen = _pointerPosition.ReadValue<Vector2>();

            ResolveOnUIInput(press);
            ResolveOnCameraInput(press, screen);
            ResolveOnTileInput(press, screen);
        }

        private void ResolveOnUIInput(bool press)
        {
            switch (_lock)
            {
                case Lock.None when !_held && press && IsPointerOverUI():
                    _held = true;
                    _lock = Lock.UI;
                    break;
                case Lock.UI when _held && !press:
                    _held = false;
                    _lock = Lock.None;
                    break;
            }
        }

        private bool IsPointerOverUI() => EventSystem.current && EventSystem.current.IsPointerOverGameObject();

        private void ResolveOnCameraInput(bool press, float2 screen)
        {
            switch (_lock)
            {
                case Lock.None:
                    break;
                case Lock.Camera:
                    break;
            }
        }

        private void ResolveOnTileInput(bool press, float2 screen)
        {
            if (_lock is not (Lock.None or Lock.Tilemap) ||
                !TryGetTileUnderMouse(screen, out int2 tile)) return;

            bool isTileChanged = !tile.Equals(_tile);
            switch (_lock)
            {
                case Lock.None when !_held && !press && isTileChanged:
                    _tile = tile;
                    _tilemapController.Process(new TilemapInputEvent(TilemapInputEvent.Type.Hover, tile));
                    break;
                case Lock.None when !_held && press:
                    _held = true;
                    _lock = Lock.Tilemap;
                    _tilemapController.Process(new TilemapInputEvent(TilemapInputEvent.Type.Down, tile));
                    break;
                case Lock.Tilemap when _held && press && isTileChanged:
                    _tile = tile;
                    _tilemapController.Process(new TilemapInputEvent(TilemapInputEvent.Type.Drag, tile));
                    break;
                case Lock.Tilemap when _held && !press:
                    _held = false;
                    _tilemapController.Process(new TilemapInputEvent(TilemapInputEvent.Type.Up, tile));
                    _lock = Lock.None;
                    break;
            }
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