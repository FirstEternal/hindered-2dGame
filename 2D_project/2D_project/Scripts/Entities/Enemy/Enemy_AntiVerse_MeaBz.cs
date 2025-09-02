using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class Enemy_AntiVerse_MeaBz : Enemy
{
    public Enemy_AntiVerse_MeaBz(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = false, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = false;
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

        moveSpeed = 100;
    }

    protected override void CreateStateController()
    {
        State state0 = new State(
            GameConstantsAndValues.States.IDLE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: true, x: 500, y: 300, Player.Instance.gameObject.transform, gameObject.transform)]);

        State state1 = new State(
            GameConstantsAndValues.States.AGGRESSIVE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: false, x: 500, y: 300, Player.Instance.gameObject.transform, gameObject.transform)]);
        stateController = new StateController(availableStates: [state0, state1]);

        // add stateController gameobject as a child
        stateAction = IdleAction;
        base.CreateStateController();
    }

    protected override void CreateVisuals()
    {
        gameObject.transform.localScale = new Vector2(0.3f, 0.3f);
        Rectangle[] rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse MeaBz", animatedSpriteCount: 4);

        SpriteAnimated spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: rectangles,
            frameTimers: [0.2f, 0.2f, 0.2f, 0.2f]
        );
        //AARectangleCollider rectangleCollider = new AARectangleCollider(
        OBBRectangleCollider rectangleCollider = new OBBRectangleCollider(
            width: spriteAnimated.sourceRectangle.Width * gameObject.transform.globalScale.X,
            height: spriteAnimated.sourceRectangle.Height * gameObject.transform.globalScale.Y,
            isAftermath: false
        );

        activeSprite = spriteAnimated;
        boundsSprite = spriteAnimated;

        gameObject.tag = GameConstantsAndValues.Tags.GravitationalEnemy.ToString();
        gameObject.AddComponent(spriteAnimated);
        gameObject.AddComponent(rectangleCollider);

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 0;

        base.LoadContent();
    }

    //Vector2 previousVelocity = Vector2.Zero;
    public override void Update(GameTime gameTime)
    {
        gameObject.GetComponent<SpriteAnimated>().spriteEffects =
            gameObject.transform.globalPosition.X > Player.Instance.gameObject.transform.globalPosition.X ?
            SpriteEffects.FlipHorizontally : SpriteEffects.None;
        stateAction?.Invoke();
        stateController.Update(gameTime);
    }

    protected override void AggroAction()
    {
        Vector2 thisPos = gameObject.transform.globalPosition;
        Vector2 playerPos = Player.Instance.gameObject.transform.globalPosition;

        float minDifference = 20;
        if (Math.Abs(playerPos.X - thisPos.X) <= minDifference)
        {
            AssignMovementDirectionY(playerPos.Y);
            return;
        }

        AssignMovementDirectionX(playerPos.X);
    }

    protected override void IdleAction()
    {
        //Debug.WriteLine("idle");
        Velocity = Vector2.Zero;
    }

    private void AssignMovementDirectionX(float posX)
    {
        float direction = (posX - gameObject.transform.globalPosition.X > 0) ? 1 : -1;
        Velocity = new Vector2(direction * moveSpeed, 0);
    }

    private void AssignMovementDirectionY(float posY)
    {
        float direction = (posY - gameObject.transform.globalPosition.Y > 0) ? 1 : -1;
        Velocity = new Vector2(0, direction * moveSpeed);
    }
}
