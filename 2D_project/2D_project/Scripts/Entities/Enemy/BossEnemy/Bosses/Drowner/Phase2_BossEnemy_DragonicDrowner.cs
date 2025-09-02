using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

internal class Phase2_BossEnemy_DragonicDrowner : IPhase
{
    private GameObject _gameObject;
    private IBossMethod movementMethod;
    private IBossMethod attackMethod;

    private BossEnemy bossEnemy;

    private float currReloadTime = 1;
    private float maxReloadTime = 1;
    private bool isReloading = false;

    SpriteAnimated animatedSprite;

    PhaseColliderObject _phaseColliderObject;
    PhaseColliderObject IPhase.phaseColliderObject { get => _phaseColliderObject; set => _phaseColliderObject = value; }

    public GameObject GetVisualGameObject()
    {
        return _gameObject;
    }

    void IPhase.CreateVisuals(GameObject parent)
    {
        // 1.) create GameObject that will hold visuals
        _gameObject = new GameObject();
        _gameObject.CreateTransform();
        _gameObject.SetActive(false);

        parent.AddChild(_gameObject);

        // 2.) assign animated sprite
        int frameCount = 2;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Drowner_P2", frameCount);
        float[] frameTimers = [0.4f, 0.4f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Drowner_P2", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        // 3.) assign collider objects
        _phaseColliderObject = new DrownerPhase2Colliders(parent: _gameObject, spriteAnimated: animatedSprite);
    }

    void IPhase.BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;

        // 1.) setup movement pattern
        movementMethod = movementMethods[0];

        float downDistance = 200;
        Vector2 startPos = bossEnemy.gameObject.transform.globalPosition;
        Vector2 ForthPos = new Vector2(startPos.X, startPos.Y - downDistance);
        Vector2 BackPos = new Vector2(startPos.X, startPos.Y + downDistance);
        float speedBackForth = 400;
        int backForthCount = int.MaxValue; // until defeated

        movementMethod.ResetSubsteps();

        movementMethod.SetParameters(
            ForthPos,
            BackPos,
            speedBackForth, //speed Forth
            speedBackForth, // speed Back
            backForthCount
        );

        bossEnemy.gameObject.transform.globalPosition = bossEnemy.ArrivalPosition; // teleport to arrival Position

        // 2.) setup attack pattern
        attackMethod = attackMethods[0];
        bossEnemy.currReloadTime = 1f; // ensure to instantly start attacking
        bossEnemy.maxReloadTime = 1f;

        float projectileDeathTimer = 10;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Drowner;
        int projectileCount = 5;
        Vector2 projectileScale = new Vector2(0.3f, 0.3f);
        int projectileSeparatorDistance = 72;
        bool isLeftToRight = true;
        bool hasProjectileTerrainImunity = false;

        attackMethod.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            projectileSeparatorDistance,
            isLeftToRight,
            hasProjectileTerrainImunity
        );

        // show visuals
        _gameObject.SetActive(true);
    }

    void IPhase.UpdatePhase(GameTime gameTime)
    {
        // 1.) update sprite effects
        animatedSprite.spriteEffects = (bossEnemy.gameObject.transform.globalPosition.X >= Player.Instance.gameObject.transform.globalPosition.X) ?
            SpriteEffects.FlipHorizontally : SpriteEffects.None;

        // 2.) update boss phase logic
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