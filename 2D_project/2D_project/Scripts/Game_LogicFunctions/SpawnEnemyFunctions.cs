
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class SpawnEnemyFunctions
{
    public static BossEnemy SpawnBossEnemy(Scene scene, BossEnemy bossEnemy, Vector2 spawnPosition, /*Vector2 arrivalPosition,*/ Reciever reciever)
    {
        scene.AddGameObjectToScene(bossEnemy.gameObject, isOverlay: false); // add to scene
        bossEnemy.gameObject.transform.spawnPosition = spawnPosition;

        if (reciever is not null) reciever.OnActionPerformed += bossEnemy.ResetEnemyViaEvent;
        return bossEnemy;
    }

    public static BossEnemy SpawnBossEnemy(Scene scene, BossEnemy bossEnemy, Vector2 spawnPosition, /*Vector2 arrivalPosition,*/ GameObject initiateFightGameObject)
    {
        scene.AddGameObjectToScene(bossEnemy.gameObject, isOverlay: false); // add to scene
        bossEnemy.gameObject.transform.spawnPosition = spawnPosition;

        MoveStopOnCollisionComponent moveStopOnCollisionComponent = initiateFightGameObject.GetComponent<MoveStopOnCollisionComponent>();
        TeleportObject teleportObject = initiateFightGameObject.GetComponent<TeleportObject>();

        if (moveStopOnCollisionComponent is not null) moveStopOnCollisionComponent.OnStop += bossEnemy.ResetEnemyViaEvent;
        if (teleportObject is not null) teleportObject.OnPlayerTeleport += bossEnemy.ResetEnemyViaEvent;
        return bossEnemy;
    }

    public static void SpawnEnemy()
    {

    }
}
