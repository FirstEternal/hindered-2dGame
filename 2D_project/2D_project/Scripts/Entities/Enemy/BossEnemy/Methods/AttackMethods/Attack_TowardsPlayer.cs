using Microsoft.Xna.Framework;
using System;

internal class Attack_TowardsPlayer : IBossMethod
{
    public Attack_TowardsPlayer(Enemy enemy)
    {
        this.enemy = enemy;
    }

    private float projectileDeathTimer;
    private GameConstantsAndValues.FactionType projectileType;
    private int projectileCount;
    private Vector2 projectileScale;
    private int projectileSeparatorDistance; // for spawning multiple
    private bool hasProjectileTerrainImunity;

    /*
    private float currReloadTime;
    private float maxReloadTime;
    private bool isAttacking;
    private bool isReloading;*/

    private readonly Enemy enemy; // Reference to the game object

    public void ResetSubsteps()
    {
        return; // nothing to reset yet
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 6)
            throw new ArgumentException("Invalid number of parameters for SpawnProjectilesMethod");

        projectileDeathTimer = (float)parameters[0];
        projectileType = (GameConstantsAndValues.FactionType)parameters[1];
        projectileCount = (int)parameters[2];
        projectileScale = (Vector2)parameters[3];
        projectileSeparatorDistance = (int)parameters[4];
        hasProjectileTerrainImunity = (bool)parameters[5];

        //currReloadTime = (float)parameters[6];      
        //maxReloadTime = (float)parameters[7];      
    }

    public void Execute(float? deltaTime = null)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Vector2 spawnPosition = new Vector2(enemy.gameObject.transform.globalPosition.X,
                                                 enemy.gameObject.transform.globalPosition.Y + i * projectileSeparatorDistance);
            Vector2 destination = Player.Instance.gameObject.transform.globalPosition;
            Projectile projectile = AttackObjectPoolingSystem.GetSpawnedProjectile(
                deathTimer: projectileDeathTimer,
                projectileType: projectileType,
                spawnPosition: spawnPosition,
                spawnRotation: 0,
                destination: destination,
                projectileOwner: enemy,
                spawnScale: projectileScale,
                hasProjectileTerrainImunity: hasProjectileTerrainImunity
            );

            projectile.BeginMovement(spawnPosition, destination);
        }
    }
}
