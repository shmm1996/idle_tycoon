using IdleTycoon.Scripts.Data.Serialization.Json;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Presentation.Tilemap.Processor;
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
            _session = new GameSession();

            unsafe
            {
                _session.LoadWorldMap(JsonDeserializer.DeserializeWorldMap(json));
            }
            
            GameSession.Context context = _session.GetContext();
            
            SessionTileProvider sessionTiles = new(context.worldMap);

            TilemapSubProcessor<TileTerrainDefinition> terrainSubProcessor = new(terrainTilemap, sessionTiles, terrainRules);

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