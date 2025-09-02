using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

internal class Attack_ChargedShotTowardsPlayer : IBossMethod
{
    public Attack_ChargedShotTowardsPlayer(BossEnemy bossEnemy)
    {
        this.bossEnemy = bossEnemy;
    }

    private float projectileDeathTimer;
    private GameConstantsAndValues.FactionType projectileType;
    private Vector2 projectileStartScale;
    private Vector2 projectileEndScale;
    private bool hasProjectileTerrainImunity;

    private float currChargingTime;
    private float maxChargingTime;
    private bool isCharging;

    private readonly BossEnemy bossEnemy; // Reference to the game object
    private Projectile projectile;
    private float projectileAngularVelocity;

    public EventHandler OnProjectileShoot { get; set; }
    private bool hasFired;

    public void ResetSubsteps()
    {
        currChargingTime = maxChargingTime;
        isCharging = false;
        hasFired = false;

        projectile = null;
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 7)
            throw new ArgumentException("Invalid number of parameters for Attack_ChargedShoot");

        projectileDeathTimer = (float)parameters[0];
        projectileType = (GameConstantsAndValues.FactionType)parameters[1];
        projectileStartScale = (Vector2)parameters[2];
        projectileEndScale = (Vector2)parameters[3];
        maxChargingTime = (float)parameters[4];
        projectileAngularVelocity = (float)parameters[5];
        hasProjectileTerrainImunity = (bool)parameters[6];

        currChargingTime = maxChargingTime;

        ResetSubsteps();
    }

    public void Execute(float? deltaTime = null)
    {
        // needs to be reset before creating a new charging shot
        if (hasFired) return;

        if (deltaTime is null)
        {
            Debug.WriteLine("Should put deltaTime otherwise charged attack will not charge");
            return;
        }

        if (!isCharging)
        {
            isCharging = true;
            // spawn projectile
            Vector2 spawnPosition = bossEnemy.gameObject.transform.globalPosition;
            Vector2 destination = Player.Instance.gameObject.transform.globalPosition;
            projectile = AttackObjectPoolingSystem.GetSpawnedProjectile(
                deathTimer: projectileDeathTimer,
                projectileType: projectileType,
                spawnPosition: spawnPosition,
                spawnRotation: 0,
                destination: spawnPosition,
                projectileOwner: bossEnemy,
                spawnScale: projectileStartScale,
                hasProjectileTerrainImunity: hasProjectileTerrainImunity,
                hasProjectileImmunity: true
            );
        }
        else
        {
            currChargingTime -= deltaTime ?? 0;

            if (currChargingTime <= 0)
            {
                Vector2 spawnPosition = bossEnemy.gameObject.transform.globalPosition;
                Vector2 destination = Player.Instance.gameObject.transform.globalPosition;

                // end of charging
                projectile.BeginMovement(spawnPosition, destination);
                OnProjectileShoot?.Invoke(this, EventArgs.Empty);
                hasFired = true;
                return;
            }

            // increase scale
            float chargeProgress = 1f - (currChargingTime / maxChargingTime);
            float easedProgress = MathHelper.SmoothStep(0f, 1f, chargeProgress);
            projectile.gameObject.transform.localScale = Vector2.Lerp(projectileStartScale, projectileEndScale, easedProgress);


            // Apply constant rotation to the projectile based on projectileAngularVelocity
            if (projectileAngularVelocity > 0)
            {
                projectile.gameObject.transform.localRotationAngle += (float)deltaTime * projectileAngularVelocity;
            }
        }
    }
}
