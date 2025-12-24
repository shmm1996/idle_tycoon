using IdleTycoon.Scripts.Data.Serialization.Json;
using IdleTycoon.Scripts.Data.Session;
using IdleTycoon.Scripts.PlayerInput;
using IdleTycoon.Scripts.PlayerInput.Camera;
using IdleTycoon.Scripts.PlayerInput.Tilemap;
using IdleTycoon.Scripts.PlayerInput.Tilemap.Brushes;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.PreviewProjection;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Rules;
using IdleTycoon.Scripts.Presentation.Tilemap.Definitions.Tiles;
using IdleTycoon.Scripts.Presentation.Tilemap.PreviewProjection;
using IdleTycoon.Scripts.Presentation.Tilemap.Processor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using CameraPointerInputSource = IdleTycoon.Scripts.PlayerInput.Camera.InputSources.PointerInputSource;
using TilemapPointerInputSource = IdleTycoon.Scripts.PlayerInput.Tilemap.InputSources.PointerInputSource;

namespace IdleTycoon.Scripts
{
    public sealed class Game : MonoBehaviour
    {
        [SerializeField] [FormerlySerializedAs("Terrain")] private Tilemap terrainTilemap;
        [SerializeField] private TilemapTileRuleDefinition<TileTerrainDefinition>[] terrainRules;
        
        [SerializeField] [FormerlySerializedAs("Roads")] private Tilemap roadsTilemap;
        [SerializeField] private TilemapPartedTileRuleDefinition<TileRoadDefinition>[] roadsRules;
        
        [SerializeField] [FormerlySerializedAs("Preview")] private Tilemap previewTilemap;
        [SerializeField] private TilePreviewDefinition[] tilePreviews;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private CameraPointerInputSource.InputActions cameraInputActions;
        [SerializeField] private TilemapPointerInputSource.InputActions tilemapInputActions;

        public string json;

        private GameSession _session;
        private TilemapProcessor _processor;
        private TilemapPreviewRenderer _previewRenderer;
        
        private InputModeController _inputModeController;
        private CameraController _cameraController;

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
            _previewRenderer?.Dispose();
        }

        private void InstallGame()
        {
            _session = new GameSession();

            unsafe
            {
                _session.LoadWorldMap(JsonDeserializer.DeserializeWorldMap(json));
            }
            
            GameSession.Context context = _session.GetContext();
            
            //Game session state representation.
            TilemapTileProcessor<TilemapTileRuleDefinition<TileTerrainDefinition>, TileTerrainDefinition> terrainProcessor =
                new(terrainTilemap, context, terrainRules, t => t.weight);
            TilemapPartedTileProcessor<TilemapPartedTileRuleDefinition<TileRoadDefinition>, TileRoadDefinition> roadProcessor = 
                new(roadsTilemap, context, roadsRules, t => t.weight);

            _processor = new TilemapProcessor(context, terrainProcessor, roadProcessor);

            //Player input.
            GameSession.CommandsBus commandsBus = _session.GetCommandsBus();
            
            //Player input. Camera.
            _cameraController = new CameraController(camera);
            CameraPointerInputSource cameraPointInputSource = new(_cameraController, cameraInputActions);
            
            //Player input. Tilemap.
            BrushManager brushManager = new();
            brushManager
                .Register("debug_area", new BrushDebugArea())
                .Register("clean_area", new BrushCleanArea(commandsBus));
            brushManager.Activate("clean_area");
            
            TilemapController tilemapController = new(brushManager, context);
            TilemapPointerInputSource tilemapPointerInputSource = new(camera, tilemapController, tilemapInputActions);
            
            //Player input. Input mode.
            _inputModeController = new InputModeController();
            _inputModeController
                .Register(InputModeController.Mode.Camera, cameraPointInputSource)
                .Register(InputModeController.Mode.Tilemap, tilemapPointerInputSource);
            
            //Tilemap player input representation.
            _previewRenderer = new TilemapPreviewRenderer(previewTilemap, tilePreviews, brushManager);
            
            _session.Init();
            
            _inputModeController.Switch(InputModeController.Mode.Tilemap);
        }
        
        private void MainLoop(float deltaTime)
        {
            _session.Frame();
            
            _deltaTime += deltaTime;
            while (_deltaTime >= TickTime)
            {
                _session.Tick();
                _deltaTime -= TickTime;
            }
            
            int toResolveTilesCount = _processor.UpdateTiles(500);

            if (toResolveTilesCount > 0)
                Debug.LogWarning($"[{nameof(MainLoop)}] Tiles left to resolve: {toResolveTilesCount}.");
            
            _cameraController.Update(deltaTime);
        }
    }
}