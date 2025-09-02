using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class Enemy_AntiVerse_ArrivalPortal : Enemy
{
    Scene scene;
    readonly float maxSpawnTimer;
    float currSpawnTimer;

    int currEnemySpawnCount;
    readonly int maxEnemySpawnCount;

    public Enemy_AntiVerse_ArrivalPortal(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        // TODO -> create hp bar
        maxSpawnTimer = 5;
        currSpawnTimer = 0;
        maxEnemySpawnCount = 10;

        damage = 0;

        this.isGravity = false;
        this.isMovable = false;
    }

    public Enemy_AntiVerse_ArrivalPortal(int enemySpawnCount = int.MaxValue, float spawnTimer = 5, float mass = 10000) : base(mass)
    {
        // TODO -> create hp bar
        maxSpawnTimer = spawnTimer;
        currSpawnTimer = 0;
        maxEnemySpawnCount = enemySpawnCount;

        isGravity = false;
        isMovable = false;
    }

    protected override void CreateStateController()
    {
        State state0 = new State(
            GameConstantsAndValues.States.IDLE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: true, x: 800, y: 300, Player.Instance.gameObject.transform, gameObject.transform)]);

        State state1 = new State(
            GameConstantsAndValues.States.AGGRESSIVE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: false, x: 800, y: 300, Player.Instance.gameObject.transform, gameObject.transform)]);
        stateController = new StateController(availableStates: [state0, state1]);

        // add stateController gameobject as a child
        stateAction = IdleAction;
        base.CreateStateController();
    }

    protected override void CreateVisuals()
    {
        gameObject.transform.localScale = new Vector2(0.3f, 0.3f);
        Rectangle rectangle = JSON_Manager.GetEnemiesSourceRectangles("AntiVerse ArrivalSpawn", animatedSpriteCount: 1)[0];

        Sprite sprite = new Sprite(texture2D: JSON_Manager.enemiesSpriteSheet, Color.White);
        sprite.sourceRectangle = rectangle;
        sprite.origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);

        //AARectangleCollider rectangleCollider = new AARectangleCollider(
        ParticleCollider collider = new ParticleCollider(
            radius: sprite.sourceRectangle.Height * gameObject.transform.globalScale.Y,
             isAftermath: false
        );

        gameObject.tag = GameConstantsAndValues.Tags.GravitationalEnemy.ToString();
        gameObject.AddComponent(sprite);
        gameObject.AddComponent(collider);

        activeSprite = sprite;

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 0;
    }


    public override void Update(GameTime gameTime)
    {
        if (stateAction == AggroAction)
        {
            currSpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currSpawnTimer <= 0)
            {
                currSpawnTimer = maxSpawnTimer;
                SpawnAntiVerse_Arrival();
            }
        }

        stateController.Update(gameTime);
    }

    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        base.ResetEnemy(spawnPosition);

        // reset hp...
        if (spawnPosition is not null)
        {
            gameObject.transform.spawnPosition = (Vector2)spawnPosition;
        }
        if (healthBar is null)
        {
            CreateHealthBar(maxHealth: 20);
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

        currSpawnTimer = maxSpawnTimer;
        currEnemySpawnCount = maxEnemySpawnCount;

        // move to spawn location
        gameObject.transform.globalPosition = gameObject.transform.spawnPosition;
    }

    private void SpawnAntiVerse_Arrival()
    {
        if (currEnemySpawnCount == 0) return;
        if (scene == null)
        {
            scene = SceneManager.Instance.GetGameObjectScene(gameObject);
        }

        GameObject arrivalObject = new GameObject();
        arrivalObject.CreateTransform();

        Vector2 offset = new Vector2(50, -30) * gameObject.transform.globalScale;
        arrivalObject.transform.globalPosition = gameObject.transform.globalPosition + offset;

        scene.AddGameObjectToScene(arrivalObject, isOverlay: false);

        Enemy_AntiVerse_Arrival arrival = new Enemy_AntiVerse_Arrival(mass: 100);

        arrivalObject.AddComponent(arrival);

        // spawn facing -> sprite of this object
        arrival.Spawn(direction_X: gameObject.GetComponent<Sprite>().spriteEffects == Microsoft.Xna.Framework.Graphics.SpriteEffects.None ? 1 : -1,
            spawnPosition: new Vector2(gameObject.transform.globalPosition.X + offset.X, gameObject.transform.globalPosition.Y));

        currEnemySpawnCount--;
    }
}
