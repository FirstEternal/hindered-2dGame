using GamePlatformer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class Movement_TeleportBackForth : IBossMethod
{
    Timer teleportCooldownTimer;
    private List<Vector2> teleportPositions;

    private int teleportCountBeforeNewSequence;
    private float speed;
    private float maxSpeed;
    private bool isSequenceRandom;
    private int[] teleportPositionIndices_Sequence;
    private int maxDeccelerateIndex;

    private int currTeleportPositionIndex;
    private int currSequenceIndex;

    // Internal state variables
    private int phaseSubstep = 0;
    private int currentTeleportCount = 0;
    private float movementSpeedIncreasePerTeleport;

    private readonly BossEnemy bossEnemy; // Reference to the game object

    public EventHandler OnTeleport;

    private int bossEnemyPhase;

    public Movement_TeleportBackForth(BossEnemy bossEnemy)
    {
        this.bossEnemy = bossEnemy;
    }
    private void RandomizeSequence()
    {
        if (isSequenceRandom)
        {
            teleportPositionIndices_Sequence = new int[teleportPositions.Count];
            for (int i = 0; i < teleportPositions.Count; i++)
            {
                teleportPositionIndices_Sequence[i] = Game2DPlatformer.Instance.random.Next(0, teleportPositions.Count);
            }
        }
    }

    public void ResetSubsteps()
    {
        phaseSubstep = 0;
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length < 8 || parameters.Length > 9) throw new ArgumentException("Expected 8 or 9 parameters.");
        teleportPositions = (List<Vector2>)parameters[0];

        float teleportCooldownTime = (float)parameters[1];
        teleportCountBeforeNewSequence = (int)parameters[2];
        speed = (int)parameters[3];
        movementSpeedIncreasePerTeleport = (int)parameters[4];
        maxSpeed = (int)parameters[5];
        isSequenceRandom = (bool)parameters[6];

        if (!isSequenceRandom)
        {
            teleportPositionIndices_Sequence = (int[])parameters[7];
        }
        else
        {
            RandomizeSequence();
        }


        phaseSubstep = 0;
        currentTeleportCount = 0;
        currSequenceIndex = 0;
        currTeleportPositionIndex = teleportPositionIndices_Sequence[currSequenceIndex];

        if (teleportCooldownTimer is null)
        {
            teleportCooldownTimer = new Timer(game: Game2DPlatformer.Instance, teleportCooldownTime);
        }
        else
        {
            teleportCooldownTimer.OverrideTimer(teleportCooldownTime);
        }

        teleportCooldownTimer.OnCountdownEnd -= OnCountdownEnd_Teleport;
        teleportCooldownTimer.OnCountdownEnd += OnCountdownEnd_Teleport;

        if (parameters.Length == 9) maxDeccelerateIndex = parameters[8] is int value ? value : int.MaxValue;
        else maxDeccelerateIndex = int.MaxValue;

        bossEnemyPhase = bossEnemy.currentPhase;
    }

    public void Execute(float? deltaTime = null)
    {
        switch (phaseSubstep)
        {
            case 0:
                teleportCooldownTimer.BeginTimer(); // teleportCooldownTimer.OnCountdownEnd event -> teleport and advance substep by 1
                phaseSubstep++;
                break;
            case 2:
                if (MGEngine.Physics.Movement.IsInPosition(
                    finalPosition: teleportPositions[currTeleportPositionIndex],
                    bossEnemy.gameObject.transform,
                    tolerationDistance: 50))
                {
                    phaseSubstep = 0;
                }
                break;
        }
    }

    private void OnCountdownEnd_Teleport(Timer timer)
    {
        currentTeleportCount++;
        if (currentTeleportCount % teleportCountBeforeNewSequence == 0)
        {
            RandomizeSequence();
        }
        TeleportBossEnemy();
        phaseSubstep++;
    }

    private void TeleportBossEnemy()
    {
        // no longer the same phase - needed because timer works separately from phase logic
        if (bossEnemy.currentPhase != bossEnemyPhase) return;

        bossEnemy.gameObject.transform.globalPosition = teleportPositions[currTeleportPositionIndex];
        currSequenceIndex++;
        if (currSequenceIndex == teleportPositionIndices_Sequence.Length) currSequenceIndex = 0;
        currTeleportPositionIndex = teleportPositionIndices_Sequence[currSequenceIndex];

        // calculate new speed;
        int cycleStep = currentTeleportCount % maxDeccelerateIndex;
        float newSpeed = speed + cycleStep * movementSpeedIncreasePerTeleport;

        MGEngine.Physics.Movement.AssignVelocity(finalPosition: teleportPositions[currTeleportPositionIndex], bossEnemy, newSpeed);
        OnTeleport?.Invoke(this, EventArgs.Empty);
    }
}