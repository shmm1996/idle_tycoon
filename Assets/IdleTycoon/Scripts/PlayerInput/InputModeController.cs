using System;
using System.Collections.Generic;

namespace IdleTycoon.Scripts.PlayerInput
{
    public sealed class InputModeController
    {
        public enum Mode { None, Camera, Tilemap, UI }
        
        private readonly Dictionary<Mode, IInputSource> _inputs = new();
        private Mode _mode = Mode.None;

        public InputModeController Register(Mode mode, IInputSource input)
        {
            if (mode is Mode.None)
                throw new ArgumentOutOfRangeException($"{nameof(InputModeController)}.{nameof(Register)}: {nameof(mode)} is {nameof(Mode.None)}.");
            
            _inputs.Add(mode, input);
            input.Disable();
            return this;
        }

        public void Switch(Mode mode)
        {
            if (mode == _mode) return;

            if (_mode is not Mode.None) _inputs[_mode].Disable();
            _mode = mode;
            if (_mode is not Mode.None) _inputs[_mode].Enable();
        }
    }
}