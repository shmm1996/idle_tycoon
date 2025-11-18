using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Commands;

namespace IdleTycoon.Scripts.Data.Session
{
    public sealed class CommandsQueue
    {
        private readonly Queue<IGameCommand> _commands = new();
        
        public int Count => _commands.Count;
        
        public void Enqueue(IGameCommand command) => _commands.Enqueue(command);

        public bool TryDequeue(out IGameCommand command) => _commands.TryDequeue(out command);
        
        public Enqueuer GetEnqueuer() => new(this);
        
        public Dequeuer GetDequeuer() => new(this);

        public sealed class Enqueuer
        {
            private readonly CommandsQueue _commands;
            
            public int Count => _commands.Count;
            
            public Enqueuer(CommandsQueue commands) => _commands = commands;
            
            public void Enqueue(IGameCommand command) => _commands.Enqueue(command);
        }
        
        public sealed class Dequeuer
        {
            private readonly CommandsQueue _commands;
            
            public int Count => _commands.Count;
            
            public Dequeuer(CommandsQueue commands) => _commands = commands;
            
            public bool TryDequeue(out IGameCommand command) => _commands.TryDequeue(out command);
        }
    }
}