using IdleTycoon.Scripts.Data.Commands;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Enums;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Models;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public class BrushCleanArea : BrushArea
    {
        private readonly GameSession.CommandsBus _commands;

        public BrushCleanArea(GameSession.CommandsBus commands)
        {
            _commands = commands;
        }

        public override void Up(int2 tile)
        {
            foreach (TilePreview tilePreview in Preview)
            {
                _commands.OnNext(new RoadCommand.Remove(tilePreview.tile));
                //TODO: OnNext() with Command.Remove for each constructions.
            }
            
            base.Up(tile);
        }
        
        protected override TilePreview BuildPreview(int2 tile) =>
            new(tile, true, TilePreviewStyle.Default);
    }
}