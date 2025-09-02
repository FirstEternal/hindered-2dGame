using Microsoft.Xna.Framework;
using System;

internal class Attack_InCircle : IBossMethod
{
    public Attack_InCircle(Entity entitiy)
    {
        this.entity = entitiy;
    }

    private float projectileDeathTimer;
    private GameConstantsAndValues.FactionType projectileType;
    private int projectileCount;
    private Vector2 projectileScale;
    private int degreeStart;
    private int degreeEnd;
    private bool isLeftToRight;

    private Entity entity;
    private bool hasProjectileTerrainImunity;

    public void ResetSubsteps()
    {
        return; // nothing to reset yet
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length < 8)
            throw new ArgumentException("Invalid number of parameters for SpawnProjectilesMethod");

        projectileDeathTimer = (float)parameters[0];
        projectileType = (GameConstantsAndValues.FactionType)parameters[1];
        projectileCount = (int)parameters[2];
        projectileScale = (Vector2)parameters[3];
        degreeStart = (int)parameters[4];
        degreeEnd = (int)parameters[5];
        isLeftToRight = (bool)parameters[6];
        hasProjectileTerrainImunity = (bool)parameters[7];
    }

    public void Execute(float? deltaTime = null)
    {
        float degreeStep = MathF.Abs(degreeEnd - degreeStart) / (projectileCount - 1); // first at degreeStart, last at degreeEnd

        for (int i = 0; i < projectileCount; i++)
        {
            float radius = 3;

            // Adjust the degree step to ensure that the first and last projectiles fire at degreeStart and degreeEnd
            float currentDegree = degreeStart + i * degreeStep;

            // Convert degrees to radians because Math.Cos and Math.Sin use radians
            float angleInRadians = MathF.PI / 180 * currentDegree;

            // Calculate the x and y position using the angle
            float posX = MathF.Cos(angleInRadians);
            float posY = -MathF.Sin(angleInRadians);

            // Adjust direction based on isLeftToRight flag
            if (!isLeftToRight)
            {
                posX = -posX;
            }

            // Calculate spawn position and destination based on the cycle offset and radius
            Vector2 spawnPosition = entity.gameObject.transform.globalPosition - new Vector2(posX, posY) * radius / 2;
            Vector2 destination = entity.gameObject.transform.globalPosition - new Vector2(posX, posY) * radius;

            // Spawn the projectile and aim it towards the player
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

            //projectile.gameObject.transform.localScale = projectileScale;
            projectile.BeginMovement(spawnPosition, destination);
        }
    }
}

