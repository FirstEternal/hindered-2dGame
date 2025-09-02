using MGEngine.ObjectBased;
using MGEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class Enemy_Antiverse_EspAh : Enemy
{
    GameObject aggroSpriteObject;
    GameObject idleSpriteObject;
    public Enemy_Antiverse_EspAh(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
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
            CreateHealthBar(maxHealth: 40);
            //healthBar.LoadContent();
            CreateStateController();
            CreateVisuals();

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

        aggroSpriteObject.SetActive(false);
        idleSpriteObject.SetActive(true);

        activeSprite = idleSpriteObject.GetComponent<Sprite>();

        moveSpeed = 100;
    }

    private float aggroRange;
    protected override void CreateStateController()
    {
        aggroRange = 300;
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
        // Aggro sprite Object
        aggroSpriteObject = new GameObject();
        aggroSpriteObject.CreateTransform();

        // Idle sprite Object
        idleSpriteObject = new GameObject();
        idleSpriteObject.CreateTransform();

        gameObject.transform.localScale = new Vector2(0.3f, 0.3f);

        gameObject.AddChild(idleSpriteObject);
        gameObject.AddChild(aggroSpriteObject);

        Rectangle[] rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse EspAh", animatedSpriteCount: 2);

        SpriteAnimated spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: rectangles,
            frameTimers: [0.3f, 0.3f]
        );
        //AARectangleCollider rectangleCollider = new AARectangleCollider(
        OBBRectangleCollider rectangleCollider = new OBBRectangleCollider(
            width: spriteAnimated.sourceRectangle.Width * aggroSpriteObject.transform.globalScale.X,
            height: spriteAnimated.sourceRectangle.Height * aggroSpriteObject.transform.globalScale.Y,
            isAftermath: false
        );

        rectangleCollider.AddTagsToIgnoreList([
            GameConstantsAndValues.Tags.Hidden.ToString(),
            GameConstantsAndValues.Tags.EnemySpawned.ToString()
        ]);

        aggroSpriteObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();
        aggroSpriteObject.AddComponent(spriteAnimated);
        aggroSpriteObject.AddComponent(rectangleCollider);


        Rectangle rectangle = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse EspAh_IDLE", animatedSpriteCount: 1)[0];
        Sprite idleSprite = new Sprite(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            colorTint: Color.White);

        idleSprite.sourceRectangle = rectangle;
        //idleSprite.origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
        idleSprite.origin = JSON_Manager.GetEnemiesOrigin("AntiVerse EspAh_IDLE", animatedSpriteCount: 1, gameObject.transform.globalScale)[0];

        rectangleCollider = new OBBRectangleCollider(
            width: rectangle.Width * idleSpriteObject.transform.globalScale.X,
            height: rectangle.Height * idleSpriteObject.transform.globalScale.Y,
            isAftermath: false
        );
        idleSpriteObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();
        idleSpriteObject.AddComponent(idleSprite);
        idleSpriteObject.AddComponent(rectangleCollider);

        activeSprite = idleSprite;

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 0;

        boundsSprite = spriteAnimated;
    }

    Vector2 previousVelocity = Vector2.Zero;
    public override void Update(GameTime gameTime)
    {
        aggroSpriteObject.GetComponent<SpriteAnimated>().spriteEffects = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        stateAction?.Invoke();
        stateController.Update(gameTime);

        // check if velocity changed
        if (previousVelocity != Velocity)
        {

            idleSpriteObject.SetActive(Velocity == Vector2.Zero);
            aggroSpriteObject.SetActive(Velocity != Vector2.Zero);

            activeSprite = (Velocity == Vector2.Zero) ? idleSpriteObject.GetComponent<Sprite>() : aggroSpriteObject.GetComponent<Sprite>();
        }

        previousVelocity = Velocity;
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
