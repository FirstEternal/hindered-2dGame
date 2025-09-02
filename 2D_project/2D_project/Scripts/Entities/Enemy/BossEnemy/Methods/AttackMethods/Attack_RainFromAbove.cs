using Microsoft.Xna.Framework;
using System;

internal class Attack_RainFromAbove : IBossMethod
{
    public Attack_RainFromAbove(Entity entity)
    {
        this.entity = entity;
    }

    private float projectileDeathTimer;
    private GameConstantsAndValues.FactionType projectileType;
    private float projectileWidth = 100f;
    private Vector2 projectileScale;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private float fallHeight;
    private bool hasProjectileTerrainImunity;
    private bool reverseSpread;
    private Entity entity;

    private int rainDirection;

    private const float minProjectileDistance = 64f;

    public void ResetSubsteps()
    {
        return; // nothing to reset yet
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length < 8)
            throw new ArgumentException("Invalid number of parameters for SpreadAndFall");

        projectileDeathTimer = (float)parameters[0];
        projectileType = (GameConstantsAndValues.FactionType)parameters[1];
        projectileScale = (Vector2)parameters[2];
        startPosition = (Vector2)parameters[3];
        endPosition = (Vector2)parameters[4];
        fallHeight = (float)parameters[5];
        hasProjectileTerrainImunity = (bool)parameters[6];
        reverseSpread = (bool)parameters[7];

        if (parameters.Length > 8)
        {
            rainDirection = (int)parameters[8];
        }
        else rainDirection = 1;
    }

    public void ReverseCurrentSpreadLogic()
    {
        reverseSpread = !reverseSpread;
    }

    public void Execute(float? deltaTime = null)
    {
        Vector2 direction = endPosition - startPosition;
        float totalDistance = direction.Length();

        float spacing = (projectileWidth * projectileScale.X) + minProjectileDistance;

        int projectileCount = (int)Math.Floor(totalDistance / spacing);
        if (projectileCount <= 0)
            return;

        Vector2 step = Vector2.Normalize(direction) * spacing;
        Vector2 halfStep = step * 0.5f;

        for (int i = 0; i <= projectileCount; i++)
        {
            Vector2 spawnPosition = startPosition + (step * i);
            if (reverseSpread)
                spawnPosition += halfStep;

            Vector2 destination = new Vector2(spawnPosition.X, spawnPosition.Y + rainDirection * fallHeight);

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
