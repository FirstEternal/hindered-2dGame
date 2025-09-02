using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

internal class Phase3_BossEnemy_DragonicDrowner : Phase1_BossEnemy_DragonicDrowner, IPhase
{
    //IBossMethod movementMethod1;
    //IBossMethod movementMethod2;
    private IBossMethod attackMethod;


    private float currReloadTime = 1;
    private float maxReloadTime;
    private bool isReloading = false;

    SpriteAnimated animatedSprite;

    void IPhase.CreateVisuals(GameObject parent)
    {
        // phase 1 has exact same visuals
        //throw new System.NotImplementedException();

        // 1.) create GameObject that will hold visuals
        _gameObject = new GameObject();
        _gameObject.CreateTransform();
        _gameObject.SetActive(false);

        parent.AddChild(_gameObject);

        // 2.) assign animated sprite
        int frameCount = 5;
        maxReloadTime = 4.5f * phase1FrameTimer;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Drowner_P3", frameCount);
        float[] frameTimers = [phase1FrameTimer, phase1FrameTimer, phase1FrameTimer, phase1FrameTimer, phase1FrameTimer];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Drowner_P3", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);

        // 3.) assign collider objects
        _phaseColliderObject = new DrownerPhase3Colliders(parent: _gameObject, spriteAnimated: animatedSprite);
    }

    public override void BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        bossEnemy.gameObject.transform.globalPosition = bossEnemy.ArrivalPosition; // teleport to arrival Position                                                                                
        //bossEnemy.BeginRecovering();


        //this.bossEnemy = bossEnemy;

        // 1.) setup movement pattern
        base.BeginPhase(bossEnemy, movementMethods, attackMethods);
        // movement parameters from phase1  -> no need for setting up parameters
        base.BeginPhase(bossEnemy, movementMethods, attackMethods);

        // 2.) setup attack pattern
        attackMethod = attackMethods[0]; // phase 0 does not declare attacks

        float projectileDeathTimer = 10;
        GameConstantsAndValues.FactionType projectileType = GameConstantsAndValues.FactionType.Drowner;
        int projectileCount = 12;
        Vector2 projectileScale = new Vector2(0.2f, 0.2f);
        int degreeStart = 0;
        int degreeEnd = 360;
        bool isLeftToRight = true;

        bossEnemy.maxReloadTime = maxReloadTime;
        bossEnemy.currReloadTime = 0; // 3 * phase1FrameTimer;

        bool hasProjectileTerrainImunity = false;

        attackMethod.SetParameters(
            projectileDeathTimer,
            projectileType,
            projectileCount,
            projectileScale,
            degreeStart,
            degreeEnd,
            isLeftToRight,
            hasProjectileTerrainImunity
        );

        // show visuals
        _gameObject.SetActive(true);
    }

    void IPhase.UpdatePhase(GameTime gameTime)
    {
        // 2.) update boss phase logic
        if (!bossEnemy.isMovementDecided)
        {
            AssignMovement();
            return;
        }
        ExecuteMovement?.Invoke();

        Reload(gameTime);
        if (!isReloading)
        {
            attackMethod?.Execute();
            isReloading = true;
        }
        // play only first 3 frames during the non recovery animation
        //if (animatedSprite.currFrameIndex == 3) animatedSprite.SetFrame(0);

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
