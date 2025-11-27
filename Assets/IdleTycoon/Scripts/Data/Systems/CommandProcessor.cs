using System;
using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Commands;
using IdleTycoon.Scripts.Data.Session;
using R3;

namespace IdleTycoon.Scripts.Data.Systems
{
    public unsafe partial class CommandProcessor : ISystem, IDisposable
    {
        private readonly GameSession _session;
        private readonly CompositeDisposable _disposables;
        private readonly Queue<IGameCommand> _commands;
        private WorldMap* _worldMap;
        
        public CommandProcessor(GameSession session)
        {
            _session = session;
            _disposables = new CompositeDisposable();
            
            _commands = new Queue<IGameCommand>();
            
            session.onGameCommand.Subscribe(OnGameCommand).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

        private void OnGameCommand(IGameCommand command) => _commands.Enqueue(command);

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void OnTick()
        {
            _worldMap = _session.WorldMap;
            while (_commands.TryDequeue(out IGameCommand command))
            {
                switch (command)
                {
                    case TerrainCommand.Ground.Set c: OnTerrainGroundSet(c); break;
                    case TerrainCommand.Ground.Remove c: OnTerrainGroundRemove(c); break;
                    case RoadCommand.Set c: OnRoadSet(c); break;
                    case RoadCommand.Remove c: OnRoadRemove(c); break;
                }
            }
        }
    }
}