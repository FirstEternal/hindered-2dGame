using MGEngine.ObjectBased;
using MGEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class Enemy_AntiVerse_EeaLt : Enemy
{
    //bool isSpawned;
    private float currAttackTimer;
    private float maxAttackTimer = 0.4f;
    private GameObject leftScyteSpriteObject;
    private GameObject rightScyteSpriteObject;
    private GameObject bodySpriteObject;
    private GameObject scyteColliderObject;

    //private bool waitingForAttackAnimation = false;

    public Enemy_AntiVerse_EeaLt(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
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
            CreateHealthBar(maxHealth: 120, maxShield: 40);
            CreateVisuals();
            CreateStateController();

            LoadContent();
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

        //isSpawned = false;
        moveSpeed = 50;
    }

    private float aggroRange;
    protected override void CreateStateController()
    {
        aggroRange = 600;
        State state0 = new State(
            GameConstantsAndValues.States.IDLE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: true, x: aggroRange, y: 250, Player.Instance.gameObject.transform, gameObject.transform)]);

        State state1 = new State(
            GameConstantsAndValues.States.AGGRESSIVE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: false, x: aggroRange, y: 250, Player.Instance.gameObject.transform, gameObject.transform)]);
        stateController = new StateController(availableStates: [state0, state1]);

        // add stateController gameobject as a child
        stateAction = IdleAction;
        base.CreateStateController();
    }

    protected override void CreateVisuals()
    {
        // Aggro sprite Object
        leftScyteSpriteObject = new GameObject();
        leftScyteSpriteObject.CreateTransform();

        rightScyteSpriteObject = new GameObject();
        rightScyteSpriteObject.CreateTransform();

        // Idle sprite Object
        bodySpriteObject = new GameObject();
        bodySpriteObject.CreateTransform();

        gameObject.transform.localScale = new Vector2(0.6f, 0.6f);

        gameObject.AddChild(leftScyteSpriteObject);
        gameObject.AddChild(bodySpriteObject);
        gameObject.AddChild(rightScyteSpriteObject);

        Rectangle[] left_rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse EeaLt_scyte_L", animatedSpriteCount: 4);
        Rectangle[] right_rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse EeaLt_scyte_R", animatedSpriteCount: 4);

        SpriteAnimated left_scyteSprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: left_rectangles,
            frameTimers: [maxAttackTimer / 4, maxAttackTimer / 4, maxAttackTimer / 4, maxAttackTimer / 4],
            origins: JSON_Manager.GetEnemiesOrigin("AntiVerse EeaLt_scyte_L", animatedSpriteCount: 4, gameObject.transform.globalScale)
        );

        SpriteAnimated right_scyteSprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: right_rectangles,
            frameTimers: [maxAttackTimer / 4, maxAttackTimer / 4, maxAttackTimer / 4, maxAttackTimer / 4],
            origins: JSON_Manager.GetEnemiesOrigin("AntiVerse EeaLt_scyte_R", animatedSpriteCount: 4, gameObject.transform.globalScale)
        );

        left_scyteSprite.loopEnabled = false;
        right_scyteSprite.loopEnabled = false;

        leftScyteSpriteObject.AddComponent(left_scyteSprite);
        rightScyteSpriteObject.AddComponent(right_scyteSprite);

        Rectangle[] body_rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse EeaLt", animatedSpriteCount: 3);
        SpriteAnimated bodySprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: body_rectangles,
            frameTimers: [0.3f, 0.3f, 0.3f],
            origins: JSON_Manager.GetEnemiesOrigin("AntiVerse EeaLt", animatedSpriteCount: 3, gameObject.transform.globalScale)
        );

        bodySpriteObject.AddComponent(bodySprite);

        activeSprite = bodySprite;
        boundsSprite = activeSprite;

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 0;

        // colliders
        GameObject bodyColliderObject = new GameObject();
        bodyColliderObject.CreateTransform(localPosition: new Vector2(0, 40));
        gameObject.AddChild(bodyColliderObject);

        boundsRectangle = new Rectangle(0, 0, width: (int)(200 * gameObject.transform.globalScale.X), height: (int)(250 * gameObject.transform.globalScale.Y));
        OBBRectangleCollider bodyRectangleCollider = new OBBRectangleCollider(
            width: 200 * bodySpriteObject.transform.globalScale.X,
            height: 80 * bodySpriteObject.transform.globalScale.Y,
            isAftermath: false
        );

        bodyColliderObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();
        bodyColliderObject.AddComponent(bodyRectangleCollider);

        scyteColliderObject = new GameObject();
        scyteColliderObject.CreateTransform(localPosition: new Vector2(150, 30));
        gameObject.AddChild(scyteColliderObject);
        OBBRectangleCollider scyteCollider = new OBBRectangleCollider(
            width: 90 * gameObject.transform.globalScale.X,
            height: 120 * gameObject.transform.globalScale.X,
            isAftermath: false,
            isRelaxPosition: false
        );
        scyteColliderObject.tag = GameConstantsAndValues.Tags.EnemyMeleeSpawned.ToString();
        scyteColliderObject.AddComponent(scyteCollider);
        scyteCollider.onCollision += Attack_OnScyteColliderCollision;

        scyteCollider.AddTagsToIgnoreList([
            GameConstantsAndValues.Tags.PlayerSpawned.ToString(),
            GameConstantsAndValues.Tags.Enemy.ToString(),
            GameConstantsAndValues.Tags.EnemySpawned.ToString(),
            GameConstantsAndValues.Tags.Hidden.ToString(),
            GameConstantsAndValues.Tags.Terrain.ToString()
        ]);
    }

    private void Attack_OnScyteColliderCollision(object sender, EventArgs e)
    {
        if (!isAlive) return;

        if (currAttackTimer <= 0)
        {
            currAttackTimer = maxAttackTimer;

            leftScyteSpriteObject.GetComponent<SpriteAnimated>().SetFrame(0);
            leftScyteSpriteObject.GetComponent<SpriteAnimated>().ResumeAnimation();

            rightScyteSpriteObject.GetComponent<SpriteAnimated>().SetFrame(0);
            rightScyteSpriteObject.GetComponent<SpriteAnimated>().ResumeAnimation();
        }
    }

    SpriteEffects previousSpriteEffects = SpriteEffects.None;
    public override void Update(GameTime gameTime)
    {
        if (stateAction == AggroAction)
        {
            currAttackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (leftScyteSpriteObject.GetComponent<SpriteAnimated>().currFrameIndex == 2)
            {
                // apply knockback and damage to player
                StatChangeFunctions.PlayerDamageCalculation(enemy: this);
                Player.Instance.BeginKnockBack(0.8f, direction: Vector2.Normalize(Velocity));
            }
        }

        SpriteEffects newSpriteEffects = Velocity.X <= 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


        if (previousSpriteEffects != newSpriteEffects)
        {
            bodySpriteObject.GetComponent<SpriteAnimated>().spriteEffects = newSpriteEffects;
            leftScyteSpriteObject.GetComponent<SpriteAnimated>().spriteEffects = newSpriteEffects;
            rightScyteSpriteObject.GetComponent<SpriteAnimated>().spriteEffects = newSpriteEffects;
            scyteColliderObject.transform.localPosition = new Vector2(-scyteColliderObject.transform.localPosition.X, scyteColliderObject.transform.localPosition.Y);
        }

        // todo if hits a terrain end change position
        stateAction?.Invoke();
        stateController.Update(gameTime);

        previousSpriteEffects = newSpriteEffects;
    }

    protected override void AggroAction()
    {
        AssignMovementDirectionX(Player.Instance.gameObject.transform.globalPosition.X);
        if (Vector2.Distance(Player.Instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > aggroRange)
        {
            stateAggression = StateAggression.idle;
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
        }
    }

    private void AssignMovementDirectionX(float posX)
    {
        float direction = (posX - gameObject.transform.globalPosition.X > 0) ? 1 : -1;
        Velocity = new Vector2(direction * moveSpeed, 0);
    }
}