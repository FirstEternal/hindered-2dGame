using GamePlatformer;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

internal class BossEnemy_DragonicGrasser : BossEnemy
{
    // TODO -> do the actual logic
    private Movement_TeleportBackForth phase1_Movement1;
    private Movement_Spiral phase1_movement2;

    private Movement_Teleport phase2_Movement;
    private Movement_DrillMode phase2_Movement_DrillMode;

    private readonly Attack_TowardsPlayer attack_TowardsPlayer;
    private readonly Attack_InLineTowardsPlayer attack_InLineTowardsPlayer;

    private bool isDrillMode = false;

    public BossEnemy_DragonicGrasser(Vector2 arrivalPosition, float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = false, bool isMovable = true) : base(arrivalPosition, mass, velocity, acceleration, isGravity, isMovable)
    {
        baseId = 5;
        // create pattern methods
        // -> movement pattern
        phase1_Movement1 = new Movement_TeleportBackForth(this);
        phase1_movement2 = new Movement_Spiral(this);

        phase2_Movement = new Movement_Teleport(this);
        phase2_Movement_DrillMode = new Movement_DrillMode(this);

        // -> attack pattern
        attack_TowardsPlayer = new Attack_TowardsPlayer(this);
        attack_InLineTowardsPlayer = new Attack_InLineTowardsPlayer(this);

        phases.Add(new Phase1_BossEnemy_DragonicGrasser());
        phases.Add(new Phase2_BossEnemy_DragonicGrasser());
        phases.Add(new Phase2_DrillMode_BossEnemy_DragonicGrasser());

        damage = 10;
        critRate = 0.3f;
        knockBackForce = 1.5f;

        isDrillMode = true; /// start second phase in drill mode
    }

    // TODO FIX
    public override void ResetEnemy(Vector2? spawnPosition = null)
    {
        ArrivalSpeed = 300;
        base.ResetEnemy(spawnPosition);

        //gameObject.transform.localScale = new Vector2(0.75f, 0.75f);

        // stats
        healthBar.maxHealth = 300;
        healthBar.maxShield = 200;
        healthBar.currHealth = healthBar.maxHealth;
        healthBar.currShield = healthBar.maxShield;
        maxRecoveringTime = 3;

        isGravity = false;
        isDrillMode = true;
    }

    public override bool HasPhaseChanged()
    {
        int currBossPhase = currentPhase;

        if (healthBar.currShield > 0) currentPhase = 0;
        else
        {
            currentPhase = isDrillMode ? 2 : 1;
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
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase1_Movement1, phase1_movement2], attackMethods: [attack_TowardsPlayer, attack_InLineTowardsPlayer]);
                break;
            case 1:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase2_Movement], attackMethods: [attack_TowardsPlayer]);
                ScheduleDrillMode(5); // after 5 seconds -> Drill Mode
                break;
            case 2:
                phases[currentPhase].BeginPhase(bossEnemy: this, movementMethods: [phase2_Movement_DrillMode], attackMethods: null);
                break;
        }

        Debug.WriteLine("phase has changed");
        /*
        Debug.WriteLine("drillMode: " + isDrillMode);
        Debug.WriteLine("Phase logic changed to:" + currentPhase);*/
    }


    private Timer currentDrillModeTimer;

    public override void TriggerSpecialMoveEnd()
    {
        // change phase from Drill Mode -> Normal Mode 
        isDrillMode = !isDrillMode;
    }
    private void ScheduleDrillMode(float delaySeconds)
    {
        // Cancel the existing timer if it's still running
        if (currentDrillModeTimer != null)
        {
            Game2DPlatformer.Instance.Components.Remove(currentDrillModeTimer);
            currentDrillModeTimer.Dispose();
        }

        currentDrillModeTimer = new Timer(Game2DPlatformer.Instance, delaySeconds);

        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            //TriggerSpecialMove();
            isDrillMode = !isDrillMode; // this should cause the phase 2 to change: Normal Mode <-> Drill Mode
            t.OnCountdownEnd -= callback;
            Game2DPlatformer.Instance.Components.Remove(t);
            t.Dispose();
            currentDrillModeTimer = null; // clear reference
        };

        currentDrillModeTimer.OnCountdownEnd += callback;
        currentDrillModeTimer.BeginTimer();
    }
}
