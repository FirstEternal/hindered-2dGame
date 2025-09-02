using GamePlatformer;
using MGEngine.ObjectBased;
using MGEngine.Physics;
using Microsoft.Xna.Framework;
using System;

internal class RecieverActions
{
    /// <summary>
    /// When called, advanced Respawn to a new section(by 1 or by index)
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// must have StopBossMovementComponent as component
    /// </summary>
    public static void StopBossMovement(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];
        StopBossMovementComponent stopBossMovementComponent = gameObject.GetComponent<StopBossMovementComponent>();

        if (stopBossMovementComponent is not null)
        {
            stopBossMovementComponent.bossEnemy.StopBossMovement();

            float delay = stopBossMovementComponent.stoppedTimeSeconds;
            Timer timer = new Timer(Game2DPlatformer.Instance, delay);

            // Define the callback
            Action<Timer> callback = null!;
            callback = (Timer t) =>
            {
                t.OnCountdownEnd -= callback; // Clean up

                stopBossMovementComponent.bossEnemy.ResumeBossMovement();
                Game2DPlatformer.Instance.Components.Remove(t); // Optional, if not in Timer internally
                t.Dispose(); // Safe here
            };

            timer.OnCountdownEnd += callback;
            timer.BeginTimer();
        }
        //gameObject.GetComponent<RespawnPointReachedRecieverComponentData>
    }
    /// <summary>
    /// When called, advanced Respawn to a new section(by 1 or by index)
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// 1 -> index, if given
    /// </summary>
    public static void RespawnPointReached(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];
        RespawnPointIndex respawnPointIndex = gameObject.GetComponent<RespawnPointIndex>();
        if (respawnPointIndex is not null)
        {
            RespawnPointSystem.Instance.RespawnPointReached(respawnPointIndex.index);
        }
        else
        {
            RespawnPointSystem.Instance.NextRespawnPointReached(sender: gameObject, EventArgs.Empty);
        }
        //gameObject.GetComponent<RespawnPointReachedRecieverComponentData>
    }

    /*
    /// <summary>
    /// When called, change active state of GameObject
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// </summary>
    public static void StartLevelPart(params object[] parameters)
    {
        return;
        GameObject gameObject = (GameObject)parameters[0];
        gameObject.GetComponent<GoToLevelPartComponent>();
        Scene_LevelPart scene = (Scene_LevelPart)SceneManager.Instance.activeScene;

        int index = gameObject.GetComponent<GoToLevelPartComponent>().levelPartIndex;
        Scene_LevelPart.StartLevelPartAtIndex(scene, index);
    }*/

    /// <summary>
    /// When called, change active state of GameObject to true
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// </summary>
    public static void MakeActive(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];
        gameObject.SetActive(true);
    }

    /// <summary>
    /// When called, change active state of GameObject
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// </summary>
    public static void Flicker(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];
        gameObject.SetActive(!gameObject.isActive);
    }

    /// <summary>
    /// When called, reset button of Terrain_ButtonBox component
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// </summary>
    public static void ResetPressureButton(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];

        float delay = 1;
        Timer timer = new Timer(Game2DPlatformer.Instance, delay);

        // Define the callback
        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            t.OnCountdownEnd -= callback; // Clean up

            gameObject.GetComponent<Terrain_ButtonBox>().ResetButton();
            Game2DPlatformer.Instance.Components.Remove(t); // Optional, if not in Timer internally
            t.Dispose(); // Safe here
        };

        timer.OnCountdownEnd += callback;
        timer.BeginTimer();
    }

    /// <summary>
    /// When called, set GameObject active, after delay set it inactive
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// 1 -> float delay(potentially)
    /// </summary>
    public static void SetInactiveFor1Seconds(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];

        //float delay = (float)parameters[1];
        float delay = 1;

        gameObject.SetActive(false);

        Timer timer = new Timer(Game2DPlatformer.Instance, delay);

        // Define the callback
        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            gameObject.SetActive(true);
            t.OnCountdownEnd -= callback; // Clean up

            Game2DPlatformer.Instance.Components.Remove(t); // Optional, if not in Timer internally
            t.Dispose(); // Safe here
        };

        timer.OnCountdownEnd += callback;
        timer.BeginTimer();
    }

    /// <summary>
    /// When called, set GameObject active, after delay set it inactive
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// 1 -> float delay(potentially)
    /// </summary>
    public static void SetInactiveFor5Seconds(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];

        //float delay = (float)parameters[1];
        float delay = 5;

        gameObject.SetActive(false);

        Timer timer = new Timer(Game2DPlatformer.Instance, delay);

        // Define the callback
        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            gameObject.SetActive(true);
            t.OnCountdownEnd -= callback; // Clean up

            Game2DPlatformer.Instance.Components.Remove(t); // Optional, if not in Timer internally
            t.Dispose(); // Safe here
        };

        timer.OnCountdownEnd += callback;
        timer.BeginTimer();
    }

    /// <summary>
    /// When called, set GameObject active, after delay set it inactive
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// 1 -> float delay(potentially)
    /// </summary>
    public static void SetActiveFor1Second(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];

        //float delay = (float)parameters[1];
        float delay = 1;

        gameObject.SetActive(true);

        Timer timer = new Timer(Game2DPlatformer.Instance, delay);

        // Define the callback
        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            gameObject.SetActive(false);
            t.OnCountdownEnd -= callback; // Clean up

            Game2DPlatformer.Instance.Components.Remove(t); // Optional, if not in Timer internally
            t.Dispose(); // Safe here
        };

        timer.OnCountdownEnd += callback;
        timer.BeginTimer();
    }


    /// <summary>
    /// When called, change active state of GameObject
    /// parameters: 
    /// 0 -> Gameobject gameObject
    /// </summary>
    public static void StartMoving(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];
        gameObject.GetComponent<PhysicsComponent>().isMovable = true;
    }

    /// <summary>
    /// When called, moves transform to a new position
    /// parameters: 
    /// 0 -> PerformAction actionOnPosition
    /// 1 -> Gameobject gameObject, 
    /// 2 -> Vector2 newPos, 
    /// 3 -> Vector2 velocity
    /// </summary>
    public static void MoveIntoPos(params object[] parameters)
    {
        // NEEDS FIXING
        Reciever.PerformAction actionOnPosition = (Reciever.PerformAction)parameters[0];
        GameObject gameObject = (GameObject)parameters[1];
        Transform transform = gameObject.transform;
        if (transform is null || actionOnPosition is null) throw new NullReferenceException();

        Vector2 moveToPos = (Vector2)parameters[2];
        Vector2 velocity = (Vector2)parameters[3];

        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();
        physicsComponent.Velocity = velocity;

        // check if already in position
        if (Movement.IsInPosition(moveToPos, transform, 30)) return;

        // not in position, therefore create a new positionCondition
        Condition_TransformPosition positionCondition = new Condition_TransformPosition(moveToPos, gameObject.transform, 30);
        //SceneManager.Instance.activeScene.positionConditions.Add(positionCondition);

        // positionCondition.OnConditionMet -= actionOnPosition;
    }

    public static void StopMovement(params object[] parameters)
    {
        GameObject gameObject = (GameObject)parameters[0];
        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();
        physicsComponent.isMovable = false;
    }
}