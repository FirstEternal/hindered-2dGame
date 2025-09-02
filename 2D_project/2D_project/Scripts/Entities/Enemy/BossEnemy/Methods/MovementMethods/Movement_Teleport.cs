using GamePlatformer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

internal class Movement_Teleport : IBossMethod
{
    private Timer teleportCooldownTimer;
    private List<Vector2> teleportPositions;
    private bool isSequenceRandom;
    private int[] teleportPositionIndices_Sequence;

    private int currSequenceIndex;
    private int currentTeleportCount = 0;
    private int teleportCountBeforeNewSequence;

    private readonly BossEnemy bossEnemy;

    public EventHandler OnTeleport;

    private int bossEnemyPhase;

    public Movement_Teleport(BossEnemy bossEnemy)
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
        // Not needed for teleport-only logic but implemented for interface compliance
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 4 && parameters.Length != 5)
            throw new ArgumentException("Expected 4 or 5 parameters.");

        teleportPositions = (List<Vector2>)parameters[0];
        float teleportCooldownTime = (float)parameters[1];
        teleportCountBeforeNewSequence = (int)parameters[2];
        isSequenceRandom = (bool)parameters[3];

        if (!isSequenceRandom)
        {
            teleportPositionIndices_Sequence = (int[])parameters[4];
        }
        else
        {
            RandomizeSequence();
        }

        currSequenceIndex = 0;
        currentTeleportCount = 0;

        if (teleportCooldownTimer is null)
        {
            teleportCooldownTimer = new Timer(Game2DPlatformer.Instance, teleportCooldownTime);
        }
        else
        {
            teleportCooldownTimer.OverrideTimer(teleportCooldownTime);
        }

        teleportCooldownTimer.OnCountdownEnd -= OnCountdownEnd_Teleport;
        teleportCooldownTimer.OnCountdownEnd += OnCountdownEnd_Teleport;

        //TeleportBossEnemy();
        bossEnemyPhase = bossEnemy.currentPhase;
    }

    public void Execute(float? deltaTime = null)
    {
        if (!teleportCooldownTimer.IsRunning)
        {
            teleportCooldownTimer.BeginTimer();
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
    }

    private void TeleportBossEnemy()
    {
        // no longer the same phase - needed because timer works separately from phase logic
        if (bossEnemy.currentPhase != bossEnemyPhase) return;

        int teleportIndex = teleportPositionIndices_Sequence[currSequenceIndex];
        bossEnemy.gameObject.transform.globalPosition = teleportPositions[teleportIndex];

        currSequenceIndex++;
        if (currSequenceIndex >= teleportPositionIndices_Sequence.Length)
        {
            currSequenceIndex = 0;
        }

        OnTeleport?.Invoke(this, EventArgs.Empty);
    }
}
