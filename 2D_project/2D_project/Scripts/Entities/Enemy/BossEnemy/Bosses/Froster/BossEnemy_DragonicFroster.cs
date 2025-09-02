using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class BossEnemy_DragonicFroster : BossEnemy
{
    private GameObject phase1Object;
    private GameObject phase2Object;
    private GameObject phase3Object;

    Movement_BackForth phase1_Movement;
    Attack_InLineTowardsPlayer attack_InLineTowardsPlayer;
    Attack_InCircle attack_InCircle;
    Attack_RainFromAbove attack_RainFromAbove;
    public BossEnemy_DragonicFroster(Vector2 arrivalPosition, float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(arrivalPosition, mass, velocity, acceleration, isGravity, isMovable)
    {
        baseId = 4;
        gameObject = new GameObject();
        gameObject.CreateTransform();
        gameObject.AddComponent(this);

        // create pattern methods
        // -> movement pattern
        // -> attack pattern

        phases.Add(new Phase1_BossEnemy_DragonicFroster());
        phases.Add(new Phase2_BossEnemy_DragonicFroster());
        phases.Add(new Phase3_BossEnemy_DragonicFroster());

        attack_InLineTowardsPlayer = new Attack_InLineTowardsPlayer(this);
        attack_InCircle = new Attack_InCircle(this);
        attack_RainFromAbove = new Attack_RainFromAbove(this);

        phase1_Movement = new Movement_BackForth(this);
        //phase2_Movement = new Movement_TeleportBackForth(this);
        //phase3_Movement = new Movement_TeleportBackForth(this);

        ArrivalSpeed = 200;
    }

    // TODO FIX
    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        // TODO
        base.ResetEnemy(spawnPosition);

        //gameObject.transform.localScale = new Vector2(0.75f, 0.75f);

        healthBar.maxHealth = 150;
        healthBar.maxShield = 200;
        healthBar.currHealth = healthBar.maxHealth;
        healthBar.currShield = healthBar.maxShield;
        maxRecoveringTime = 3;

        damage = 10;

        isGravity = false;

        phase1Object = phases[0].GetVisualGameObject();
        /*
        phase2Object = phases[1].GetVisualGameObject();
        phase3Object = phases[2].GetVisualGameObject();

        phase1Object.SetActive(true);
        phase2Object.SetActive(false);
        phase3Object.SetActive(false);*/
        phase1Object.SetActive(true);
    }

    public override bool HasPhaseChanged()
    {
        int currBossPhase = currentPhase;
        if (healthBar.currShield > healthBar.maxShield / 2)
        {
            currentPhase = 0;
        }
        else if (healthBar.currShield > 0)
        {
            currentPhase = 1;
        }
        else
        {
            currentPhase = 2;
        }
        return currBossPhase != currentPhase;
    }

    public override void UpdatePhaseLogic()
    {
        // arrival Phase
        // phase: -1 -> arrival to the scene
        base.UpdatePhaseLogic();

        // boss Phase
        // there are 2 phases for this boss

        switch (currentPhase)
        {
            case 0:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement], attackMethods: [attack_InCircle, attack_InLineTowardsPlayer]);
                break;
            case 1:
                // phase 2 has same phase1 movement just different parameters
                boundsSprite = phases[currentPhase].GetVisualGameObject().GetComponent<Sprite>();
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement], attackMethods: [attack_RainFromAbove]);
                break;
            case 2:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement], attackMethods: [attack_InCircle, attack_InLineTowardsPlayer, attack_RainFromAbove]);
                break;
        }
    }
    /*
    public BossEnemy_DragonicFroster(ICondition idleToAggroCondition, ICondition AggroToIdleCondition, float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(idleToAggroCondition, AggroToIdleCondition, mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = isGravity;
        this.isMovable = isMovable;
    }
    private Vector2 phaseDestination; // needed to for less calculation logic

    // phase 3 variable
    private int projectileCycle = 0;
    public override void Initialize()
    {
        base.Initialize();

        // TODO mess with numbers a bit

        //entitySpriteWidth = 1100;
        gameObject.transform.globalScale = new Vector2(0.5f, 0.5f);
        maxRecoveringTime = 3;
    }

    public override void LoadContent()
    {
        // TODO
        AddSpriteRelatedComponents(texture: Game2DPlatformer.Instance.Content.Load<Texture2D>("sprites/Dragonic Froster"));

        knockBackImunity = false;
        damage = 5;
        critRate = 0.3f;
        critMultiplier = 0.25f;
        knockBackForce = 0;


        bossPhase = 0;
        base.LoadContent();
        /*
        gameObject.GetComponent<Sprite>().frameWidth = 1100; // Width of each frame
        gameObject.GetComponent<Sprite>().frameHeight = gameObject.GetComponent<Sprite>().texture2D.Height; // Height of each frame

        // Calculate the total number of frames in the sprite sheet
        gameObject.GetComponent<Sprite>().totalFrames = gameObject.GetComponent<Sprite>().texture2D.Width / gameObject.GetComponent<Sprite>().frameWidth;

        // Initialize animation timing
        gameObject.GetComponent<Sprite>().frameTime = 0.35f;
    }

    public override bool HasPhaseChanged()
    {
        return false;
        int currBossPhase = bossPhase;
        if(gameObject.GetComponent<HealthBar>().currShield > gameObject.GetComponent<HealthBar>().currShield * 3/4)
        {
            bossPhase = 0;
        }
        else if (gameObject.GetComponent<HealthBar>().currShield > gameObject.GetComponent<HealthBar>().currShield / 2)
        {
            bossPhase = 1;
        }
        else if (gameObject.GetComponent<HealthBar>().currShield > gameObject.GetComponent<HealthBar>().currShield / 4)
        {
            bossPhase = 2;
        }
        else if (gameObject.GetComponent<HealthBar>().currShield == 0)
        {
            bossPhase = 3;
        }
        return currBossPhase != bossPhase;
    }

    public override void UpdatePhaseLogic()
    {
        // there are 4 phases for this boss
        // based on bossPhase assign subStep 0 characteristics
        phaseSubstep = 0;

        // IN THIS CASE 1-3 do the same thing for now -> might need adjustments later
        switch (bossPhase)
        {
            case 0:
                //dmgReduction = 1; // is damage immune
                dmgReduction = 0; // is damage immune
                isAttacking = false;
                break;
            case 1:
                currReloadTime = 1f; // ensure to instantly start attacking
                maxReloadTime = 1f;

                movementType = MovementType.Back_Forth;
                translationVector = TranslationDownNormalized; // start moving down
                moveSpeed = 400;
                maxTravelDistance = 400;
                currStaminaTime = 0;
                maxStaminaTime = int.MaxValue; // no stamina limit

                isAttacking = true;
                break;
            case 2:
                currReloadTime = 2f; // ensure to instantly start attacking
                maxReloadTime = 2f;

                movementType = MovementType.Back_Forth;
                translationVector = TranslationDownNormalized; // start moving down
                moveSpeed = 400;
                maxTravelDistance = 400;
                currStaminaTime = 0;
                maxStaminaTime = int.MaxValue; // no stamina limit

                isAttacking = true;
                break;
            case 3:
                currReloadTime = 2f; // ensure to instantly start attacking
                maxReloadTime = 2f;

                movementType = MovementType.Back_Forth;
                translationVector = TranslationDownNormalized; // start moving down
                moveSpeed = 400;
                maxTravelDistance = 400;
                currStaminaTime = 0;
                maxStaminaTime = int.MaxValue; // no stamina limit

                isAttacking = true;
                break;
        }
    }

    public override void Movement(GameTime gameTime)
    {
        // IN THIS CASE 1-3 do the same thing for now -> might need adjustments later
        switch (bossPhase)
        {
            case 0:
                // sleeping -> do nothing
                return;
            case 1:
                BackAndForthRecoveryMovementAttack(gameTime);
                break;
            case 2:
                BackAndForthRecoveryMovementAttack(gameTime);
                break;
            case 3:
                BackAndForthRecoveryMovementAttack(gameTime);
                break;
        }
    }
    public override void ProjectileAttackLogic()
    {
        switch (bossPhase)
        {
            case 0:
                // sleeping -> do nothing
                return;
            case 1:
                float distanceX = Math.Abs(gameObject.transform.globalPosition.X - Player.Instance.gameObject.transform.globalPosition.X);
                bool isLeftToRight = distanceX > (gameObject.GetComponent<Sprite>().texture2D.Width * gameObject.transform.globalScale.X) / 2;
                int projectileCount = isLeftToRight ? 3 : 5;

                // if players is underneath spawn 5: up -> down, otherwise spawn 3: left -> right
                SpawnProjectilesInLine(
                    projectileType: GameConstantsAndValues.FactionType.Froster,
                    projectileCount: projectileCount,
                    projectileScale: Vector2.One / 2,
                    projectileSeparatorDistance: 100,
                    isLeftToRight: isLeftToRight
                );
                break;
            case 2:

                SpawnProjectilesTowardPlayer(
                    projectileType: GameConstantsAndValues.FactionType.Froster,
                    projectileCount: 4,
                    projectileScale: Vector2.One / 2,
                    projectileSeparatorDistance: 100
                );
                break;
            case 3:
                int degreeStart;
                int degreeEnd;
                switch (projectileCycle)
                {
                    case 0:
                        // fire left
                        degreeStart = 300;
                        degreeEnd = 90;
                        projectileCount = 6;
                        projectileCycle++;
                        break;
                    case 1:
                        // fire right
                        degreeStart = 90;
                        degreeEnd = 240;
                        projectileCount = 6;
                        projectileCycle++;
                        break;
                    case 2:
                        // fire down
                        degreeStart = 0;
                        degreeEnd = 180;
                        projectileCount = 10;
                        projectileCycle = 0;
                        break;
                    default:
                        degreeStart = 0;
                        degreeEnd = 0;
                        projectileCount = 0;
                        break;
                }

                SpawnProjectilesInCircle(
                    projectileType: GameConstantsAndValues.FactionType.Froster,
                    projectileCount: projectileCount,
                    projectileScale: Vector2.One / 2,
                    degreeStart: degreeStart,
                    degreeEnd: degreeEnd
                );
                break;
        }

    }*/

}
