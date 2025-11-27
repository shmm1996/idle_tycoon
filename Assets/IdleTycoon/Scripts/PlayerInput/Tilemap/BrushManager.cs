using System.Collections.Generic;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes;
using JetBrains.Annotations;
using R3;

namespace IdleTycoon.Scripts.PlayerInput.Tilemap
{
    public sealed class BrushManager
    {
        private readonly Dictionary<string, Brush> _brushes = new();

        private readonly Subject<Unit> _onConfirmationRequested = new();
        private readonly Subject<Unit> _onConfirmationCleared = new();
        
        [CanBeNull] public Brush Active { get; private set; }

        public Observable<Unit> OnConfirmationRequested => _onConfirmationRequested;
        public Observable<Unit> OnConfirmationCleared => _onConfirmationCleared;

        public void Register(string name, Brush brush) => _brushes[name] = brush;

        public bool Activate(string name)
        {
            if (!_brushes.TryGetValue(name, out Brush brush)) return false;

            Active?.Cancel();
            Active = brush;

            DetectConfirmationState();
            
            return true;
        }

        public void ResetActive()
        {
            Active?.Cancel();
            Active = null;
            _onConfirmationCleared.OnNext(Unit.Default);
        }

        public bool TryConfirm()
        {
            if (Active is not ConfirmableBrush c || !c.TryApply()) return false;
                
            Active.Cancel();
            _onConfirmationCleared.OnNext(Unit.Default);
                
            return true;
        }

        public void DetectConfirmationState()
        {
            if (Active is ConfirmableBrush c)
            {
                if (c.NeedsConfirmation) 
                    _onConfirmationRequested.OnNext(Unit.Default);
                else 
                    _onConfirmationCleared.OnNext(Unit.Default);
            }
            else
            {
                _onConfirmationCleared.OnNext(Unit.Default);
            }
        }
    }
}