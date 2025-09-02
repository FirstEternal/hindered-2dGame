using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class Phase1_BossEnemy_DragonicGrasser : IPhase
{
    // TODO -> add actual logic for the boss fight / right now only sprites
    private GameObject _gameObject;
    private IBossMethod activeMovementMethod;

    private IBossMethod movementMethod1;
    private Movement_Spiral movementMethod2;

    private IBossMethod attackMethod1;
    private IBossMethod attackMethod2;

    private BossEnemy bossEnemy;

    SpriteAnimated animatedSprite;

    private float currReloadTime = 0;
    private float maxReloadTime = 0.75f;
    private bool isReloading = false;

    private bool isSpiralMovement;

    PhaseColliderObject _phaseColliderObject;
    PhaseColliderObject IPhase.phaseColliderObject { get => _phaseColliderObject; set => _phaseColliderObject = value; }

    public EventHandler OnVisualChange { get; set; }

    public GameObject GetVisualGameObject()
    {
        return _gameObject;
    }

    void IPhase.CreateVisuals(GameObject parent)
    {
        // 1.) create GameObject that will hold visuals
        _gameObject = new GameObject();
        _gameObject.CreateTransform();

        parent.AddChild(_gameObject);

        // 2.) assign animated sprite
        int frameCount = 3;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Grasser_P0", frameCount);
        float[] frameTimers = [0.07f, 0.07f, 0.07f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Grasser_P0", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        _phaseColliderObject = new GrasserPhase1Colliders(parent: _gameObject, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    void IPhase.BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;

        // 1.) setup movement pattern
        // BackForth movement
        movementMethod1 = movementMethods[0];

        Movement_TeleportBackForth movement_TeleportBackForth = (Movement_TeleportBackForth)movementMethod1;
        movement_TeleportBackForth.OnTeleport -= BeginAttack;
        movement_TeleportBackForth.OnTeleport += BeginAttack;

        movementMethod1.ResetSubsteps();

        Vector2 spawnPosition = bossEnemy.ArrivalPosition;

        List<Vector2> teleportLocations = [
            new Vector2(spawnPosition.X - 1400, spawnPosition.Y),
            new Vector2(spawnPosition.X + 1400, spawnPosition.Y)
        ];
        float teleportCooldownTime = 1f;
        int teleportCountBeforeNewSequence = int.MaxValue;
        int speed = 200;
        int movementSpeedIncreasePerTeleport = 0;
        int maxSpeed = speed;
        bool isSequenceRandom = false;
        int[] teleportPositionIndices_Sequence = [0, 1];

        movementMethod1.SetParameters(
            teleportLocations,
            teleportCooldownTime,
            teleportCountBeforeNewSequence,
            speed,
            movementSpeedIncreasePerTeleport,
            maxSpeed,
            isSequenceRandom,
            teleportPositionIndices_Sequence
        );

        // Spiral movement
        movementMethod2 = (Movement_Spiral)movementMethods[1];
        movementMethod2.ResetSubsteps();

        Vector2 center = new Vector2(spawnPosition.X, spawnPosition.Y + 400);
        float angularSpeed = 0.1f * 60;              // radians per frame
        float radialSpeed = 2f * 60;               // units per frame
        float maxRadius = 1000f;
        float startAngle = 0f;

        movementMethod2.SetParameters(center, angularSpeed, radialSpeed, maxRadius, startAngle);

        bossEnemy.OnSpecialMove -= ToggleMovement;
        bossEnemy.OnSpecialMove += ToggleMovement;

        activeMovementMethod = movementMethod1;
        isSpiralMovement = false;

        // 2.) setup attack pattern
        attackMethod1 = attackMethods[0];

        float projectileDeathTimer = 5;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Grasser;
        int projectileCount = 1;
        Vector2 projectileScale = new Vector2(1.2f, 1.2f);
        int projectileSeparatorDistance = 0;
        bool hasProjectileTerrainImunity = true;

        attackMethod1.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            projectileSeparatorDistance,
            hasProjectileTerrainImunity
        );

        attackMethod2 = attackMethods[1];

        projectileScale = new Vector2(0.8f, 0.8f);
        bool isLeftToRight = false;
        hasProjectileTerrainImunity = false;

        attackMethod2.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            projectileSeparatorDistance,
            isLeftToRight,
            hasProjectileTerrainImunity
        );

        currReloadTime = 0;
        // show visuals
        _gameObject.SetActive(true);

        // schedule movement change
        int scheduleTimer = 8;
        ScheduleSpecialMove(scheduleTimer);
    }

    private Vector2 CalculateSpiralCenterPosition(Vector2 spawnPosition)
    {
        float playerPosX = Player.Instance.gameObject.transform.globalPosition.X;
        float edgeLeft = spawnPosition.X - 1600;
        float centerX = spawnPosition.X;
        float edgeRight = spawnPosition.X + 1600;

        // Find the closest anchor to the player
        float closestX = centerX;
        float distToCenter = Math.Abs(playerPosX - centerX);
        float distToLeft = Math.Abs(playerPosX - edgeLeft);
        float distToRight = Math.Abs(playerPosX - edgeRight);

        if (distToLeft < distToCenter && distToLeft < distToRight)
        {
            closestX = spawnPosition.X - 1000;
        }
        else if (distToRight < distToCenter && distToRight < distToLeft)
        {
            closestX = spawnPosition.X + 1000;
        }

        // Final center position with Y offset
        return new Vector2(closestX, spawnPosition.Y + 400); // y position is slightly different than spawnPosition
    }

    private void ToggleMovement(object sender, EventArgs e)
    {
        // toggle movement
        isSpiralMovement = !isSpiralMovement;

        if (!isSpiralMovement)
        {
            activeMovementMethod = movementMethod1;

            // set next schedule
            int scheduleTimer = 8;
            ScheduleSpecialMove(scheduleTimer);
        }
        else
        {
            Vector2 spawnPosition = bossEnemy.ArrivalPosition;
            movementMethod2.ChangeSpiralCenter(CalculateSpiralCenterPosition(spawnPosition));
            activeMovementMethod = movementMethod2;

            // set next schedule
            float shortenedScheduleTimer = 3.5f;
            ScheduleSpecialMove(shortenedScheduleTimer);
        }

        activeMovementMethod.ResetSubsteps();
        bossEnemy.currReloadTime = 0;
    }

    void IPhase.UpdatePhase(GameTime gameTime)
    {
        // update sprite effects
        animatedSprite.spriteEffects = (bossEnemy.Velocity.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        activeMovementMethod?.Execute((float)gameTime.ElapsedGameTime.TotalSeconds);

        Reload(gameTime);
        if (!isReloading && !isSpiralMovement && bossEnemy.Velocity != Vector2.Zero)
        {
            attackMethod2?.Execute(); // attack in line
            isReloading = true;
        }

        _phaseColliderObject.UpdateColliders();
    }

    public void Reload(GameTime gameTime)
    {
        currReloadTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currReloadTime >= maxReloadTime)
        {
            currReloadTime = 0;
            isReloading = false;
        }
    }

    private void BeginAttack(object sender, EventArgs e)
    {
        attackMethod1?.Execute(); // towards player
        isReloading = false;
        currReloadTime = 0;
    }

    private Timer currentSpecialMoveTimer;

    private void ScheduleSpecialMove(float delaySeconds)
    {
        // Cancel the existing timer if it's still running
        if (currentSpecialMoveTimer != null)
        {
            Game2DPlatformer.Instance.Components.Remove(currentSpecialMoveTimer);
            currentSpecialMoveTimer.Dispose();
        }

        currentSpecialMoveTimer = new Timer(Game2DPlatformer.Instance, delaySeconds);

        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            bossEnemy.TriggerSpecialMove();
            t.OnCountdownEnd -= callback;
            Game2DPlatformer.Instance.Components.Remove(t);
            t.Dispose();
            currentSpecialMoveTimer = null; // clear reference
        };

        currentSpecialMoveTimer.OnCountdownEnd += callback;
        currentSpecialMoveTimer.BeginTimer();
    }
}