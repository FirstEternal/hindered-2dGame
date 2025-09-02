using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class Phase2_BossEnemy_DragonicFroster : IPhase
{
    private GameObject _gameObject;
    private IBossMethod movementMethod;

    private Attack_RainFromAbove attackMethod;

    private bool isRainFromAboveReloading;

    Vector2 prevVelocity;

    private BossEnemy bossEnemy;

    SpriteAnimated animatedSprite;

    PhaseColliderObject _phaseColliderObject;
    PhaseColliderObject IPhase.phaseColliderObject { get => _phaseColliderObject; set => _phaseColliderObject = value; }

    public EventHandler OnVisualChange { get; set; }

    public GameObject GetVisualGameObject()
    {
        return _gameObject;
    }

    public void CreateVisuals(GameObject parent)
    {
        // 1.) create GameObject that will hold visuals
        _gameObject = new GameObject();
        _gameObject.CreateTransform();

        parent.AddChild(_gameObject);

        // 2.) assign animated sprite
        int frameCount = 7;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Froster_P2", frameCount);
        float[] frameTimers = [0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.5f, 0.5f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Froster_P2", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        _phaseColliderObject = new FrosterPhase2Colliders(parent: _gameObject, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    public void BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;

        Vector2 phase2ArrivalPosition = new Vector2(bossEnemy.ArrivalPosition.X, bossEnemy.ArrivalPosition.Y - 350);
        bossEnemy.gameObject.transform.globalPosition = phase2ArrivalPosition;

        movementMethod = movementMethods[0];
        movementMethod.ResetSubsteps();

        float distanceX = 350;
        Vector2 startPosition = new Vector2(phase2ArrivalPosition.X - distanceX, phase2ArrivalPosition.Y);
        Vector2 endDestination = new Vector2(phase2ArrivalPosition.X + distanceX, phase2ArrivalPosition.Y);
        float speedForth = 200;
        float speedBack = speedForth;
        int backForthCount = int.MaxValue;

        movementMethod.SetParameters(
            startPosition,
            endDestination,
            speedForth,
            speedBack,
            backForthCount
        );

        // 2.) setup attack pattern
        attackMethod = (Attack_RainFromAbove)attackMethods[0];

        float projectileDeathTimer = 5;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Froster;
        Vector2 projectileScale = new Vector2(1.5f, 1.5f);
        Vector2 startPosX = new Vector2(
            bossEnemy.ArrivalPosition.X - 20 * GameConstantsAndValues.SQUARE_TILE_WIDTH,
            bossEnemy.ArrivalPosition.Y - 1000
        );
        Vector2 endPosX = new Vector2(
            bossEnemy.ArrivalPosition.X + 20 * GameConstantsAndValues.SQUARE_TILE_WIDTH,
            bossEnemy.ArrivalPosition.Y - 1000
        );

        bool hasProjectileTerrainImunity = true;

        float fallHeight = 350;
        bool reverseSpread = false;
        attackMethod.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileScale,
            startPosX,
            endPosX,
            fallHeight,
            hasProjectileTerrainImunity,
            reverseSpread
        );

        isRainFromAboveReloading = true;

        // show visuals
        _gameObject.SetActive(true);
    }

    public void UpdatePhase(GameTime gameTime)
    {
        // update sprite effects
        // opposite logic because my dumass flipped sprites while drawing
        animatedSprite.spriteEffects = (bossEnemy.Velocity.X > 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        movementMethod?.Execute();

        if (animatedSprite.currFrameIndex == 0)
        {
            isRainFromAboveReloading = false;
            bossEnemy.isMovable = true;
        }
        else if (animatedSprite.currFrameIndex == 5)
        {
            if (!isRainFromAboveReloading)
            {
                bossEnemy.isMovable = false;
                attackMethod?.Execute();

                isRainFromAboveReloading = true;
            }

        }

        _phaseColliderObject.UpdateColliders();
    }

    public void Reload(GameTime gameTime) { throw new NotImplementedException(); }
}