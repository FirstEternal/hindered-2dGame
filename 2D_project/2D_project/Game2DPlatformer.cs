using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static GameConstantsAndValues;

namespace GamePlatformer
{
    public class Game2DPlatformer : Game
    {
        public static Game2DPlatformer Instance { get; private set; }
        private GraphicsDeviceManager graphics;

        public Random random { get; private set; } = new Random(Environment.TickCount ^ Guid.NewGuid().GetHashCode());
        public Game2DPlatformer()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private int currFrameCount = 0;
        private const int FIXED_UPDATE_FRAME_COUNT = 15;// every 15 frames

        public static void SetupCollisionRules()
        {
            // Set up custom collision rules for the game
            Dictionary<string, HashSet<string>> customCollisionMap = new Dictionary<string, HashSet<string>>()
            {
                { Tags.Player.ToString(), new HashSet<string> { "Enemy", "GravitationalEnemy", "EnemySpawned", "Terrain", "MovableTerrain", "AntiTerrainEnemySpawned", "EnemyMeleeSpawned" } },
                { Tags.PlayerSpawned.ToString(), new HashSet<string> { "Enemy", "GravitationalEnemy", "EnemySpawned", "Terrain", "MovableTerrain", "AntiTerrainEnemySpawned", "Button", "EnemyWeakToProjectileOnly" } },
                { Tags.PlayerMeleeSpawned.ToString(), new HashSet<string> { "Enemy", "GravitationalEnemy" } },
                { Tags.AntiTerrainPlayerSpawned.ToString(), new HashSet<string> { "Enemy", "GravitationalEnemy", "EnemySpawned", "AntiTerrainEnemySpawned", "Button", "EnemyWeakToProjectileOnly" } },
                { Tags.EnemySpawned.ToString(), new HashSet<string> { "Player", "PlayerSpawned", "Terrain", "MovableTerrain", "AntiTerrainPlayerSpawned" } },
                { Tags.EnemyMeleeSpawned.ToString(), new HashSet<string> { "Player" } },
                { Tags.AntiTerrainEnemySpawned.ToString(), new HashSet<string> { "Player", "PlayerSpawned", "AntiTerrainPlayerSpawned" } },
                { Tags.Enemy.ToString(), new HashSet<string> { "Player", "PlayerSpawned", "AntiTerrainPlayerSpawned", "Terrain", "MovableTerrain", "PlayerMeleeSpawned" } },
                { Tags.GravitationalEnemy.ToString(), new HashSet<string> { "Player", "PlayerSpawned", "AntiTerrainPlayerSpawned", "PlayerMeleeSpawned" } },
                { Tags.Terrain.ToString(), new HashSet<string> { "Player", "PlayerSpawned", "Enemy", "EnemySpawned" } },
                { Tags.MovableTerrain.ToString(), new HashSet<string> { "Player", "PlayerSpawned", "Enemy", "EnemySpawned", "Hidden" } },
                { Tags.Hidden.ToString(), new HashSet<string> { "MovableTerrain" } },
                { Tags.Button.ToString(), new HashSet<string> { "PlayerSpawned" } },
                { Tags.EnemyWeakToProjectileOnly.ToString(), new HashSet<string> { "PlayerSpawned", "AntiTerrainPlayerSpawned" } },
            };

            // Now set these rules into the CollisionRules class
            CollisionRules.SetCollisionRules(customCollisionMap);

            // Optionally, you can also add or remove specific rules dynamically if needed:
            // CollisionRules.AddCollisionRule("Player", "Boss");
            // CollisionRules.RemoveCollisionRule("Terrain", "Player");
        }

        protected override void Initialize()
        {

            //Window.IsBorderless = true;

            SetupCollisionRules();
            // create fps counter
            new FPSCounter(this);
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = true;
            //IsFixedTimeStep = false;
            // sprite sheet manager
            JSON_Manager spriteSheetManager = new JSON_Manager();
            spriteSheetManager.LoadJson();

            // create condition controller
            ConditionController conditionController = new ConditionController(this);
            ButtonResponseSystem buttonResponseSystem = new ButtonResponseSystem(this, refreshTimer: 0.25f);

            // gravity section
            Physics.gameGravityVector = Physics.gameGravityVector * 50;
            // create GameWindow Overlay information
            GameWindow gameWindow = new GameWindow(this, graphics, 1280, 720);
            //gameWindow.EnableResizing(true);
            //gameWindow.gameScale = new Vector2(0.5f, 0.5f);
            SettingsToolBar settingsToolBar = new SettingsToolBar();
            // create a scene
            Scene mainMenuScene = new Scene_MenuScene(this);

            HomeBaseScene homeBaseScene = new HomeBaseScene(this);

            Scene stage1 = new Scene_Stage1(this);
            Scene stage2 = new Scene_Stage2(this);

            string jsonChallengePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles");
            Scene bossFightScene_1 = new Boss_Fight_Burner(this);
            Scene bossFightScene_2 = new Boss_Fight_Drowner(this);
            Scene bossFightScene_3 = new Boss_Fight_Boulderer(this);
            Scene bossFightScene_4 = new Boss_Fight_Froster(this);
            Scene bossFightScene_5 = new Boss_Fight_Grasser(this);
            Scene bossFightScene_6 = new Boss_Fight_Shader(this);
            Scene bossFightScene_7 = new Boss_Fight_Thunderer(this);
            Scene bossFightScene_8 = new Boss_Fight_TraSephTra(this);

            Scene testingCollisionScene = new CollisionTESTscene(this);
            Scene bossColliderTestingScene = new BossCollidersTesting(this);

            //string jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge1.json");

            int sceneIndex = 0;

            // create a scene manager
            SceneManager sceneManager = SceneManager.Instance;

            // add scene to the list
            sceneManager.AddScene(mainMenuScene, sceneIndex++);
            sceneManager.AddScene(homeBaseScene, sceneIndex++);
            sceneManager.AddScene(stage1, sceneIndex++);
            sceneManager.AddScene(stage2, sceneIndex++);

            sceneManager.AddScene(bossFightScene_1, sceneIndex++);
            sceneManager.AddScene(bossFightScene_2, sceneIndex++);
            sceneManager.AddScene(bossFightScene_3, sceneIndex++);
            sceneManager.AddScene(bossFightScene_4, sceneIndex++);
            sceneManager.AddScene(bossFightScene_5, sceneIndex++);
            sceneManager.AddScene(bossFightScene_6, sceneIndex++);
            sceneManager.AddScene(bossFightScene_7, sceneIndex++);
            sceneManager.AddScene(bossFightScene_8, sceneIndex++);
            //sceneManager.AddScene(sceneJsonTest, sceneIndex++);
            sceneManager.AddScene(testingCollisionScene, sceneIndex++);
            sceneManager.AddScene(bossColliderTestingScene, sceneIndex++);

            MouseGameObject mouseGameObject = new MouseGameObject(graphics.GraphicsDevice);

            InputController inputController = new InputController(this);
            KeyBindManager keyBindManager = new KeyBindManager(inputController);

            // sound
            SoundController soundController = new SoundController(this);
            string soundJsonFilePath = Path.Combine("..", "..", "..", "Content", "sounds", "JSON_sounds.json");
            SoundData.LoadFromJson(soundJsonFilePath);
            SoundData.LoadAllSounds(this);


            // create Player Instance
            GameObject gameObject;
            gameObject = new GameObject(-999);
            gameObject.CreateTransform();
            Player player = new Player(mass: 50);
            gameObject.AddComponent(player);
            player.Initialize();

            // create Respawn System
            RespawnPointSystem respawnPointSystem = new RespawnPointSystem();

            // create camera       
            Camera camera = new Camera();

            // add renderer to scene manager
            // create renderer
            Renderer renderer = new Renderer(this);
            SceneManager.Instance.ChangeRenderer(renderer);

            DebugRenderer debugRenderer = new DebugRenderer(this);
            SceneManager.Instance.ChangeDebugRenderer(debugRenderer);

            OverlayRenderer overlayRenderer = new OverlayRenderer(this);
            SceneManager.Instance.ChangeOverlayRenderer(overlayRenderer);

            Components.Add(renderer);
            Components.Add(debugRenderer);
            Components.Add(overlayRenderer);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create projectiles
            AttackObjectPoolingSystem projectilePoolingSystem = new AttackObjectPoolingSystem(this);
            projectilePoolingSystem.Initialize();

            AttackObjectPoolingSystem.LoadContent();
            //SceneManager.Instance.activeScene.LoadContent();
            SceneManager.Instance.LoadScene(0);
            //SceneManager.Instance.LoadScene(1);
            //SceneManager.Instance.LoadScene("TESTscene");
            //SceneManager.Instance.LoadScene("BossCollidersTesting");

            // initialize element data base
            ElementDataBase elementDataBase = new ElementDataBase();
            elementDataBase.CreateElementDictionary();

            SpriteFont font = Content.Load<SpriteFont>("fonts/Arial_8");
            SceneManager.Instance.activeDebugRenderer.LoadSpriteFont(font);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Check if the window is active
            if (!IsActive)
            {
                // Skip updating input or game logic when window is not active
                base.Update(gameTime);
                return;
            }
            /*
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/

            FPSCounter.Instance.Update(gameTime);
            InputController.Instance.Update(gameTime);
            MouseGameObject.Singleton.Update(gameTime);

            currFrameCount++;
            if (currFrameCount == FIXED_UPDATE_FRAME_COUNT)
            {
                // reset frame counter
                currFrameCount = 0;

                SceneManager.Instance.activeScene.FixedUpdate(gameTime);
                return;
            }

            SceneManager.Instance.activeScene.Update(gameTime);
            ConditionController.Instance.Update(gameTime);
            ButtonResponseSystem.Instance.Update(gameTime);

            base.Update(gameTime);
            /*
            Debug.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds);
            double fps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;
            Debug.WriteLine("fps: " +( 1.0 / gameTime.ElapsedGameTime.TotalSeconds));
            */
            
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Black); // single clear
            GraphicsDevice.Clear(Color.CornflowerBlue); // single clear
            //SceneManager.Instance.RenderScene(gameTime);
            base.Draw(gameTime);
        }
    }
}