using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

internal class Projectile(float mass = 0, bool isGravity = false) : PhysicsComponent(mass, isGravity: isGravity)
{
    public void UpdateSpeed(float speed, float angularSpeed = 0)
    {
        Velocity = Velocity * speed;
        AngularVelocity = AngularVelocity * angularSpeed;
    }

    protected void AdjustAngularRotation()
    {
        AngularVelocity = baseAngularSpeed * (1 + chargedValue / 4);
    }

    public bool exitedTeleport;
    public bool isPlayerSpawned { get; set; }
    //protected float moveSpeed;
    //protected float currSpeed;

    public float knockBackForce;
    public float currKnockBackForce;

    public float currSpeed { get; private set; }
    protected float baseLinearSpeed;
    protected float baseAngularSpeed;
    protected Vector2 directionVector;


    protected float chargedValue;
    //public bool isPlayerSpawned { get; private set; }

    public Entity projectileOwner { get; private set; }

    protected float deathTimer = 10;

    protected bool isAlive = true;
    protected bool isCharging;
    protected bool isFired;

    private Vector2 baseLocalScale;
    private bool terrainImunity;

    public Weapon.ImbuedElement element { get; protected set; }

    public override void Update(GameTime gameTime)
    {
        // CHECK FOR COLLISION

        if (isAlive)
        {
            deathTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (deathTimer <= 0)
            {
                isAlive = false;
                gameObject.SetActive(false);
            }
        }

        // projectile owner no longer exists therefore destroy this gameObject
        if (projectileOwner == null) isAlive = false;
    }

    public void AssignDirection(Vector2 spawnPosition, Vector2 destination)
    {
        // Move to spawn location 
        gameObject.transform.spawnPosition = spawnPosition;
        gameObject.transform.globalPosition = spawnPosition;

        // Calculate direction
        Vector2 direction = destination - gameObject.transform.globalPosition;
        if (direction != Vector2.Zero) direction.Normalize();

        // If the direction is suspiciously (-1, -1), print a warning
        if (Math.Abs(direction.X + 1) < 0.1f && Math.Abs(direction.Y + 1) < 0.1f)
        {
            Debug.WriteLine("⚠ Warning: Unexpected diagonal (-1, -1) direction detected!");
        }

        // Apply rotation
        float angle = (float)(Math.Atan2(direction.Y, direction.X));
        gameObject.transform.localRotationAngle = angle;

        currSpeed = baseLinearSpeed;
        directionVector = direction;
    }

    public void SpawnToDie(Vector2 deathSpawnPosition)
    {
        gameObject.transform.globalPosition = deathSpawnPosition;
        deathTimer = 0;
    }

    public virtual void Spawn(float deathTimer, Vector2 spawnPosition, float spawnRotation, Vector2 destination, Entity projectileOwner, Vector2? spawnScale = null, bool terrainImunity = false, bool hasProjectileImmunity = false)
    {
        SceneManager.Instance.RemoveGameObjectFromScene(gameObject); // remove from current scene
        SceneManager.Instance.activeScene.AddGameObjectToScene(gameObject, isOverlay: false); // add object to active scene
        // reset values
        gameObject.SetActive(true);
        isAlive = false;
        isCharging = true;
        currSpeed = 0;
        Velocity = Vector2.Zero;
        currKnockBackForce = 0;
        chargedValue = 0;
        AngularVelocity = baseAngularSpeed;

        this.deathTimer = deathTimer;

        gameObject.transform.localScale = spawnScale ?? Vector2.One;

        gameObject.transform.globalPosition = spawnPosition;
        UpdateCollider();
        //AssignDirection(spawnPosition, destination, projectileOwner);

        this.projectileOwner = projectileOwner;
        isPlayerSpawned = projectileOwner != null && projectileOwner.GetType() == typeof(Player);

        exitedTeleport = false;

        directionVector = Vector2.Zero;

        baseLocalScale = gameObject.transform.localScale;

        this.terrainImunity = terrainImunity;
        if (hasProjectileImmunity)
        {
            gameObject.tag = isPlayerSpawned ? GameConstantsAndValues.Tags.PlayerMeleeSpawned.ToString() : GameConstantsAndValues.Tags.EnemyMeleeSpawned.ToString();
        }
        else
        {
            gameObject.tag = isPlayerSpawned ? GameConstantsAndValues.Tags.PlayerSpawned.ToString() : GameConstantsAndValues.Tags.EnemySpawned.ToString();

            if (terrainImunity)
            {
                gameObject.tag = $"AntiTerrain{gameObject.tag}";
            }
        }

        Task.Delay(1).ContinueWith(_ =>
        {
            AssignDirection(spawnPosition, destination);
        });

        isFired = false;
    }

    public virtual void UpdateValues(float chargeValue, Vector2 spawnPosition, Vector2 destination)
    {
        chargedValue = chargeValue;
        gameObject.transform.localScale = baseLocalScale * (1 + chargedValue / 4); // up to 100% increase in size

        AdjustAngularRotation();
        UpdateCollider();
        AssignDirection(spawnPosition, destination);
    }

    protected virtual void UpdateCollider() { }

    public virtual void BeginMovement(Vector2 spawnPosition, Vector2 destination)
    {
        AdjustAngularRotation();
        UpdateCollider();
        AssignDirection(spawnPosition, destination);

        // begin normal movement
        BeginMovement();
    }

    public virtual void BeginMovement()
    {
        isAlive = true;
        //deathTimer = 10;
        currSpeed = baseLinearSpeed * (1 + chargedValue / 4);// up to 100% increase in speed
        Velocity = directionVector * currSpeed;

        currKnockBackForce = knockBackForce * (1 + chargedValue / 4); // up to 100% increase in knockback

        isFired = true;
    }

    public override void LoadContent()
    {
        //gameObject.GetComponent<Collider>().AddTagsToIgnoreList([GameConstantsAndValues.Tags.Hidden.ToString()]);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        if (!isFired) return;

        if (!collider.isAftermath) return; // just collision detection do nothing

        Projectile projectile = collider.gameObject.GetComponent<Projectile>();

        if (projectile != null && isPlayerSpawned == projectile.isPlayerSpawned) return;
        if (terrainImunity && collider.gameObject.tag == GameConstantsAndValues.Tags.Terrain.ToString()) return;

        gameObject.SetActive(false);
    }

    public override void OnDetectionRange(Collider collider)
    {
        if (!isFired) return;

        if (collider.gameObject.tag == GameConstantsAndValues.Tags.PlayerSpawned.ToString())
        {
            gameObject.SetActive(false);
        }
    }

}
