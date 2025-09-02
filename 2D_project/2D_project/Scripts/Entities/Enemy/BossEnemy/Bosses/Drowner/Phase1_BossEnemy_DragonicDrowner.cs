using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class Phase1_BossEnemy_DragonicDrowner : IPhase
{
    protected GameObject _gameObject;
    private IBossMethod movementMethod1;
    private IBossMethod movementMethod2;

    protected delegate void MovementAction(float? deltaTime = null);
    protected MovementAction ExecuteMovement;

    //private IBossMethod attackMethod;

    protected BossEnemy_DragonicDrowner bossEnemy;

    private Vector2 UpPosition;
    private Vector2 DownPosition;

    protected float phase1FrameTimer = 0.4f;

    SpriteAnimated animatedSprite;

    protected PhaseColliderObject _phaseColliderObject;
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
        int frameCount = 3;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Drowner_P1", frameCount);
        float[] frameTimers = [phase1FrameTimer, phase1FrameTimer, phase1FrameTimer];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Drowner_P1", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);

        // 3.) assign collider objects
        _phaseColliderObject = new DrownerPhase1Colliders(parent: _gameObject, spriteAnimated: animatedSprite);
    }

    public virtual void BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = (BossEnemy_DragonicDrowner)bossEnemy;

        // 1.) setup movement pattern
        movementMethod1 = movementMethods[0]; // phase 0 does not declare attacks
        movementMethod2 = movementMethods[1]; // phase 0 does not declare attacks

        Vector2 startPos = bossEnemy.gameObject.transform.globalPosition;

        int upDistance = 100;
        int downDistance = 200;
        int leftRightDistance = 600;

        UpPosition = new Vector2(startPos.X, startPos.Y - upDistance);
        DownPosition = new Vector2(startPos.X, startPos.Y + downDistance);

        Vector2 leftBottomPos = new Vector2(startPos.X - leftRightDistance, DownPosition.Y);
        Vector2 rightBottomPos = new Vector2(startPos.X + leftRightDistance, DownPosition.Y);

        Vector2 leftTopPos = new Vector2(startPos.X - leftRightDistance, UpPosition.Y);
        Vector2 rightTopPos = new Vector2(startPos.X + leftRightDistance, UpPosition.Y);

        float speedDown = 200;
        float speedLeftRight = 900;
        int bounceLeftRightCount = 4;

        float dmgReductionDuringBounce = 0;

        // top lane
        movementMethod1.ResetSubsteps();
        movementMethod1.SetParameters(
            startPos,
            UpPosition,
            leftTopPos,
            rightTopPos,
            speedDown,
            speedLeftRight, // speed left
            speedLeftRight, // speed right
            bounceLeftRightCount,
            true, // reversed order -> starts going up then down
            dmgReductionDuringBounce // no reduction
        );

        // bottom lane
        movementMethod2.ResetSubsteps();
        movementMethod2.SetParameters(
            startPos,
            DownPosition,
            leftBottomPos,
            rightBottomPos,
            speedDown,
            speedLeftRight, // speed left
            speedLeftRight, // speed right
            bounceLeftRightCount,
            false, // original order -> starts going down then up
            dmgReductionDuringBounce // no reduction
        );

        // 2.) setup attack pattern
        //this.attackMethod = null; // phase 0 does not declare attacks

        //isMovementAssigned = false;

        // show visuals
        _gameObject.SetActive(true);
    }

    void IPhase.UpdatePhase(GameTime gameTime)
    {
        // 2.) update boss phase logic
        if (bossEnemy.isRecovering)
        {
            // 2.1.) update sprite effects
            animatedSprite.spriteEffects = (bossEnemy.gameObject.transform.globalPosition.X >= Player.Instance.gameObject.transform.globalPosition.X) ?
                SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!animatedSprite.isAnimationPaused)
            {
                int lastFrameIndex = 2;
                animatedSprite.SetFrame(frameIndex: lastFrameIndex);
                animatedSprite.PauseAnimation();
                bossEnemy.isMovementDecided = false;
            }
        }
        else
        {
            animatedSprite.ResumeAnimation();

            if (!bossEnemy.isMovementDecided)
            {
                AssignMovement();
                return;
            }
            ExecuteMovement?.Invoke();
        }

        _phaseColliderObject.UpdateColliders();
    }

    protected void AssignMovement()
    {
        float playerPosY = Player.Instance.gameObject.transform.globalPosition.Y;
        bool isCloserToTopPos = MathF.Abs(playerPosY - UpPosition.Y) < MathF.Abs(playerPosY - DownPosition.Y);

        if (isCloserToTopPos)
        {
            // up -> bounce: left/right -> down
            ExecuteMovement = movementMethod1.Execute;
        }
        else
        {
            // down -> bounce: left/right -> up
            ExecuteMovement = movementMethod2.Execute;
        }
        bossEnemy.isMovementDecided = true;
    }

    void IPhase.Reload(GameTime gameTime) { return; } // no attacking -> no reloading requiered
}