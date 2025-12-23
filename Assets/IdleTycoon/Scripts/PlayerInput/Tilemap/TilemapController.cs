using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap
{
    public sealed class TilemapController
    {
        private readonly BrushManager _manager;
        private readonly GameSession.Context _context;
        
        public bool IsReady => _manager.Active != null; //TODO: Remove. Do not use for reading player control state.

        public TilemapController(BrushManager manager, GameSession.Context context)
        {
            _manager = manager;
            _context = context;
        }

        public void Process(InputEvent input) //TODO: Make async UniTask.
        {
            if (!_context.WorldMap.HasTile(input.tile)) return; //TODO: Process in brush. Hide or ceil on drag area.
            
            Brush brush = _manager.Active;
            if (brush == null) return;
            
            switch (input.type)
            {
                case InputEvent.Type.Hover: brush.Hover(input.tile); break;
                case InputEvent.Type.Down: brush.Down(input.tile); break;
                case InputEvent.Type.Drag: brush.Drag(input.tile); break;
                case InputEvent.Type.Up: brush.Up(input.tile); break;
                default: return;
            }

            _manager.DetectConfirmationState();
            _manager.NotifyPreviewChanged();
        }
    }
}