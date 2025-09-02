using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

internal class Scene_Stage(Game game) : TestingScene(game)
{
    protected List<RespawnPointSystem.RespawnPoint> respawnPoints;
    protected Vector2 spawnPointAdjustment = new Vector2(0, -200);

    public HUD_PauseMenu hudPauseMenu;
    public HUD_GameOverMenu gameOverMenu;

    //private bool isInitialized = false;

    protected override void InitializeContent()
    {
        base.InitializeContent();
        Debug.WriteLine("initializing content");
        // Create challenge ui
        HUD_PlayerAbilites.Add_HUD_PlayerAbilites(this);
        hudPauseMenu = new HUD_PauseMenu(this);
        gameOverMenu = new HUD_GameOverMenu(hudPauseMenu);
        Player.Instance.MakePlayerObjectChangeScene(this);
        AddJsonLevels();
        LevelStart();
    }

    protected virtual void AddJsonLevels() { }

    protected virtual void LevelStart()
    {
        // TODO -> reset entire scene logic
        isPaused = false;
        RespawnPointSystem.Instance.AssignStartingRespawnPoint(scene: this, respawnPoints: respawnPoints, startingRespawnPointIndex: 0);
    }

    public static void CreateLevel(Scene_Stage scene, string jsonFilePath, Vector2 levelStartPos)
    {
        // create level from json
        foreach (var gameObject in LevelFactory.DeserializeLevel(File.ReadAllText(jsonFilePath)))
        {
            scene.AddGameObjectToScene(gameObject, isOverlay: false);

            gameObject.GetComponent<MoveOnCollisionPlatformComponent>()?.AdjustToLevelStartPosition(levelStartPos);
            gameObject.GetComponent<TeleportObject>()?.AdjustToLevelStartPosition(startPosition: levelStartPos);

            EnemySpawner enemySpawner = gameObject.GetComponent<EnemySpawner>();
            if (enemySpawner is not null)
            {
                enemySpawner.SpawnEnemies(scene: scene, startPosition: levelStartPos);
                enemySpawner.SpawnBossEnemies(scene: scene, startPosition: levelStartPos);
            }

            // move to match start position of level part1
            gameObject.transform.globalPosition += levelStartPos;
            /*

            scene.AddGameObjectToScene(gameObject, isOverlay: false);
            MoveOnCollisionPlatformComponent mocpc = gameObject.GetComponent<MoveOnCollisionPlatformComponent>();
            if (mocpc is not null)
            {
                mocpc.AdjustToLevelStartPosition(levelStartPos);
            }

            TeleportObject teleporter = gameObject.GetComponent<TeleportObject>();
            if (teleporter is not null)
            {
                teleporter.AdjustToLevelStartPosition(startPosition: levelStartPos);
            }

            EnemySpawner enemySpawner = gameObject.GetComponent<EnemySpawner>();
            if (enemySpawner is not null)
            {
                enemySpawner.SpawnEnemies(scene: scene, startPosition: levelStartPos);
                enemySpawner.SpawnBossEnemies(scene: scene, startPosition: levelStartPos);
            }

            //if (EnemySpawner)

            // move to match start position of level part1
            gameObject.transform.globalPosition += levelStartPos;
            */
        }

        // initialize recievers and other components that require objects already present
        foreach (var gameObject in scene.gameObjects)
        {
            Reciever reciever = gameObject.GetComponent<Reciever>();
            if (reciever is not null)
            {
                reciever.AssignTransmitter(scene.gameObjects);
            }
            StopBossMovementComponent stopBossMovementComponent = gameObject.GetComponent<StopBossMovementComponent>();
            if (stopBossMovementComponent is not null)
            {
                stopBossMovementComponent.AssignBossEnemy(scene.gameObjects);
            }
        }


        Player.Instance.onDeath -= scene.OnPlayerDeath;
        Player.Instance.onDeath += scene.OnPlayerDeath;
    }

    protected virtual void OnPlayerDeath(object sender, EventArgs e)
    {
        RestartStage(resetRespawn: false);
    }

    public virtual void RestartStage(bool resetRespawn)
    {
        if (resetRespawn)
        {
            // this also resets player
            RespawnPointSystem.Instance.AssignStartingRespawnPoint(scene: this, respawnPoints: respawnPoints, startingRespawnPointIndex: 0);
        }

        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].ResetAll();
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (InputController.Instance.IsKeyPressed(Keys.Escape))
        {
            PauseGame();
        }

        base.Update(gameTime);
    }

    protected void PauseGame()
    {
        isPaused = !isPaused;
        hudPauseMenu.PauseMenu.SetActive(isPaused);
    }

    public override void StateController_OnStateChange(object sender, EventArgs e)
    {
        base.StateController_OnStateChange(sender, e);

        isPaused = GetCurrentStateName() == "GAME PAUSED";
        hudPauseMenu.PauseMenu.SetActive(isPaused);
    }
    public override void UnloadContent()
    {
        return;
        //base.UnloadContent();
    }
}
