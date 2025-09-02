using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

internal class BossEnemy : Enemy
{
    public int baseId;
    protected List<IPhase> phases = new List<IPhase>();

    protected bool visualsNotSpawned;
    public event EventHandler OnPhaseChange;
    public event EventHandler OnSpecialMove;
    public event EventHandler OnArenaChange;

    public event EventHandler OnFreezeChange;

    public bool isBossFrozen { get; private set; }

    private int freezeCount = 0;
    private bool hasBossPhaseStarted;
    public void TriggerSpecialMove()
    {
        OnSpecialMove?.Invoke(this, EventArgs.Empty);
    }

    public virtual void TriggerSpecialMoveEnd() { }

    public virtual void StopBossMovement()
    {
        isMovable = false;
        isBossFrozen = true;
        freezeCount++;  // prolong freeze if already frozen
        OnFreezeChange?.Invoke(this, EventArgs.Empty);
    }

    public virtual void ResumeBossMovement()
    {

        freezeCount--;
        if (freezeCount == 0)
        {
            // unfreeze after all instances have unfrozen it
            isMovable = true;
            isBossFrozen = false;
        }
        OnFreezeChange?.Invoke(this, EventArgs.Empty);
    }

    protected void TriggerArenaChange()
    {
        OnArenaChange?.Invoke(this, EventArgs.Empty);
    }
    public BossEnemy(Vector2 ArrivalPosition, float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = false) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = isGravity;
        this.isMovable = isMovable;
        this.ArrivalPosition = ArrivalPosition;

        gameObject = new GameObject();
        gameObject.CreateTransform();
        gameObject.AddComponent(this);

        //CreateHealthBar(maxHealth: 100, maxShield: 100);
        CreateStateController();

        visualsNotSpawned = true;

        currentPhase = -1;
    }

    public Vector2 ArrivalPosition { get; private set; }
    public int ArrivalSpeed { get; protected set; }

    bool isPlayingScript;
    // Phase setup
    protected int phaseSubstep = 0;

    // recovering time -> this Boss is standing still
    protected float currRecoveringTime = 0;
    protected float maxRecoveringTime;
    public bool isRecovering;

    protected int bounceCount;
    public float currStaminaTime = 0;
    public float maxStaminaTime;

    // projectile variables
    public float currReloadTime = 0;
    public float maxReloadTime;
    protected bool isAttacking;
    protected bool isReloading;

    protected bool hasBossStageBegun = false;

    public void ResetEnemyViaEvent(object sender, EventArgs e)
    {
        if (gameObject is not null)
        {
            hasBossStageBegun = true;
            ResetEnemy();
            UpdatePhaseLogic();
        }
    }
    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        base.ResetEnemy(spawnPosition);
        isBossFrozen = false;
        freezeCount = 0;
        if (spawnPosition != null)
        {
            gameObject.transform.spawnPosition = (Vector2)spawnPosition;
        }

        if (visualsNotSpawned)
        {
            // create Phase Visuals(sprite + collider)
            foreach (IPhase phase in phases)
            {
                phase.CreateVisuals(parent: gameObject);
            }

            visualsNotSpawned = false;
        }

        if (healthBar is null)
        {
            CreateHealthBar(maxHealth: 100, maxShield: 100);
            //healthBar.LoadContent();
            CreateStateController();
            CreateVisuals();

            LoadContent();
        }

        healthBar.currHealth = healthBar.maxHealth;
        healthBar.currShield = healthBar.maxShield;

        totalPhases = 2;
        currentPhase = -1; // state before arrival

        // move to spawn location
        gameObject.transform.globalPosition = gameObject.transform.spawnPosition;
        isPlayingScript = true;

        Velocity = Vector2.Zero;
        cumulatedVelocity = Vector2.Zero;
    }

    public void Reload(GameTime gameTime)
    {
        currReloadTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currReloadTime >= maxReloadTime)
        {
            currReloadTime = 0;
            isReloading = false;
            ProjectileAttackLogic();
        }
    }

    protected override void CreateStateController()
    {

        //GameObject stateControllerObject = new GameObject();
        State state0 = new State(
            GameConstantsAndValues.States.IDLE.ToString(),
            //exitConditions: [new Condition_TransformProximityRadius(isWithinRange: true, radius: 300, Player.Instance.gameObject.transform, gameObject.transform)]);
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: true, x: 300, y: 100, Player.Instance.gameObject.transform, gameObject.transform)]);

        State state1 = new State(
            GameConstantsAndValues.States.AGGRESSIVE.ToString(),
            //exitConditions: [new Condition_TransformProximityRadius(isWithinRange: false, radius: 300, Player.Instance.gameObject.transform, gameObject.transform)]);
            exitConditions: [new Condition_TransformProximityElipse(isWithinRange: false, x: 300, y: 100, Player.Instance.gameObject.transform, gameObject.transform)]);
        stateController = new StateController(availableStates: [state0, state1]);
        //stateControllerObject.AddComponent(stateController);

        // add stateController gameobject as a child
        //gameObject.AddChild(stateControllerObject);
        stateAction = IdleAction;
        base.CreateStateController();
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (visualsNotSpawned || isBossFrozen) return;

        if (isPlayingScript)
        {
            // play arrival function -> intro script
            //isPlayingScript = MGEngine.Physics.Movement.IsInPosition(ArrivalPosition, gameObject.transform, tolerationDistance: 100);
            PhaseTransitionLogic();
            return;
        }

        //base.Update(gameTime);
        phases[currentPhase].GetVisualGameObject().GetComponent<SpriteAnimated>().spriteEffects = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        stateAction?.Invoke();
        PhaseLogic(gameTime);
    }

    protected virtual void PhaseLogic(GameTime gameTime)
    {
        // check for phase change
        if (HasPhaseChanged())
        {
            isBossFrozen = false; // unfreeze if it was frozen
            UpdatePhaseLogic();
        }

        if (isRecovering)
        {
            dmgReduction = 0; // when recovering it is not immune to damage
            Recover(gameTime);
        }
        phases[currentPhase].UpdatePhase(gameTime);

        /*
        if (isAttacking)
        {
            // apply reload logic
            Reload(gameTime);
        }

        // apply recover or phase logic
        if (isRecovering)
        {
            dmgReduction = 0; // when recovering it is not immune to damage
            Recover(gameTime);
        }
        else
        {
            // apply current boss phase logic
            Movement(gameTime);
        }


        UpdateCollider();*/
    }

    public virtual void BeginRecovering()
    {
        currRecoveringTime = 0;
        isRecovering = true;
    }

    protected virtual void PhaseTransitionLogic()
    {
        if (currentPhase == -1)
        {
            if (!isMovable) return;
            // arrival to the scene
            isPlayingScript = !MGEngine.Physics.Movement.IsInPosition(ArrivalPosition, gameObject.transform, tolerationDistance: 100);
            if (!isPlayingScript)
            {
                // is within toleration distance -> place to arrival position
                gameObject.transform.globalPosition = ArrivalPosition;
                Velocity = Vector2.Zero;

                // change camera's zoom 
                SceneManager.Instance.activeScene.mainCamera.ChangeZoomSmoothAnimation(finalZoom: 0.5f, durationInSeconds: 1);

                // begin first phase
                currentPhase = 0;
                UpdatePhaseLogic();
            }
        }
        else
        {
            isPlayingScript = false;
        }
    }

    public virtual bool HasPhaseChanged()
    {
        return false;
    }

    public virtual void Movement(GameTime gameTime)
    {
        // do boss phases
    }

    protected void Recover(GameTime gameTime)
    {
        currRecoveringTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currRecoveringTime >= maxRecoveringTime)
        {
            currRecoveringTime = 0;
            isRecovering = false;

            bounceCount = 0;
        }
    }

    public virtual void ProjectileAttackLogic() { }

    public virtual void UpdatePhaseLogic()
    {
        // reset wallCount
        bounceCount = 0;

        if (currentPhase == -1)
        {
            isPlayingScript = true;
            MGEngine.Physics.Movement.AssignVelocity(ArrivalPosition, this, ArrivalSpeed);
            for (int i = 0; i < phases.Count; i++)
            {
                phases[i].GetVisualGameObject().SetActive(i == 0);
            }

            isGravity = false;

            return;
        }
        else
        {
            Velocity = Vector2.Zero;
        }

        currRecoveringTime = 0;
        isRecovering = false;

        phaseSubstep = 0;

        // hide all non active phase visuals
        foreach (IPhase phase in phases)
        {
            phase?.GetVisualGameObject()?.SetActive(false);
        }

        if (currentPhase > 0) OnPhaseChange?.Invoke(this, EventArgs.Empty);
    }
    public override void OnCollisionEnter(Collider collider)
    {
        if (!isAlive) return;
        base.OnCollisionEnter(collider);
        CollisionLogic(collider);
    }

    public override void OnDetectionRange(Collider collider)
    {
        if (!isAlive) return;
        base.OnDetectionRange(collider);
        CollisionLogic(collider);
    }

    protected virtual void CollisionLogic(Collider collider)
    {
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
            player.BeginKnockBack(pushPower: knockBackForce, direction: Vector2.Normalize(Velocity));
            StatChangeFunctions.PlayerDamageCalculation(enemy: this);
        }
    }

    public override void ApplyDeath()
    {
        base.ApplyDeath();

        // change camera's zoom 
        SceneManager.Instance.activeScene.mainCamera.ChangeZoomSmoothAnimation(finalZoom: Camera.BASE_ZOOM, durationInSeconds: 1);
    }
}
