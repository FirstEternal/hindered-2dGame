using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

internal class AttackObjectPoolingSystem(Game game)
{
    public static AttackObjectPoolingSystem Instance { get; private set; }
    public static Dictionary<GameConstantsAndValues.FactionType, List<Projectile>> ProjectileDictionary = new Dictionary<GameConstantsAndValues.FactionType, List<Projectile>>();
    public static List<Melee> MeleeList = [];

    private Game game = game;

    public void Initialize()
    {
        if (Instance is null) Instance = this;
    }

    public static void LoadContent()
    {
        // spawn n of each projectile type
        // n will be amount of possible projectiles on screen of the same type
        int n = 50; // lets start at 50
        foreach (GameConstantsAndValues.FactionType factionType in Enum.GetValues(typeof(GameConstantsAndValues.FactionType)))
        {
            List<Projectile> projectiles = new List<Projectile>();
            ProjectileDictionary[factionType] = projectiles;
            for (int i = 0; i < n; i++)
            {
                Projectile projectile = CreateProjectile(factionType);
                projectiles.Add(projectile);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            Melee melee = CreateMelee();
            MeleeList.Add(melee);
        }
    }

    private static Melee CreateMelee()
    {

        GameObject gameObject = new GameObject(5554);
        Melee meele = new Melee();
        gameObject.CreateTransform();
        gameObject.AddComponent(meele);

        meele.Initialize();
        meele.LoadContent();
        meele.gameObject.SetActive(false); // make them unActive -> they are here for later use so no need to spawn more later
        return meele;
    }

    private static Projectile CreateProjectile(GameConstantsAndValues.FactionType projectileType)
    {
        // Get the class by name
        Type classType = Type.GetType("Projectile_After" + projectileType.ToString());

        if (classType == null)
        {
            throw new Exception("Class not found exception");
        }
        // Create an instance of the class

        GameObject gameObject = new GameObject(5555);
        Projectile projectile = (Projectile)System.Activator.CreateInstance(classType);
        gameObject.CreateTransform();
        gameObject.AddComponent(projectile);
        //SceneManager.Instance.activeScene.gameObjects.Add(gameObject);

        projectile.Initialize();
        projectile.LoadContent();
        projectile.gameObject.SetActive(false); // make them unActive -> they are here for later use so no need to spawn more later
        return projectile;
    }

    public static Projectile GetSpawnedProjectile(float deathTimer, GameConstantsAndValues.FactionType projectileType, Vector2 spawnPosition, float spawnRotation, Vector2 destination, Entity projectileOwner, Vector2? spawnScale = null, bool hasProjectileTerrainImunity = false, bool hasProjectileImmunity = false)
    {
        foreach (Projectile projectile in ProjectileDictionary[projectileType])
        {
            if (!projectile.gameObject.isActive)
            {
                // found available projectile
                projectile.Spawn(deathTimer, spawnPosition: spawnPosition, spawnRotation: spawnRotation, destination: destination, projectileOwner: projectileOwner, spawnScale: spawnScale, hasProjectileTerrainImunity, hasProjectileImmunity: hasProjectileImmunity);
                return projectile;
            }
        }
        // in case all projectiles are unavailable, create a new one -> add it to the list and spawn it
        Projectile newProjectile = CreateProjectile(projectileType);
        ProjectileDictionary[projectileType].Add(newProjectile);
        newProjectile.Spawn(deathTimer, spawnPosition, spawnRotation, destination, projectileOwner, terrainImunity: hasProjectileTerrainImunity, hasProjectileImmunity: hasProjectileImmunity);
        return newProjectile;
    }

    public static Melee GetSpawnedMeele(float duration, Vector2 spawnPosition, Entity meleeOwner, int width, int height, int dmg, Vector2 knockDirection, float pushPower)
    {
        foreach (Melee melee in MeleeList)
        {
            if (!melee.gameObject.isActive)
            {
                // found available projectile
                melee.Spawn(Instance.game, duration, spawnPosition, width, height, meleeOwner, dmg, knockDirection, pushPower);
                return melee;
            }
        }
        // in case all melees are unavailable, create a new one -> add it to the list and spawn it
        Melee newMelee = CreateMelee();
        MeleeList.Add(newMelee);
        newMelee.Spawn(Instance.game, duration, spawnPosition, width, height, meleeOwner, dmg, knockDirection, pushPower);
        return newMelee;
    }
}

