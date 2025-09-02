
using Microsoft.Xna.Framework;
using System;

internal class Attack_InLineTowardsPlayer : IBossMethod
{
    public Attack_InLineTowardsPlayer(Entity entity)
    {
        this.entity = entity;
    }

    private float projectileDeathTimer;
    private GameConstantsAndValues.FactionType projectileType;
    private int projectileCount;
    private Vector2 projectileScale;
    private int projectileSeparatorDistance;
    private bool isLeftToRight;
    private bool hasProjectileTerrainImunity;
    private Entity entity;
    public void ResetSubsteps()
    {
        return; // nothing to reset yet
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 7)
            throw new ArgumentException("Invalid number of parameters for SpawnProjectilesMethod");

        projectileDeathTimer = (float)parameters[0];
        projectileType = (GameConstantsAndValues.FactionType)parameters[1];
        projectileCount = (int)parameters[2];
        projectileScale = (Vector2)parameters[3];
        projectileSeparatorDistance = (int)parameters[4];
        isLeftToRight = (bool)parameters[5];

        hasProjectileTerrainImunity = (bool)parameters[6];
    }

    public void Execute(float? deltaTime = null)
    {
        Vector2 spawnPosition;
        Vector2 destination;
        int currentSeparatorIndex;

        for (int i = 0; i < projectileCount; i++)
        {
            currentSeparatorIndex = (i % 2 == 0) ? i / 2 : -(i / 2 + 1);
            int separationDistance = currentSeparatorIndex * projectileSeparatorDistance;

            if (isLeftToRight)
            {
                spawnPosition = new Vector2(entity.gameObject.transform.globalPosition.X,
                                            entity.gameObject.transform.globalPosition.Y + separationDistance);
                destination = new Vector2(Player.Instance.gameObject.transform.globalPosition.X,
                                          entity.gameObject.transform.globalPosition.Y + separationDistance);
            }
            else
            {
                spawnPosition = new Vector2(entity.gameObject.transform.globalPosition.X + separationDistance,
                                            entity.gameObject.transform.globalPosition.Y);
                destination = new Vector2(entity.gameObject.transform.globalPosition.X + separationDistance,
                                          Player.Instance.gameObject.transform.globalPosition.Y);
            }

            Projectile projectile = AttackObjectPoolingSystem.GetSpawnedProjectile(
                deathTimer: projectileDeathTimer,
                projectileType: projectileType,
                spawnPosition: spawnPosition,
                spawnRotation: 0,
                destination: destination,
                projectileOwner: entity,
                spawnScale: projectileScale,
                hasProjectileTerrainImunity: hasProjectileTerrainImunity
            );

            projectile.BeginMovement(spawnPosition, destination);
        }
    }
}

