using Microsoft.Xna.Framework;

internal class BossEnemy_DragonicDrowner : BossEnemy
{
    private Movement_DownBounceLeftRightReturnUp phase1_Movement_TopLane;
    private Movement_DownBounceLeftRightReturnUp phase1_Movement_BottomLane;
    private Movement_BackForth phase2_Movement;

    private Attack_InLineTowardsPlayer phase2_Attack;
    private Attack_InCircle phase3_Attack;
    public bool isMovementDecided = false;

    public BossEnemy_DragonicDrowner(Vector2 arrivalPosition, float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(arrivalPosition, mass, velocity, acceleration, isGravity, isMovable)
    {
        baseId = 2;
        /*
        gameObject = new GameObject();
        gameObject.CreateTransform();
        gameObject.AddComponent(this);
        */
        // create pattern methods
        // -> movement pattern
        phase1_Movement_TopLane = new Movement_DownBounceLeftRightReturnUp(this);
        phase1_Movement_BottomLane = new Movement_DownBounceLeftRightReturnUp(this);
        phase2_Movement = new Movement_BackForth(this);

        // -> attack pattern
        phase2_Attack = new Attack_InLineTowardsPlayer(this);
        phase3_Attack = new Attack_InCircle(this);

        // create Phases
        phases.Add(new Phase1_BossEnemy_DragonicDrowner());
        phases.Add(new Phase2_BossEnemy_DragonicDrowner());
        phases.Add(new Phase3_BossEnemy_DragonicDrowner());

        damage = 15;
        critRate = 0.6f;
        knockBackForce = 1.5f;
    }

    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        ArrivalSpeed = 300;
        base.ResetEnemy(spawnPosition);

        gameObject.transform.localScale = new Vector2(0.75f, 0.75f);

        healthBar.maxHealth = 30;
        healthBar.maxShield = 20;
        healthBar.currHealth = healthBar.maxHealth;
        healthBar.currShield = healthBar.maxShield;
        maxRecoveringTime = 3;

        isGravity = false;
    }

    public override bool HasPhaseChanged()
    {
        int currBossPhase = currentPhase;
        if (healthBar.currShield > 0)
        {
            currentPhase = 0;
        }
        else if (healthBar.currHealth > healthBar.maxHealth / 2)
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
        // there are 3 phases for this boss
        isMovementDecided = false;
        switch (currentPhase)
        {
            case 0:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement_BottomLane, phase1_Movement_TopLane], attackMethods: null);
                break;
            case 1:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase2_Movement], attackMethods: [phase2_Attack]);
                isMovementDecided = true;
                break;
            case 2:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement_BottomLane, phase1_Movement_TopLane], attackMethods: [phase3_Attack]);
                break;
        }
    }
    public override void BeginRecovering()
    {
        isMovementDecided = false;
        base.BeginRecovering();
    }
}