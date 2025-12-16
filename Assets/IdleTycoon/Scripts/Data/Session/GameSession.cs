using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IdleTycoon.Scripts.Data.Commands;
using IdleTycoon.Scripts.Data.Systems;
using R3;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public unsafe class GameSession : IDisposable
    {
        private WorldMap* _worldMap;
        private WorldMap.ReadOnly _worldMapReadOnly;
        private readonly Subjects _subjects = new();
        private readonly ISystem[] _systems;
        private readonly List<int2> _toUpdate = new(256);
        
        public WorldMap* WorldMap => _worldMap;

        public GameSession()
        {
            _systems = new ISystem[]
            {
                new CommandProcessor(this)
            };
        }
        
        public void Dispose()
        {
            DisposeWorldMap();
        }
        
        private void DisposeWorldMap()
        {
            if (_worldMap == null) return;
            
            _worldMap->Dispose();
            UnsafeUtility.Free(_worldMap, Allocator.Persistent);
            _worldMapReadOnly = default;
        }
        
        public Context GetContext() => new(this);

        public CommandsBus GetCommandsBus() => new(this);

        public void LoadWorldMap(WorldMap* worldMap)
        {
            DisposeWorldMap();
            _worldMap = worldMap;
            _worldMapReadOnly = new WorldMap.ReadOnly(_worldMap);
            
            _subjects.onWorldLoaded.OnNext(new WorldMap.ReadOnly(_worldMap));
        }

        public void Init()
        {
            foreach (ISystem system in _systems) 
                system.Init();
        }

        public void Frame()
        {
            if (_toUpdate.Count != 0) _toUpdate.Clear();
            
            foreach (ISystem system in _systems) 
                system.OnFrame();

            if (_toUpdate.Count != 0) _subjects.onTilesUpdated.OnNext(_toUpdate.ToArray());
        }
        
        public void Tick()
        {
            if (_toUpdate.Count != 0) _toUpdate.Clear();
            
            foreach (ISystem system in _systems) 
                system.OnTick();

            if (_toUpdate.Count != 0) _subjects.onTilesUpdated.OnNext(_toUpdate.ToArray());
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToUpdate(int2 tile) => _toUpdate.Add(tile);
        
        public sealed class Context
        {
            private readonly GameSession _session;
            
            public Subjects.Observables Observables { get; }
            public WorldMap.ReadOnly WorldMap => _session._worldMapReadOnly;

            public Context(GameSession session)
            {
                _session = session;
                Observables = new Subjects.Observables(session._subjects);
            }
        }
        
#region CommandBus //TODO: Move into inner class.
        private readonly ReplaySubject<IGameCommand> _onGameCommand = new(100);
        public Observable<IGameCommand> OnGameCommand => _onGameCommand;
        
        public sealed class CommandsBus
        {
            private readonly GameSession _session;
            
            public CommandsBus(GameSession session) => _session = session;

            public void OnNext(IGameCommand command) => _session._onGameCommand.OnNext(command);
        }
#endregion
        
        public sealed class Subjects
        {
            public readonly ReplaySubject<WorldMap.ReadOnly> onWorldLoaded = new(1);
            public readonly ReplaySubject<int2[]> onTilesUpdated = new(1024);
            public readonly ReplaySubject<int2[]> onTilesCleaned = new(1024);
            
            public sealed class Observables
            {
                private readonly Subjects _subjects;
                
                public Observable<WorldMap.ReadOnly> OnWorldLoaded => _subjects.onWorldLoaded;
                public Observable<int2[]> OnTilesUpdated => _subjects.onTilesUpdated;
                public Observable<int2[]> OnTilesCleaned => _subjects.onTilesCleaned;
                
                public Observables(Subjects subjects) => _subjects = subjects;
            }
        }
    }
}