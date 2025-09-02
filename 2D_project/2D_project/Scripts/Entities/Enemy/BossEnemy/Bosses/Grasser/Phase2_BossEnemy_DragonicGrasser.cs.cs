using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class Phase2_BossEnemy_DragonicGrasser : IPhase
{
    // TODO -> add actual logic for the boss fight / right now only sprites
    private GameObject _gameObject;
    private IBossMethod movementMethod;
    private IBossMethod attackMethod;

    private BossEnemy bossEnemy;

    private float currReloadTime = 0;
    private float maxReloadTime = 2.5f;
    private bool isReloading = false;

    SpriteAnimated animatedSprite;

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
        int frameCount = 2;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Grasser_P1", frameCount);
        float[] frameTimers = [0.5f, 0.5f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Grasser_P1", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        _phaseColliderObject = new GrasserPhase2Colliders(parent: _gameObject, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    void IPhase.BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;

        bossEnemy.gameObject.transform.localRotationAngle = 0;
        bossEnemy.moveSpeed = 0;

        movementMethod = movementMethods[0];
        Movement_Teleport movement_Teleport = (Movement_Teleport)movementMethod;

        movementMethod.ResetSubsteps();

        Vector2 spawnPosition = bossEnemy.ArrivalPosition;

        List<Vector2> teleportLocations = [
            new Vector2(spawnPosition.X - 1450, spawnPosition.Y + 200),
            new Vector2(spawnPosition.X - 750, spawnPosition.Y + 500),
            new Vector2(spawnPosition.X + 750, spawnPosition.Y + 500),
            new Vector2(spawnPosition.X + 1450, spawnPosition.Y + 200)
        ];
        float teleportCooldownTime = 2f;
        int teleportCountBeforeNewSequence = int.MaxValue;

        bool isSequenceRandom = true;
        int[] teleportPositionIndices_Sequence = [0, 1, 2, 3];

        movementMethod.SetParameters(
            teleportLocations,
            teleportCooldownTime,
            teleportCountBeforeNewSequence,
            isSequenceRandom,
            teleportPositionIndices_Sequence
        );

        // 2.) setup attack pattern
        attackMethod = attackMethods[0];

        float projectileDeathTimer = 5;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Grasser;
        int projectileCount = 3;
        Vector2 projectileScale = new Vector2(0.8f, 0.8f);
        int projectileSeparatorDistance = 100;
        bool hasProjectileTerrainImunity = true;

        attackMethod.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            projectileSeparatorDistance,
            hasProjectileTerrainImunity
        );

        currReloadTime = 0;

        // show visuals
        _gameObject.SetActive(true);
    }

    void IPhase.UpdatePhase(GameTime gameTime)
    {
        // update sprite effects
        animatedSprite.spriteEffects = (bossEnemy.gameObject.transform.globalPosition.X > Player.Instance.gameObject.transform.globalPosition.X)
            ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        movementMethod?.Execute();

        Reload(gameTime);
        if (!isReloading)
        {
            attackMethod?.Execute();
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
}