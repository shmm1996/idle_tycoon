using IdleTycoon.Scripts.Data.Commands;
using IdleTycoon.Scripts.Data.Session;

namespace IdleTycoon.Scripts.Data.Systems
{
    public unsafe partial class CommandProcessor : ISystem
    {
        private readonly GameSession _session;
        private readonly CommandsQueue.Dequeuer _commands;
        
        private WorldMap* _worldMap;
        
        public CommandProcessor(GameSession session)
        {
            _session = session;
            _commands = session.commands.GetDequeuer();
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void OnTick()
        {
            _worldMap = _session.WorldMap;
            for (int count = _commands.Count; count > 0 && _commands.TryDequeue(out IGameCommand command); count--)
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