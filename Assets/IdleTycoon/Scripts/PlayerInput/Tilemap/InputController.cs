using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap
{
    public sealed class InputController
    {
        private readonly BrushManager _manager;
        private readonly GameSession.Context _context;
        private int2 _lastTile;

        public InputController(BrushManager manager, GameSession.Context context)
        {
            _manager = manager;
            _context = context;
        }

        public void Process(InputEvent evt)
        {
            if (!_context.WorldMap.HasTile(evt.tile)) return;
            
            Brush brush = _manager.Active;
            if (brush == null) return;

            if (evt.type == InputEvent.Type.Hover)
            {
                if (evt.tile.Equals(_lastTile)) return;
                
                _lastTile = evt.tile;
                brush.Hover(evt.tile);
                _manager.DetectConfirmationState();
                
                return;
            }

            switch (evt.type)
            {
                case InputEvent.Type.PrimaryDown: brush.PrimaryDown(evt.tile); break;
                case InputEvent.Type.PrimaryDrag: brush.PrimaryDrag(evt.tile); break;
                case InputEvent.Type.PrimaryUp: brush.PrimaryUp(evt.tile); break;
            }

            _manager.DetectConfirmationState();
        }
    }
}