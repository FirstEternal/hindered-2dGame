using Microsoft.Xna.Framework;

internal class BossEnemy_DragonicBurner : BossEnemy
{
    private Movement_DownBounceLeftRightReturnUp phase1_Movement;
    private Movement_BackForth phase2_Movement;
    private Attack_TowardsPlayer phase2_Attack;

    public BossEnemy_DragonicBurner(Vector2 arrivalPosition, float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = false, bool isMovable = true) : base(arrivalPosition, mass, velocity, acceleration, isGravity, isMovable)
    {
        baseId = 1;
        // create pattern methods
        // -> movement pattern
        phase1_Movement = new Movement_DownBounceLeftRightReturnUp(this);
        phase2_Movement = new Movement_BackForth(this);

        // -> attack pattern
        phase2_Attack = new Attack_TowardsPlayer(this);

        phases.Add(new Phase1_BossEnemy_DragonicBurner());
        phases.Add(new Phase2_BossEnemy_DragonicBurner());

        damage = 10;
        critRate = 0.3f;
        knockBackForce = 1.5f;
    }

    // TODO FIX
    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        ArrivalSpeed = 300;
        base.ResetEnemy(spawnPosition);

        //gameObject.transform.localScale = new Vector2(0.75f, 0.75f);

        // stats
        healthBar.maxHealth = 100;
        healthBar.maxShield = 50;
        healthBar.currHealth = healthBar.maxHealth;
        healthBar.currShield = healthBar.maxShield;
        maxRecoveringTime = 3;

        isGravity = false;
    }

    public override bool HasPhaseChanged()
    {
        int currBossPhase = currentPhase;
        currentPhase = (healthBar.currShield > 0) ? 0 : 1;
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
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement], attackMethods: null);
                break;
            case 1:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase2_Movement], attackMethods: [phase2_Attack]);
                break;
        }
    }
}
