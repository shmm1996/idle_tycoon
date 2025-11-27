namespace IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes
{
    public abstract class ConfirmableBrush : Brush
    {
        public bool NeedsConfirmation { get; protected set; } = false;
        
        public abstract bool TryApply();
    }
}