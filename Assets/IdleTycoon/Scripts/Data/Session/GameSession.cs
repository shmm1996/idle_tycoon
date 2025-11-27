using System;
using IdleTycoon.Scripts.Data.Commands;
using IdleTycoon.Scripts.Data.Systems;
using IdleTycoon.Scripts.Utils;
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
        
        public readonly ReplaySubject<IGameCommand> onGameCommand = new(100);
        public readonly HashQueue<int2> toUpdate = new();
        
        public WorldMap* WorldMap => _worldMap;
        public WorldMap.ReadOnly WorldMapReadOnly => _worldMapReadOnly;

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
        }
        
        public Context GetContext() => new(this);

        public void LoadWorldMap(WorldMap* worldMap)
        {
            DisposeWorldMap();
            _worldMap = worldMap;
            _worldMapReadOnly = new WorldMap.ReadOnly(_worldMap);
            
            _subjects.onWorldLoaded.OnNext(new WorldMap.ReadOnly(this._worldMap));
        }

        public void Init()
        {
            foreach (ISystem system in _systems) 
                system.Init();
        }
        
        public void Tick()
        {
            foreach (ISystem system in _systems) 
                system.OnTick();
        }
        
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