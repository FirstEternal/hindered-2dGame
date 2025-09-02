using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

internal class CollisionTESTscene(Game game) : TestingScene(game)
{

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        /*
        foreach(var i in projectiles)
        {
            Debug.WriteLine(i.gameObject.transform.globalScale);
        }*/
    }

    List<Projectile> projectiles = new List<Projectile>();
    List<Projectile> projectiles1 = new List<Projectile>();
    public override void LoadContent()
    {
        base.LoadContent();

        //Test1();
        //Test2();
        /*
        Player.Instance.ResetPlayer(this, globalPosition: new Vector2(-500, 0));
        Player.Instance.EnableCheatMode();
        */
        //M(projectiles);
        Z1(1, projectiles);
        Z(-1, projectiles1);

        /*
        Projectile projectile0 = ProjectilePoolingSystem.GetSpawnedProjectile(
    deathTimer: 10,
    GameConstantsAndValues.FactionType.Boulderer,
    spawnPosition: new Vector2(0, 0),
    spawnRotation: 0,
    destination: new Vector2(0, 0),
    projectileOwner: null,
    spawnScale: new Vector2(0.2f, 0.2f)
);
        projectile0.gameObject.id = 4444;
        projectile0.BeginMovement();
        projectile0.AngularVelocity = 1;
        CircularMovement cM = new CircularMovement(CenterPoint: Vector2.Zero, Radius: 10);
        projectile0.gameObject.AddComponent(cM);
        projectile0.UpdateSpeed(speed: -500, angularSpeed: 5);

        Z(1, projectiles);
        Z(-1, projectiles1);
        */
    }

    private void Test1()
    {
        // both squares
        AddGameObjectToScene(CreateOOBOBject(width: 100, height: 100, -1, new Vector2(100, 150), mass: 10, rotation: 0), isOverlay: false);

        AddGameObjectToScene(CreateOOBOBject(width: 100, height: 100, 1, new Vector2(-100, 150), mass: 50, rotation: (float)Math.PI / 2), isOverlay: false);


        // both squares
        AddGameObjectToScene(CreateOOBOBject(width: 50, height: 100, -1, new Vector2(100, -150), mass: 10, rotation: 0), isOverlay: false);

        AddGameObjectToScene(CreateOOBOBject(width: 100, height: 100, 1, new Vector2(-100, -150), mass: 50, rotation: (float)Math.PI / 2), isOverlay: false);

    }

    private void Test2()
    {
        // both squares
        AddGameObjectToScene(CreateOOBOBject(width: 50, height: 100, -1, new Vector2(100, 150), mass: 10, rotation: 0), isOverlay: false);

        AddGameObjectToScene(CreateOOBOBject(width: 50, height: 100, 1, new Vector2(-100, 150), mass: 50, rotation: (float)Math.PI / 2), isOverlay: false);


        // both squares
        AddGameObjectToScene(CreateOOBOBject(width: 100, height: 50, -1, new Vector2(100, -150), mass: 10, rotation: 0), isOverlay: false);

        AddGameObjectToScene(CreateOOBOBject(width: 50, height: 100, 1, new Vector2(-100, -150), mass: 50, rotation: (float)Math.PI / 2), isOverlay: false);

    }

    private void M(List<Projectile> projectiles1)
    {
        float path = 1;
        Projectile projectile1 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
          GameConstantsAndValues.FactionType.Boulderer,
          spawnPosition: new Vector2(path * -200, -140),
          spawnRotation: 0,
          destination: new Vector2(path * 0, -143),
          projectileOwner: null,
          spawnScale: new Vector2(0.2f, 0.2f)
      );

        Projectile projectile2 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
            GameConstantsAndValues.FactionType.Burner,
            spawnPosition: new Vector2(path * -200, -100),
            spawnRotation: 0,
            destination: new Vector2(path * 0, -100),
            projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile3 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
    GameConstantsAndValues.FactionType.Shader,
spawnPosition: new Vector2(path * -200, -60),
spawnRotation: 0,
destination: new Vector2(path * 0, 0),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile4 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
GameConstantsAndValues.FactionType.Grasser,
spawnPosition: new Vector2(path * -200, -20),
spawnRotation: 0,
destination: new Vector2(path * 0, 100),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile5 = AttackObjectPoolingSystem.GetSpawnedProjectile(
            deathTimer: 10,
GameConstantsAndValues.FactionType.Froster,
spawnPosition: new Vector2(path * -200, 20),
spawnRotation: 0,
destination: new Vector2(path * -20, 150),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile6 = AttackObjectPoolingSystem.GetSpawnedProjectile(
    deathTimer: 10,
GameConstantsAndValues.FactionType.Drowner,
spawnPosition: new Vector2(path * -200, 40),
spawnRotation: 0,
destination: new Vector2(path * -20, 150),
projectileOwner: null,
    spawnScale: new Vector2(0.2f, 0.2f));

        projectiles1.Add(projectile1);
        projectiles1.Add(projectile2);
        projectiles1.Add(projectile3);
        projectiles1.Add(projectile4);
        projectiles1.Add(projectile5);
        projectiles1.Add(projectile6);

        int i = 0;
        foreach (var item in projectiles1)
        {
            item.BeginMovement();
            item.UpdateSpeed(0.15f - i * 0.01f);
            item.isPlayerSpawned = path == -1 ? true : false;
            i++;
        }
    }

    private void Z1(int path, List<Projectile> projectiles1)
    {
        Projectile projectile0 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                deathTimer: 10,
  GameConstantsAndValues.FactionType.Boulderer,
  spawnPosition: new Vector2(path * -250, 0),
  spawnRotation: 0,
  destination: new Vector2(path * -250, -150),
  projectileOwner: null,
  spawnScale: new Vector2(0.2f, 0.2f)
);
        Projectile projectile1 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
          GameConstantsAndValues.FactionType.Boulderer,
          spawnPosition: new Vector2(path * -200, -143),
          spawnRotation: 0,
          destination: new Vector2(path * 0, -143),
          projectileOwner: null,
          spawnScale: new Vector2(0.2f, 0.2f)
      );

        Projectile projectile2 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
            GameConstantsAndValues.FactionType.Burner,
            spawnPosition: new Vector2(path * -200, -100),
            spawnRotation: 0,
            destination: new Vector2(path * 0, -100),
            projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));


        Projectile projectile3 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
    GameConstantsAndValues.FactionType.Grasser,
spawnPosition: new Vector2(path * 0, 0),
spawnRotation: -MathF.PI,
destination: new Vector2(path * 0, 0),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        projectile3.gameObject.transform.localRotationAngle = -MathF.PI;
        Projectile projectile4 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
GameConstantsAndValues.FactionType.Grasser,
spawnPosition: new Vector2(path * -200, 50),
spawnRotation: 0,
destination: new Vector2(path * 0, 105),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile5 = AttackObjectPoolingSystem.GetSpawnedProjectile(
            deathTimer: 10,
GameConstantsAndValues.FactionType.Grasser,
spawnPosition: new Vector2(path * -200, 250),
spawnRotation: 0,
destination: new Vector2(path * -20, 150),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        projectiles1.Add(projectile0);
        projectiles1.Add(projectile1);
        projectiles1.Add(projectile2);
        projectiles1.Add(projectile3);
        projectiles1.Add(projectile4);
        projectiles1.Add(projectile5);

        int i = projectiles1.Count;
        foreach (var item in projectiles1)
        {
            item.BeginMovement();
            item.UpdateSpeed(0.15f - i * 0.01f);
            item.isPlayerSpawned = true;
            i++;
        }
    }

    private void Z(int path, List<Projectile> projectiles1)
    {
        Projectile projectile0 = AttackObjectPoolingSystem.GetSpawnedProjectile(
        deathTimer: 10,
GameConstantsAndValues.FactionType.Drowner,
spawnPosition: new Vector2(-250, -150),
spawnRotation: 0,
destination: new Vector2(-250, 0),
projectileOwner: null,
spawnScale: new Vector2(0.2f, 0.2f)
);
        Projectile projectile1 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
          GameConstantsAndValues.FactionType.Burner,
          spawnPosition: new Vector2(path * -200, -143),
          spawnRotation: 0,
          destination: new Vector2(path * 0, -143),
          projectileOwner: null,
          spawnScale: new Vector2(0.2f, 0.2f)
      );

        Projectile projectile2 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
            GameConstantsAndValues.FactionType.Burner,
            spawnPosition: new Vector2(path * -200, -100),
            spawnRotation: 0,
            destination: new Vector2(path * 0, -100),
            projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile3 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
    GameConstantsAndValues.FactionType.Burner,
spawnPosition: new Vector2(path * -200, -50),
spawnRotation: 0,
destination: new Vector2(path * 0, 0),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile4 = AttackObjectPoolingSystem.GetSpawnedProjectile(
                        deathTimer: 10,
GameConstantsAndValues.FactionType.Boulderer,
spawnPosition: new Vector2(path * -200, 100),
spawnRotation: 0,
destination: new Vector2(path * 0, 100),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        Projectile projectile5 = AttackObjectPoolingSystem.GetSpawnedProjectile(
            deathTimer: 10,
GameConstantsAndValues.FactionType.Grasser,
spawnPosition: new Vector2(path * -200, 250),
spawnRotation: 0,
destination: new Vector2(path * -20, 150),
projectileOwner: null,
            spawnScale: new Vector2(0.2f, 0.2f));

        projectiles1.Add(projectile0);
        projectiles1.Add(projectile1);
        projectiles1.Add(projectile2);
        projectiles1.Add(projectile3);
        projectiles1.Add(projectile4);
        projectiles1.Add(projectile5);

        int i = projectiles1.Count;
        foreach (var item in projectiles1)
        {
            item.BeginMovement();
            item.UpdateSpeed(0.15f - i * 0.01f);
            item.isPlayerSpawned = false;
            i++;
        }
    }

    private GameObject CreateOOBOBject(int width, int height, int VelocityDirection, Vector2 globalPosition, float mass, float rotation)
    {
        GameObject g = new GameObject();
        g.CreateTransform(localRotationAngle: rotation);
        g.transform.globalPosition = globalPosition;
        OBBRectangleCollider oBBRectangleCollider = new OBBRectangleCollider(width: width, height: height, isAftermath: true);
        g.AddComponent(oBBRectangleCollider);
        PhysicsComponent p = new PhysicsComponent(mass: mass, velocity: new Vector2(10 * VelocityDirection, 0), isGravity: false);
        g.AddComponent(p);

        return g;
    }

}