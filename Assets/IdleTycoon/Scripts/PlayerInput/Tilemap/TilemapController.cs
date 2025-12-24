using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes;
using IdleTycoon.Scripts.PlayerInput.Tilemap.InputEvents;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap
{
    public sealed class TilemapController
    {
        private readonly BrushManager _manager;
        private readonly GameSession.Context _context;

        public TilemapController(BrushManager manager, GameSession.Context context)
        {
            _manager = manager;
            _context = context;
        }

        public void Process(PointerInputEvent pointerInput) //TODO: Make async UniTask.
        {
            if (!_context.WorldMap.HasTile(pointerInput.tile)) return; //TODO: Process in brush. Hide or ceil on drag area.
            
            Brush brush = _manager.Active;
            if (brush == null) return;
            
            switch (pointerInput.type)
            {
                case PointerInputEvent.Type.Hover: brush.Hover(pointerInput.tile); break;
                case PointerInputEvent.Type.Down: brush.Down(pointerInput.tile); break;
                case PointerInputEvent.Type.Drag: brush.Drag(pointerInput.tile); break;
                case PointerInputEvent.Type.Up: brush.Up(pointerInput.tile); break;
                default: return;
            }

            _manager.DetectConfirmationState();
            _manager.NotifyPreviewChanged();
        }
    }
}