using System;
using R3;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace IdleTycoon.Scripts.Data.Session
{
    public unsafe class GameSession : IDisposable
    {
        private WorldMap* _worldMap;
        private readonly Subjects _subjects = new();
        
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
            
            _subjects.onWorldLoaded.OnNext(new WorldMap.ReadOnly(_worldMap));
        }
        
        public void Tick()
        {
            
        }
        
        public class Context
        {
            public readonly WorldMap.ReadOnly worldMap;
            public readonly Subjects.Observables observables;

            public Context(GameSession context)
            {
                worldMap = new WorldMap.ReadOnly(context._worldMap);
                observables = new Subjects.Observables(context._subjects);
            }
        }
        
        public class Subjects
        {
            public readonly ReplaySubject<WorldMap.ReadOnly> onWorldLoaded = new(1);
            public readonly ReplaySubject<int2[]> onTilesUpdated = new(1024);
            public readonly ReplaySubject<int2[]> onTilesCleaned = new(1024);
            
            public class Observables
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