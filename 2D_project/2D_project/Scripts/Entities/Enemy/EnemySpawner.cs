using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class EnemySpawner(List<string> enemyTypes, List<Vector2> enemyPositions, List<string> bossEnemyTypes, List<Vector2> bossEnemyPositions, List<Vector2> bossEnemyArrivalPositions, List<int> moveStopPlaftormId) : ObjectComponent
{
    List<string> enemyTypes = enemyTypes;
    List<Vector2> enemyPositions = enemyPositions;

    List<string> bossEnemyTypes = bossEnemyTypes;
    List<Vector2> bossEnemyPositions = bossEnemyPositions;
    List<Vector2> bossEnemyArrivalPositions = bossEnemyArrivalPositions;
    List<int> initiateFIghtGameObjectId = moveStopPlaftormId;

    public void SpawnEnemies(Scene scene, Vector2 startPosition)
    {
        if (enemyTypes is null) return;
        for (int i = 0; i < enemyTypes.Count; i++)
        {
            string enemyTypeName = enemyTypes[i];

            Type enemyType = Type.GetType($"{enemyTypeName}");

            if (enemyType != null && typeof(Enemy).IsAssignableFrom(enemyType))
            {
                GameObject enemyObject = new GameObject();
                enemyObject.CreateTransform();
                Vector2 spawnPosition = startPosition + enemyPositions[i];
                enemyObject.transform.spawnPosition = spawnPosition;
                enemyObject.transform.globalPosition = spawnPosition;
                // Use reflection to create an instance with the required constructor
                var constructor = enemyType.GetConstructor(new Type[] { typeof(float), typeof(Vector2?), typeof(Vector2?), typeof(bool), typeof(bool) });
                Enemy enemy = (Enemy)constructor.Invoke(new object[] { 1000, null, null, true, true });

                //Enemy enemy = (Enemy)Activator.CreateInstance(enemyType, null);

                enemyObject.AddComponent(enemy);
                scene.AddGameObjectToScene(enemyObject, isOverlay: false); // add to scene
                enemy.ResetEnemy(spawnPosition);
            }
            else
            {
                Console.WriteLine($"Enemy type '{enemyTypeName}' not found or invalid.");
            }
        }
    }

    public void SpawnBossEnemies(Scene scene, Vector2 startPosition)
    {
        if (bossEnemyTypes is null) return;
        for (int i = 0; i < bossEnemyTypes.Count; i++)
        {
            string enemyTypeName = bossEnemyTypes[i];

            Type enemyType = Type.GetType($"{enemyTypeName}");

            if (enemyType != null && typeof(Enemy).IsAssignableFrom(enemyType))
            {
                GameObject enemyObject = new GameObject();
                enemyObject.CreateTransform();
                Vector2 spawnPosition = startPosition + bossEnemyPositions[i];
                Vector2 arrivalPosition = startPosition + bossEnemyArrivalPositions[i];

                enemyObject.transform.spawnPosition = spawnPosition;
                enemyObject.transform.globalPosition = spawnPosition;
                // Use reflection to create an instance with the required constructor
                var constructor = enemyType.GetConstructor(new Type[] { typeof(Vector2), typeof(float), typeof(Vector2?), typeof(Vector2?), typeof(bool), typeof(bool) });
                BossEnemy enemy = (BossEnemy)constructor.Invoke(new object[] { arrivalPosition, 1000, null, null, false, true });
                enemyObject.id = GameConstantsAndValues.bossMinId + enemy.baseId;
                //Enemy enemy = (Enemy)Activator.CreateInstance(enemyType, null);

                enemyObject.AddComponent(enemy);
                scene.AddGameObjectToScene(enemyObject, isOverlay: false); // add to scene
                //enemy.ResetEnemy(spawnPosition);


                SpawnEnemyFunctions.SpawnBossEnemy(
                    scene: scene,
                    bossEnemy: enemy,
                    spawnPosition: spawnPosition,
                    initiateFightGameObject: scene.GetGameObjectInScene(gameObjectID: initiateFIghtGameObjectId[i], isOverlay: false)
                );

                if (scene is ChallengeScene challengeScene)
                {
                    challengeScene.finalEnemy = enemy;
                }
            }
            else
            {
                Console.WriteLine($"Enemy type '{enemyTypeName}' not found or invalid.");
            }
        }
    }
}
