using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

internal class Weapon_Bow : Weapon
{
    protected bool hasProjectileTerrainImunity = false;
    internal Weapon_Bow(
        Vector2 localPosition,
        ImbuedElement imbuedElement,        // Element imbued in the weapon 

        // Combat stats
        int damage,                         // Base damage of the weapon
        float critRate,                     // Chance of a critical hit
        float critMultiplier,               // Multiplier for critical damage

        // Projectile properties
        Vector2? projectileBaseScale = null,
        int projectileCount = 1,                // Number of projectiles fired
        float projectileSaparatorDistance = 0,  // Distance between projectiles

        // Reloading properties
        float reloadTimer = 0,                // reload time
        float chargeTime = 1f
    )
    {
        this.imbuedElement = imbuedElement;
        this.damage = damage;
        this.critRate = critRate;
        this.weaponType = WeaponType.Bow;
        this.critMultiplier = critMultiplier;
        this.projectileBaseScale = projectileBaseScale ?? Vector2.One;
        this.projectileCount = projectileCount;
        this.projectileSaparatorDistance = projectileSaparatorDistance;
        this.reloadTimer = reloadTimer;
        this.localPosition = localPosition;
        this.chargeTime = chargeTime;
    }

    // Characteristics
    protected Vector2 projectileBaseScale;
    protected int projectileCount;
    protected int currProjectileCount;
    protected float projectileSaparatorDistance;

    private List<Projectile> projectiles = new List<Projectile>();

    public override void SpawnGameObject(string[] weaponSpriteFrameNames, Texture2D weaponSprite)
    {
        base.SpawnGameObject(weaponSpriteFrameNames, weaponSprite);

        gameObject.transform.localScale = new Vector2(1.25f, 1.25f);
        gameObject.transform.localPosition = new Vector2(0, -80);

        // calculates sourceRectangles , weaponSpriteFrameNames
        Rectangle[] sourceRectangles = new Rectangle[weaponSpriteFrameNames.Length];

        for (int i = 0; i < weaponSpriteFrameNames.Length; i++)
        {
            Rectangle sourceRectangle = JSON_Manager.GetWeaponBowSourceRectangle(weaponSpriteFrameNames[i]);

            sourceRectangles[i] = sourceRectangle;
        }

        // first frame lasts 0.5f on all weapon, last frame lasts indefenitely
        float[] frameTimers = [int.MaxValue, int.MaxValue, int.MaxValue];
        float[] layerDepths = [0.76f, 0.76f, 0.76f];

        // TODO assign arrays
        spriteAnimated = new SpriteAnimated(
            texture2D: weaponSprite,
            sourceRectangles: sourceRectangles,
            frameTimers: frameTimers,
            isAnimationDisabled: true,
            layerDepths: layerDepths);
        gameObject.AddComponent(spriteAnimated);

        // add under a player object
        Player.Instance.gameObject.AddChild(gameObject);
    }

    public override void BeginCharging(Vector2 destinationVector)
    {
        base.BeginCharging(destinationVector);
        if (weaponType != WeaponType.Bow || imbuedElement == ImbuedElement.None)
        {
            //This function should only be called while equiped with a Bow weapon
            // every bow has an element
            return;
        }

        gameObject.GetComponent<SpriteAnimated>().animationEnabled = true;

        projectiles.Clear();

        SpawnProjectiles(destinationVector);
    }

    public override void Attack()
    {
        currChargingValue = 0;
        currChargingLevel = CHARGED_LEVEL1;

        foreach (Projectile projectile in projectiles)
        {
            projectile.BeginMovement(projectile.gameObject.transform.globalPosition, destination: MouseGameObject.Singleton.GetMouseWorldSpacePosition());
        }
        // also pause charging animation
        gameObject.GetComponent<SpriteAnimated>().animationEnabled = false;
    }

    // update weapon direction
    public override void Update(GameTime gameTime)
    {
        /*
        MouseState mouseState = MouseGameObject.Singleton.MouseState;
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        mousePos = SceneManager.Instance.activeScene.mainCamera.ScreenToWorld(mousePos, Game2DPlatformer.Instance.GraphicsDevice);
        */
        AssignWeaponDirection(aimPosition: MouseGameObject.Singleton.GetMouseWorldSpacePosition());
    }

    public override void UpdateWeaponChargeValue(GameTime gameTime, Vector2 destinationVector)
    {
        float prevChargingLevel = currChargingLevel;

        UpdateChargeValue(gameTime);

        int currentSeparatorIndex;

        // get tangent line to spawn projectiles at sperarator distance
        Vector2 tangentVector = GetTangentVector(gameObject.transform.globalPosition, destinationVector);

        // update everything:  scale, direction ...
        if (currChargingLevel > prevChargingLevel)
        {
            SpawnProjectiles(destinationVector); // spawn more if charged values changed

            for (int i = 0; i < projectiles.Count; i++)
            {
                currentSeparatorIndex = UpdateSeperatorIndex(projectiles.Count, i);

                // calculate separation distance on the tangent to spawn projectile 
                Vector2 spawnPosition = gameObject.transform.globalPosition + tangentVector * currentSeparatorIndex * projectileSaparatorDistance * currChargingLevel;

                Projectile projectile = projectiles[i];
                projectile.UpdateValues(currChargingLevel, spawnPosition, destinationVector);
            }

            return;
        }
        // update only direction
        for (int i = 0; i < projectiles.Count; i++)
        {
            currentSeparatorIndex = UpdateSeperatorIndex(projectiles.Count, i);

            // calculate separation distance on the tangent to spawn projectile 
            Vector2 spawnPosition = gameObject.transform.globalPosition + tangentVector * currentSeparatorIndex * projectileSaparatorDistance * currChargingLevel;

            Projectile projectile = projectiles[i];
            projectile.AssignDirection(spawnPosition, destinationVector);
        }
    }

    public static Vector2 GetTangentVector(Vector2 spawnVector, Vector2 destinationVector)
    {
        Vector2 normalVector = destinationVector - spawnVector;
        normalVector.Normalize();

        // tangent vector is (-Y, X) of normal vector
        return new Vector2(-normalVector.Y, normalVector.X);
    }

    public void AssignWeaponDirection(Vector2 aimPosition)
    {
        // define translation
        Vector2 direction = aimPosition - gameObject.transform.globalPosition;
        if (direction != Vector2.Zero) direction.Normalize();
        // define rotation
        // Get the angle in degrees between the direction vector and the right vector (default forward vector in 2D)
        float angle = (float)(Math.Atan2(direction.Y, direction.X));

        // Rotate the object (around the Z-axis in 2D)
        gameObject.transform.localRotationAngle = angle;
    }

    private int GetCurrentProjectileCount()
    {
        switch (currChargingLevel)
        {
            case CHARGED_LEVEL1:
                return ((projectileCount + 1) / 2 + 1) / 2;
            case CHARGED_LEVEL2:
                return (projectileCount + 1) / 2;
            case CHARGED_LEVEL3:
                return projectileCount;
            default:
                return 1;
        }
    }

    private void SpawnProjectiles(Vector2 destinationVector)
    {
        GameConstantsAndValues.FactionType projectileType;
        Enum.TryParse(imbuedElement.ToString(), out projectileType);

        int currentSeparatorIndex;

        // get tangent line to spawn projectiles at sperarator distance
        Vector2 tangentVector = GetTangentVector(gameObject.transform.globalPosition, destinationVector);


        for (int i = projectiles.Count /* spawn the remaining */; i < GetCurrentProjectileCount(); i++)
        {
            currentSeparatorIndex = UpdateSeperatorIndex(projectileCount: projectiles.Count, i);

            // calculate separation distance on the tangent to spawn projectile 
            Vector2 spawnPosition = gameObject.transform.globalPosition + tangentVector * currentSeparatorIndex * projectileSaparatorDistance * currChargingLevel;

            Projectile projectile = AttackObjectPoolingSystem.GetSpawnedProjectile(
                deathTimer: 10,
                projectileType: projectileType,
                spawnPosition: spawnPosition,
                spawnRotation: 0,
                destination: destinationVector,
                projectileOwner: Player.Instance,
                spawnScale: projectileBaseScale,
                hasProjectileTerrainImunity: hasProjectileTerrainImunity
            );

            //projectile.UpdateValues(chargeValue: currChargingLevel, spawnPosition: spawnPosition, destination: destinationVector);
            projectile.gameObject.GetComponent<PhysicsComponent>().isGravity = false;
            projectiles.Add(projectile);
        }
    }

    public static int UpdateSeperatorIndex(int projectileCount, int i)
    {
        if (projectileCount % 2 == 1)
        {
            // Odd case: 0, -1, 1, -2, 2
            return (i % 2 == 0) ? i / 2 : -(i / 2 + 1);
        }
        else
        {
            // Even case: -1, 1, -2, 2
            return (i % 2 == 0) ? -(i / 2 + 1) : (i / 2 + 1);
        }
    }

    public void ReleaseAttackOnDeath()
    {
        // Player died release all dangling projectiles
        Vector2 deathPosition = new Vector2(int.MaxValue, int.MaxValue); 
        foreach (Projectile projectile in projectiles) {

            projectile.SpawnToDie(deathPosition);
        }
    }
}
