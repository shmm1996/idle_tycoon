using UnityEngine;

namespace IdleTycoon.Scripts.PlayerInput.Camera
{
    public sealed class CameraController
    {
        private readonly Transform _camera;

        public CameraController(Transform camera)
        {
            _camera = camera;
        }

        public void Process(InputEvent input) //TODO: Make async UniTask.
        {
            switch (input.type)
            {
                case InputEvent.Type.Down:
                    break;
                case InputEvent.Type.Drag:
                    break;
                case InputEvent.Type.Up:
                    break;
            }
        }
    }
}