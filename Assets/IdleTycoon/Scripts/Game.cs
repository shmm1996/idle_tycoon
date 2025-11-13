using System.Collections.Generic;
using IdleTycoon.Scripts.Data.Serialization.Json;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.TileMap.Definitions.Rules;
using IdleTycoon.Scripts.TileMap.Definitions.Tiles;
using IdleTycoon.Scripts.TileMap.Processor;
using IdleTycoon.Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] [FormerlySerializedAs("Terrain")] private Tilemap terrainTilemap;
        [SerializeField] private TilemapRuleDefinition<TileTerrainDefinition>[] terrainRules;

        public string json;

        private GameSession _session;
        private TilemapProcessor _processor;
        private RandomProvider _random;

        private void Start()
        {
            InstallGame();
        }

        private void Update()
        {
            MainLoop(1);
        }

        private void OnDestroy()
        {
            _session?.Dispose();
            _processor?.Dispose();
        }

        private void InstallGame()
        {
            _random = new RandomProvider(new Dictionary<string, int> { { "tilemap", 0 } });
            
            _session = new GameSession();

            unsafe
            {
                _session.LoadWorldMap(JsonDeserializer.DeserializeWorldMap(json));
            }
            
            GameSession.Context context = _session.GetContext();
            
            SessionTileProvider sessionTiles = new(context.worldMap);

            TilemapSubProcessor<TileTerrainDefinition> terrainSubProcessor = new(terrainTilemap, sessionTiles, terrainRules, _random);

            _processor = new TilemapProcessor(context, terrainSubProcessor);
        }

        private void MainLoop(int ticks)
        {
            for(int t = 0; t < ticks; t++)
                _session.Tick();

            int toResolveTilesCount = _processor.UpdateTiles(100);

            if (toResolveTilesCount > 0)
                Debug.LogWarning($"[{nameof(MainLoop)}] Tiles left to resolve: {toResolveTilesCount}.");
        }
    }
}