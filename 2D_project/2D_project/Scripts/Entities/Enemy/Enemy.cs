using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;

internal abstract class Enemy : Entity, IResettable
{
    protected delegate void StateAction();
    protected StateAction stateAction;

    protected StateController stateController;

    protected Sprite activeSprite;

    protected bool isAlive;

    public Enemy(float mass = 1000, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = isGravity;
        this.isMovable = isMovable;
    }

    //protected int currentPhase = 0;
    public int currentPhase { get; protected set; } = 0;
    protected int totalPhases;

    public virtual void Reset() { ResetEnemy(); }
    public virtual void ResetEnemy(Vector2? spawnPosition = null) { isAlive = true; gameObject.SetActive(true); }
    protected virtual void CreateVisuals() { }

    protected enum StateAggression
    {
        idle,
        aggro,
    }
    protected StateAggression stateAggression = StateAggression.idle;
    protected virtual void AggroAction() { }
    protected virtual void IdleAction() { }


    public override void Initialize()
    {
        base.Initialize();
    }

    protected virtual void CreateStateController()
    {
        stateController.OnStateChange += OnStateChange;
    }

    protected virtual void OnStateChange(object sender, EventArgs e)
    {
        if (stateController.GetCurrentStateName() == GameConstantsAndValues.States.AGGRESSIVE.ToString())
        {
            stateAction = AggroAction;
        }
        else
        {
            stateAction = IdleAction;
        }
    }
    protected bool isInTransition;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        stateAction?.Invoke();
    }

    public override void ApplyDeath()
    {
        // TODO drop loot
        gameObject.SetActive(false);
        isAlive = false;
        //isMovable = false;
    }

    // Enemy damage dealt
    protected bool knockBackImunity;
    public int damage;
    public float critRate;
    public float critMultiplier;
    public float knockBackForce;

    // Movement related
    protected float currTraveledDistance;
    protected float maxTravelDistance = int.MaxValue;
    public Vector2 translationVector;

    protected bool isMoving;

    public enum MovementType
    {
        One_Direction,
        Back_Forth,
    }

    protected MovementType movementType;

    protected void AssignTranslationTowardsPlayer(bool isMovementX, bool isMovementY)
    {
        Vector2 direction = Player.Instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
        if (!isMovementX) direction.X = 0;
        if (!isMovementY) direction.Y = 0;
        direction.Normalize();

        float angle = (float)(Math.Atan2(direction.Y, direction.X) * 180.0 / Math.PI);
        translationVector = direction;
        isMoving = true;
    }

    public override void OnDetectionRange(Collider collider)
    {
        if (!isAlive) return;

        base.OnCollisionEnter(collider); // terrain collision
        Projectile projectile = collider.gameObject.GetComponent<Projectile>();
        Weapon_Blade blade = collider.gameObject.GetComponent<Weapon_Blade>();
        Player player = collider.gameObject.GetComponent<Player>();
        if (projectile != null)
        {
            if (projectile.isPlayerSpawned)
            {
                StatChangeFunctions.EnemyDamageCalculation(weapon: WeaponDataBase.WeaponDictionary[projectile.element][WeaponDataBase.BOW_INDEX], enemy: this);
                StatChangeFunctions.ApplyElementalEffect(
                    attacker: Player.Instance,
                    target: this,
                    effect: projectile.element);
                //StatChangeFunctions.EnemyDamageCalculation(Player.Instance.equipedWeapon, this);
                projectile.gameObject.SetActive(false);
            }
        }
        else if (blade != null)
        {
            StatChangeFunctions.EnemyDamageCalculation(weapon: WeaponDataBase.WeaponDictionary[blade.imbuedElement][WeaponDataBase.BLADE_INDEX], enemy: this);
            StatChangeFunctions.ApplyElementalEffect(
                attacker: Player.Instance,
                target: this,
                effect: blade.imbuedElement
            );

        }
        else if (player != null)
        {
            if (player.IsKnockedBack) return;
            player.BeginKnockBack(pushPower: knockBackForce, direction: Vector2.Normalize(Velocity));
            StatChangeFunctions.PlayerDamageCalculation(enemy: this);
        }
        /*
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
        else if (collider.gameObject.tag == GameConstantsAndValues.Tags.Terrain.ToString())
        {
            Velocity.Y = 0;
            cumulatedVelocity.Y = 0;
        }
        else if (collider.gameObject.tag == GameConstantsAndValues.Tags.Player.ToString())
        {
            Player.Instance.BeginKnockBack(knockBackForce, direction:Velocity.Normalize, knockDuration: 0.3f);
        }*/
    }
}
