using GamePlatformer;
using MGEngine.ObjectBased;
using MGEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

internal class Enemy_Antimatter_Vher_tou : Enemy
{
    GameObject[] BodySpriteObjects = new GameObject[4]; // 0 -> idle, 1,2,3 are attack patterns

    private int activeBodyIndex;
    private float currAttackCooldown;
    private float maxAttackCooldown = 3f;

    private void AssignRandomMeleeAttack()
    {
        int[] values = { 2, 3 };
        activeBodyIndex = values[Game2DPlatformer.Instance.random.Next(values.Length)];
        SetBodyActive(activeBodyIndex);
    }

    private void SetBodyActive(int bodyIndex)
    {
        activeBodyIndex = bodyIndex;

        for (int i = 0; i < BodySpriteObjects.Length; i++)
        {
            BodySpriteObjects[i].SetActive(i == activeBodyIndex);
        }

        BodySpriteObjects[activeBodyIndex].GetComponent<SpriteAnimated>().SetFrame(0);
        BodySpriteObjects[activeBodyIndex].GetComponent<SpriteAnimated>().ResumeAnimation();
    }

    public Enemy_Antimatter_Vher_tou(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = isGravity;
        this.isMovable = isMovable;
    }

    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        base.ResetEnemy(spawnPosition);
        if (spawnPosition is not null)
        {
            gameObject.transform.spawnPosition = (Vector2)spawnPosition;
        }
        if (healthBar is null)
        {
            CreateHealthBar(maxHealth: 150, maxShield: 110);
            CreateStateController();
            CreateVisuals();

            LoadContent();

            knockBackImunity = false;
            damage = 5;
            critRate = 0.3f;
            critMultiplier = 0.25f;
            knockBackForce = 0;
        }
        else
        {
            healthBar.currHealth = healthBar.maxHealth;
            healthBar.currShield = healthBar.maxShield;
        }

        totalPhases = 2;
        base.currentPhase = -1; // state before arrival

        // move to spawn location
        gameObject.transform.globalPosition = gameObject.transform.spawnPosition;

        knockBackImunity = false;
        damage = 5;
        critRate = 0.5f;
        critMultiplier = 0.25f;
        knockBackForce = 0;

        BodySpriteObjects[0].SetActive(true);
        BodySpriteObjects[1].SetActive(false);
        BodySpriteObjects[2].SetActive(false);
        BodySpriteObjects[3].SetActive(false);

        activeSprite = BodySpriteObjects[0].GetComponent<Sprite>();

        moveSpeed = 100;
    }

    private float aggroRange;
    protected override void CreateStateController()
    {
        aggroRange = 600;
        State state0 = new State(
            GameConstantsAndValues.States.IDLE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: true, x: aggroRange, y: 100, Player.Instance.gameObject.transform, gameObject.transform)]);

        State state1 = new State(
            GameConstantsAndValues.States.AGGRESSIVE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: false, x: aggroRange, y: 100, Player.Instance.gameObject.transform, gameObject.transform)]);
        stateController = new StateController(availableStates: [state0, state1]);

        // add stateController gameobject as a child
        stateAction = IdleAction;
        base.CreateStateController();
    }

    protected override void CreateVisuals()
    {
        gameObject.transform.localScale = new Vector2(0.5f, 0.5f);

        // moving
        BodySpriteObjects[0] = new GameObject();
        BodySpriteObjects[0].CreateTransform();
        gameObject.AddChild(BodySpriteObjects[0]);

        SpriteAnimated spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: JSON_Manager.GetEnemiesSourceRectangles("Antimatter Vher'tou_IDLE", animatedSpriteCount: 3),
            frameTimers: [0.3f, 0.3f, 0.3f]
        );
        activeSprite = spriteAnimated;
        BodySpriteObjects[0].AddComponent(spriteAnimated);

        // body attack 1
        BodySpriteObjects[1] = new GameObject();
        BodySpriteObjects[1].CreateTransform();
        gameObject.AddChild(BodySpriteObjects[1]);

        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: JSON_Manager.GetEnemiesSourceRectangles("Antimatter Vher'tou_A", animatedSpriteCount: 4),
            frameTimers: [0.2f, 0.2f, 0.2f, 0.3f]
        );
        spriteAnimated.loopEnabled = false;
        BodySpriteObjects[1].AddComponent(spriteAnimated);

        // body attack 2
        BodySpriteObjects[2] = new GameObject();
        BodySpriteObjects[2].CreateTransform();
        gameObject.AddChild(BodySpriteObjects[2]);

        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: JSON_Manager.GetEnemiesSourceRectangles("Antimatter Vher'tou_B", animatedSpriteCount: 3),
            frameTimers: [0.15f, 0.3f, 0.15f]
            );
        spriteAnimated.loopEnabled = false;
        BodySpriteObjects[2].AddComponent(spriteAnimated);

        // body attack 3
        BodySpriteObjects[3] = new GameObject();
        BodySpriteObjects[3].CreateTransform();
        gameObject.AddChild(BodySpriteObjects[3]);

        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: JSON_Manager.GetEnemiesSourceRectangles("Antimatter Vher'tou_C", animatedSpriteCount: 5),
            frameTimers: [0.1f, 0.1f, 0.1f, 0.1f, 0.1f]
        );
        spriteAnimated.loopEnabled = false;
        BodySpriteObjects[3].AddComponent(spriteAnimated);

        //collider
        OBBRectangleCollider rectangleCollider = new OBBRectangleCollider(
            width: 100 * gameObject.transform.globalScale.X,
            height: spriteAnimated.sourceRectangle.Height * 0.8f * gameObject.transform.globalScale.Y,
            isAftermath: false
        );

        rectangleCollider.AddTagsToIgnoreList([
            GameConstantsAndValues.Tags.PlayerSpawned.ToString(),
            GameConstantsAndValues.Tags.Player.ToString()
        ]);
        gameObject.AddComponent(rectangleCollider);
        gameObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();

        boundsSprite = activeSprite;

    }

    Vector2 previousVelocity = Vector2.Zero;
    public override void Update(GameTime gameTime)
    {
        SpriteAnimated spriteAnimated = BodySpriteObjects[activeBodyIndex].GetComponent<SpriteAnimated>();
        spriteAnimated.spriteEffects = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        if (activeBodyIndex == 1 && spriteAnimated.currFrameIndex == 2)
        {
            // shoot a projectile
            // TODO
        }
        if (spriteAnimated.isAnimationPaused)
        {
            // attack pattern animation ended
            spriteAnimated.SetFrame(0);
            isMovable = true; ;
            SetBodyActive(0);
        }
        stateAction?.Invoke();
        stateController.Update(gameTime);

        currAttackCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (currAttackCooldown < 0)
        {
            currAttackCooldown = maxAttackCooldown;
            Debug.WriteLine("attack patern assigned");

            if (Math.Abs(Player.Instance.gameObject.transform.globalPosition.X - gameObject.transform.globalPosition.X) < 100)
            {
                AssignRandomMeleeAttack();
            }
            else
            {
                SetBodyActive(1); // long ranged attack
            }

            isMovable = false;
        }
    }

    protected override void AggroAction()
    {
        AssignMovementDirectionX(Player.Instance.gameObject.transform.globalPosition.X);
        if (Vector2.Distance(Player.Instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > aggroRange)
        {
            stateAggression = StateAggression.idle;
            SetBodyActive(bodyIndex: 0); // idle body

            BodySpriteObjects[0].GetComponent<SpriteAnimated>().PauseAnimation();

            currAttackCooldown = int.MaxValue;
        }
    }

    protected override void IdleAction()
    {
        //Debug.WriteLine("idle");
        bool isInPosition = Movement.IsInPosition(gameObject.transform.spawnPosition, gameObject.transform, 30);

        if (!isInPosition)
        {
            // move away from player
            AssignMovementDirectionX(gameObject.transform.spawnPosition.X);
        }
        else
        {
            Velocity = Vector2.Zero;
            //gameObject.GetComponent<SpriteAnimated>().animationEnabled = false;
        }

        // check if player is within aggro range
        if (Vector2.Distance(Player.Instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) <= aggroRange)
        {
            stateAggression = StateAggression.aggro;
            currAttackCooldown = maxAttackCooldown;

            BodySpriteObjects[0].GetComponent<SpriteAnimated>().ResumeAnimation();
        }
    }

    private void AssignMovementDirectionX(float posX)
    {
        float direction = (posX - gameObject.transform.globalPosition.X > 0) ? 1 : -1;
        Velocity = new Vector2(direction * moveSpeed, 0);
    }
}