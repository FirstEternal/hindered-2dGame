using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class Phase1_BossEnemy_DragonicFroster : IPhase
{
    private GameObject _gameObject;
    private IBossMethod movementMethod;

    private IBossMethod attackMethod1;
    private IBossMethod attackMethod2;

    private float currReloadTime = 0;
    private float maxReloadTime = 1.25f;
    private bool isReloading = false;

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
        int frameCount = 2;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Froster_P1", frameCount);
        float[] frameTimers = [0.2f, 0.2f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Froster_P1", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        _phaseColliderObject = new FrosterPhase1Colliders(parent: _gameObject, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    public void BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;

        movementMethod = movementMethods[0];
        movementMethod.ResetSubsteps();

        float distanceX = 500;
        Vector2 startPosition = new Vector2(bossEnemy.ArrivalPosition.X - distanceX, bossEnemy.ArrivalPosition.Y);
        Vector2 endDestination = new Vector2(bossEnemy.ArrivalPosition.X + distanceX, bossEnemy.ArrivalPosition.Y);
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
        attackMethod1 = attackMethods[0]; // setting parameters not requiered, due to dynamic changing
        attackMethod2 = attackMethods[1];

        float projectileDeathTimer = 5;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Froster;
        int projectileCount = 4;
        Vector2 projectileScale = new Vector2(0.9f, 0.9f);
        bool hasProjectileTerrainImunity = true;
        bool isLeftToRight = false;
        int projectileSeparatorDistance = 70;

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
    }

    public void UpdatePhase(GameTime gameTime)
    {
        // update sprite effects
        // opposite logic because my dumass flipped sprites while drawing
        animatedSprite.spriteEffects = (bossEnemy.Velocity.X > 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        movementMethod?.Execute();

        Reload(gameTime);

        float distanceX = Math.Abs(bossEnemy.gameObject.transform.globalPosition.X - Player.Instance.gameObject.transform.globalPosition.X);
        bool strikeBellow = distanceX <= 100;

        if (!isReloading)
        {
            if (strikeBellow) attackMethod2?.Execute(); // attack in a line, beneath
            else BeginAttack();

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
    private void BeginAttack()
    {
        float projectileDeathTimer = 5;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Froster;
        int projectileCount = 3;
        Vector2 projectileScale = new Vector2(0.9f, 0.9f);

        int degreeStart = 0;
        int degreeEnd = 90;
        bool isLeftToRight = bossEnemy.gameObject.transform.globalPosition.X > Player.Instance.gameObject.transform.globalPosition.X;
        bool hasProjectileTerrainImunity = true;
        // if players is underneath spawn 5: up -> down, otherwise spawn 3: left -> right

        attackMethod1.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            degreeStart,
            degreeEnd,
            isLeftToRight,
            hasProjectileTerrainImunity
        );

        attackMethod1?.Execute();
    }
}
