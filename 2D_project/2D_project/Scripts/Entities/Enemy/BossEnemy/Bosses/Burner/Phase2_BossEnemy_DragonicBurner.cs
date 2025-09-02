using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

internal class Phase2_BossEnemy_DragonicBurner : IPhase
{
    private GameObject _gameObject;
    private IBossMethod movementMethod;
    private IBossMethod attackMethod;

    private float currReloadTime = 1;
    private float maxReloadTime = 1;
    private bool isReloading = false;

    SpriteAnimated animatedSprite;

    BossEnemy bossEnemy;

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

        parent.AddChild(_gameObject);

        // 2.) assign animated sprite
        int frameCount = 3;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Burner_P2", frameCount);
        float[] frameTimers = [maxReloadTime / frameCount, maxReloadTime / frameCount, maxReloadTime / frameCount];//, maxReloadTime / frameCount];

        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Burner_P2", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);

        // 3.) assign collider
        _phaseColliderObject = new BurnerPhase2Colliders(parent: parent, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    void IPhase.BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;
        bossEnemy.currReloadTime = 1f; // ensure to instantly start attacking
        bossEnemy.maxReloadTime = 1f;

        // 1.) setup movement pattern
        movementMethod = movementMethods[0];

        int downDistance = 200;
        int leftRightDistance = 600;
        Vector2 startPos = bossEnemy.gameObject.transform.globalPosition;
        Vector2 downPos = new Vector2(startPos.X, startPos.Y + downDistance);
        Vector2 ForthPos = new Vector2(startPos.X - leftRightDistance, startPos.Y);
        Vector2 BackPos = new Vector2(startPos.X + leftRightDistance, startPos.Y);

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

        //isAttacking = true;

        // 2.) setup attack pattern
        attackMethod = attackMethods[0];

        float projectileDeathTimer = 10;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Burner;
        int projectileCount = 1;
        Vector2 projectileScale = new Vector2(0.5f, 0.5f);
        int projectileSeparatorDistance = 0;
        /*
        currReloadTime = 1; 
        maxReloadTime = 1;
        isReloading = false;*/

        bool hasProjectileTerrainImunity = false;

        // only 1 attack, no substeps
        attackMethod.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            projectileSeparatorDistance,
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
