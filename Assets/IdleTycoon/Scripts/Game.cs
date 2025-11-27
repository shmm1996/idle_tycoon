using IdleTycoon.Scripts.Data.Serialization.Json;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.PlayerInput.Tilemap;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes;
using IdleTycoon.Scripts.PlayerInput.Tilemap.InputSources;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Presentation.Tilemap.Processor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace IdleTycoon.Scripts
{
    public sealed class Game : MonoBehaviour
    {
        [SerializeField] [FormerlySerializedAs("Terrain")] private Tilemap terrainTilemap;
        [SerializeField] private TilemapTileRuleDefinition<TileTerrainDefinition>[] terrainRules;
        
        [SerializeField] [FormerlySerializedAs("Roads")] private Tilemap roadsTilemap;
        [SerializeField] private TilemapPartedTileRuleDefinition<TileRoadDefinition>[] roadsRules;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private InputActionReference pointerPosition;
        [SerializeField] private InputActionReference pointerPress;

        public string json;

        private GameSession _session;
        private TilemapProcessor _processor;
        private MouseInputSource _inputSource;

        private const float TickTime = 1f;
        private float _deltaTime = 0f;

        private void Start()
        {
            InstallGame();
        }

        private void Update()
        {
            MainLoop(Time.deltaTime);
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
            
            TilemapTileProcessor<TilemapTileRuleDefinition<TileTerrainDefinition>, TileTerrainDefinition> terrainProcessor =
                new(terrainTilemap, context, terrainRules, t => t.weight);
            TilemapPartedTileProcessor<TilemapPartedTileRuleDefinition<TileRoadDefinition>, TileRoadDefinition> roadProcessor = 
                new(roadsTilemap, context, roadsRules, t => t.weight);

            _processor = new TilemapProcessor(context, terrainProcessor, roadProcessor);

            BrushManager tilemapBrushManager = new();
            tilemapBrushManager.Register("debug", new BrushDebugArea());
            tilemapBrushManager.Activate("debug");
            InputController tilemapInputController = new(tilemapBrushManager, context);
            _inputSource = new MouseInputSource(camera, tilemapInputController, pointerPosition, pointerPress);
        }
        
        private void MainLoop(float deltaTime)
        {
            _inputSource.Update();
            
            _deltaTime += deltaTime;

            while (_deltaTime >= TickTime)
            {
                _session.Tick();
                _deltaTime -= TickTime;
            }
            
            int toResolveTilesCount = _processor.UpdateTiles(500);

            if (toResolveTilesCount > 0)
                Debug.LogWarning($"[{nameof(MainLoop)}] Tiles left to resolve: {toResolveTilesCount}.");
        }
    }
}