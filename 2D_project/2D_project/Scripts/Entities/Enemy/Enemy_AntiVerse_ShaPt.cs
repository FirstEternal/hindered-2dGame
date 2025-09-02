using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class Enemy_AntiVerse_ShaPt : Enemy
{
    SpriteAnimated idleSprite;
    SpriteAnimated aggroSprite;

    GameObject colliderObject;
    float colliderYPos;
    public Enemy_AntiVerse_ShaPt(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = false, bool isMovable = true) : base(mass, velocity, acceleration)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = false;
        this.isMovable = false;
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
            CreateHealthBar(maxHealth: 10, maxShield: 200);
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
        knockBackForce = 1f;

        activeSprite = idleSprite;
        idleSprite.gameObject.SetActive(true);
        aggroSprite.gameObject.SetActive(false);
    }

    protected override void CreateStateController()
    {
        State state0 = new State(
            GameConstantsAndValues.States.IDLE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: true, x: 150, y: 400, Player.Instance.gameObject.transform, gameObject.transform)]);

        State state1 = new State(
            GameConstantsAndValues.States.AGGRESSIVE.ToString(),
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: false, x: 150, y: 400, Player.Instance.gameObject.transform, gameObject.transform)]);
        stateController = new StateController(availableStates: [state0, state1]);

        // add stateController gameobject as a child
        stateAction = IdleAction;
        base.CreateStateController();
    }

    protected override void OnStateChange(object sender, EventArgs e)
    {
        base.OnStateChange(sender, e);

        bool isAggroAction = stateAction == AggroAction;

        aggroSprite?.gameObject.SetActive(isAggroAction);
        idleSprite?.gameObject.SetActive(!isAggroAction);

        activeSprite = isAggroAction ? aggroSprite : idleSprite;
    }

    protected override void CreateVisuals()
    {
        gameObject.transform.localScale = new Vector2(0.5f, 0.5f);

        // idle object
        GameObject idleGameObject = new GameObject();
        idleGameObject.CreateTransform();
        gameObject.AddChild(idleGameObject);

        idleSprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: JSON_Manager.GetEnemiesSourceRectangles("AntiVerse ShaPt_Idle", animatedSpriteCount: 2),
            frameTimers: [0.2f, 0.2f],
            origins: JSON_Manager.GetEnemiesOrigin("AntiVerse ShaPt_Idle", animatedSpriteCount: 2, gameObject.transform.globalScale),
            layerDepths: [0.05f, 0.05f]
        );

        idleGameObject.AddComponent(idleSprite);
        activeSprite = idleSprite;

        // aggro object
        GameObject aggroGameObject = new GameObject();
        aggroGameObject.CreateTransform();
        gameObject.AddChild(aggroGameObject);

        aggroSprite = new SpriteAnimated(
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangles: JSON_Manager.GetEnemiesSourceRectangles("AntiVerse ShaPt", animatedSpriteCount: 4),
            frameTimers: [0.2f, 0.2f, 0.2f, 0.2f],
            origins: JSON_Manager.GetEnemiesOrigin("AntiVerse ShaPt", animatedSpriteCount: 4, gameObject.transform.globalScale),
            layerDepths: [0.05f, 0.05f, 0.05f, 0.05f]
        );

        aggroGameObject.AddComponent(aggroSprite);

        colliderObject = new GameObject();
        colliderYPos = 400 * gameObject.transform.globalScale.Y;
        colliderObject.CreateTransform(localPosition: new Vector2(0, colliderYPos));
        aggroGameObject.AddChild(colliderObject);

        OBBRectangleCollider rectangleCollider = new OBBRectangleCollider(
            width: 200 * gameObject.transform.globalScale.X,
            height: 150 * gameObject.transform.globalScale.Y,
            isAftermath: false
        );

        rectangleCollider.AddTagsToIgnoreList([
            GameConstantsAndValues.Tags.Terrain.ToString(),
            GameConstantsAndValues.Tags.Hidden.ToString(),
            GameConstantsAndValues.Tags.Enemy.ToString(),
            GameConstantsAndValues.Tags.EnemySpawned.ToString(),
        ]);

        colliderObject.tag = GameConstantsAndValues.Tags.Enemy.ToString();
        colliderObject.AddComponent(rectangleCollider);

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 1;

        base.LoadContent();
    }

    //Vector2 previousVelocity = Vector2.Zero;
    public override void Update(GameTime gameTime)
    {
        bool isFlipped = gameObject.transform.globalPosition.Y > Player.Instance.gameObject.transform.globalPosition.Y;
        if (activeSprite is not null) activeSprite.spriteEffects = isFlipped ? SpriteEffects.FlipVertically : SpriteEffects.None;

        if (colliderObject is not null) colliderObject.transform.localPosition = new Vector2(0, colliderYPos * (isFlipped ? -1 : 1));
        stateAction?.Invoke();
        stateController.Update(gameTime);
    }

    protected override void AggroAction()
    {
        Vector2 thisPos = gameObject.transform.globalPosition;
        Vector2 playerPos = Player.Instance.gameObject.transform.globalPosition;
    }

    protected override void IdleAction()
    {
        //Debug.WriteLine("idle");
        Velocity = Vector2.Zero;
    }

    public override void OnCollisionEnter(Collider collider) { /* do nothing*/ }

    public override void OnDetectionRange(Collider collider)
    {
        if (!isAlive) return;
        Projectile projectile = collider.gameObject.GetComponent<Projectile>();
        Weapon_Blade blade = collider.gameObject.GetComponent<Weapon_Blade>();
        Player player = collider.gameObject.GetComponent<Player>();
        if (projectile != null)
        {
            if (projectile.isPlayerSpawned)
            {
                StatChangeFunctions.EnemyDamageCalculation(weapon: WeaponDataBase.WeaponDictionary[projectile.element][WeaponDataBase.BOW_INDEX], enemy: this);
                //StatChangeFunctions.EnemyDamageCalculation(Player.Instance.equipedWeapon, this);
                projectile.gameObject.SetActive(false);
            }
        }
        else if (blade != null)
        {
            StatChangeFunctions.EnemyDamageCalculation(weapon: WeaponDataBase.WeaponDictionary[blade.imbuedElement][WeaponDataBase.BLADE_INDEX], enemy: this);
        }
        else if (player != null)
        {
            if (player.IsKnockedBack) return;
            Vector2 direction = Vector2.Zero;
            direction.X = player.gameObject.transform.globalPosition.X < gameObject.transform.globalPosition.X ? -1 : 1;
            if (player.gameObject.transform.globalPosition.Y > gameObject.transform.globalPosition.Y) direction.Y = -player.gameObject.transform.globalPosition.Y;
            player.BeginKnockBack(pushPower: knockBackForce, direction: Vector2.Normalize(direction));
            StatChangeFunctions.PlayerDamageCalculation(enemy: this);
        }
    }
}

