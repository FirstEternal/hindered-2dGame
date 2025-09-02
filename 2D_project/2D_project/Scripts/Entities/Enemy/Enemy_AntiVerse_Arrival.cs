using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class Enemy_AntiVerse_Arrival : Enemy
{
    bool isSpawned;
    private float currSpawnAnimationTimer;
    private float spawnAnimationTimer = 0.8f;
    public GameObject spawnSpriteObject;
    public GameObject afterSpawnSpriteObject;

    public void Spawn(float direction_X, Vector2? spawnPosition = null)
    {
        ResetEnemy(spawnPosition);
        isMovable = false;
        isGravity = false;
        Velocity = new Vector2(direction_X * moveSpeed, 0);

        isSpawned = false;
        activeSprite = spawnSpriteObject.GetComponent<Sprite>();
        spawnSpriteObject.GetComponent<SpriteAnimated>().spriteEffects = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        afterSpawnSpriteObject.GetComponent<SpriteAnimated>().spriteEffects = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        currSpawnAnimationTimer = spawnAnimationTimer;
    }
    public Enemy_AntiVerse_Arrival(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
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
            CreateHealthBar(maxHealth: 15);
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

        spawnSpriteObject.SetActive(true);
        afterSpawnSpriteObject.SetActive(false);

        knockBackImunity = false;
        damage = 5;
        critRate = 0.5f;
        critMultiplier = 0.25f;
        knockBackForce = 0;

        isSpawned = false;
        moveSpeed = 200;
    }

    protected override void CreateVisuals()
    {
        // Aggro sprite Object
        spawnSpriteObject = new GameObject();
        spawnSpriteObject.CreateTransform();

        // Idle sprite Object
        afterSpawnSpriteObject = new GameObject();
        afterSpawnSpriteObject.CreateTransform();

        gameObject.transform.localScale = new Vector2(0.3f, 0.3f);

        gameObject.AddChild(afterSpawnSpriteObject);
        gameObject.AddChild(spawnSpriteObject);

        Rectangle[] rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse Arrival_Spawn", animatedSpriteCount: 4);

        SpriteAnimated spawnSprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: rectangles,
            frameTimers: [spawnAnimationTimer / 4, spawnAnimationTimer / 4, spawnAnimationTimer / 4, spawnAnimationTimer / 4]
        );
        //AARectangleCollider rectangleCollider = new AARectangleCollider(
        OBBRectangleCollider rectangleCollider = new OBBRectangleCollider(
            width: spawnSprite.sourceRectangle.Width * spawnSpriteObject.transform.globalScale.X,
            height: spawnSprite.sourceRectangle.Height * spawnSpriteObject.transform.globalScale.Y,
            isAftermath: false
        );

        spawnSpriteObject.AddComponent(spawnSprite);
        spawnSpriteObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();
        spawnSpriteObject.AddComponent(rectangleCollider);


        rectangles = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse Arrival", animatedSpriteCount: 2);
        SpriteAnimated afterSpawnSprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: rectangles,
            frameTimers: [0.3f, 0.3f]
        );


        rectangleCollider = new OBBRectangleCollider(
            width: afterSpawnSprite.sourceRectangle.Width * afterSpawnSpriteObject.transform.globalScale.X,
            height: afterSpawnSprite.sourceRectangle.Height * afterSpawnSpriteObject.transform.globalScale.Y,
            isAftermath: false
        );
        afterSpawnSpriteObject.AddComponent(afterSpawnSprite);
        afterSpawnSpriteObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();
        afterSpawnSpriteObject.AddComponent(rectangleCollider);

        boundsSprite = afterSpawnSprite;

        activeSprite = afterSpawnSprite;

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 0;
    }
    public override void Update(GameTime gameTime)
    {
        if (isSpawned) return;

        currSpawnAnimationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currSpawnAnimationTimer <= 0)
        {
            isSpawned = true;
            //return;
            isMovable = true;
            isGravity = true;

            activeSprite = afterSpawnSpriteObject.GetComponent<Sprite>();

            spawnSpriteObject.SetActive(false);
            afterSpawnSpriteObject.SetActive(true);
        }
    }
}