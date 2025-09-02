using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class Phase1_BossEnemy_DragonicBurner : IPhase
{
    private GameObject _gameObject;
    private IBossMethod movementMethod;
    //private IBossMethod attackMethod;

    private BossEnemy bossEnemy;

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
        int frameCount = 3;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Burner_P1", frameCount);
        float[] frameTimers = [0.5f, 0.5f, 0.5f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Burner_P1", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        _phaseColliderObject = new BurnerPhase1Colliders(parent: _gameObject, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    void IPhase.BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;

        // 1.) setup movement pattern
        int higherStart = 100;
        bossEnemy.gameObject.transform.globalPosition =
            new Vector2(bossEnemy.gameObject.transform.globalPosition.X, bossEnemy.gameObject.transform.globalPosition.Y - higherStart);
        Vector2 startPos = bossEnemy.gameObject.transform.globalPosition;

        int downDistance = 300;
        int leftRightDistance = 600;
        Vector2 downPos = new Vector2(startPos.X, startPos.Y + downDistance);

        Vector2 leftPos = new Vector2(startPos.X - leftRightDistance, downPos.Y);
        Vector2 rightPos = new Vector2(startPos.X + leftRightDistance, downPos.Y);

        float speedDown = 200;
        float speedLeftRight = 900;
        int bounceLeftRightCount = 4;

        float dmgReductionDuringBounce = 1;

        movementMethod = movementMethods[0];

        movementMethod.ResetSubsteps();

        movementMethod.SetParameters(
            startPos,
            downPos,
            leftPos,
            rightPos,
            speedDown,
            speedLeftRight, // speed left
            speedLeftRight, // speed right
            bounceLeftRightCount,
            false, // reversed order
            dmgReductionDuringBounce // full reduction
        );

        // 2.) setup attack pattern
        //attackMethod = null; // phase 0 does not declare attacks

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
                // pause animation only once per recovery(at the start)
                int lastFrameIndex = 2;
                animatedSprite.SetFrame(frameIndex: lastFrameIndex);
                animatedSprite.PauseAnimation();
            }
        }
        else
        {
            movementMethod?.Execute();
            _gameObject.GetComponent<SpriteAnimated>().ResumeAnimation();
        }

        _phaseColliderObject.UpdateColliders();
    }

    void IPhase.Reload(GameTime gameTime) { return; } // no attacking -> no reloading requiered
}