using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

internal class EnemySpawnerData : IComponentTypeData
{
    public string Type { get; set; }

    public List<string> EnemyTypes { get; set; }
    public List<Vector2> EnemySpawnPositions { get; set; }

    public List<string> BossEnemyTypes { get; set; }
    public List<Vector2> BossEnemySpawnPositions { get; set; }
    public List<Vector2> BossEnemyArrivalPositions { get; set; }
    public List<int> InitiateFightGameObjectIDs { get; set; }

    public static EnemySpawner Deserialize(EnemySpawnerData data)
    {

        if (data.Type == null
            || (data.EnemyTypes == null && data.BossEnemyTypes == null)
            // condition 1: only enemies
            || (data.EnemyTypes != null
                && (data.EnemySpawnPositions == null
                    || data.EnemyTypes.Count != data.EnemySpawnPositions.Count))
            // condition 2: only boss enemies
            || (data.BossEnemyTypes != null
               && (data.BossEnemyArrivalPositions == null
                   || data.BossEnemySpawnPositions == null
                   || data.InitiateFightGameObjectIDs == null
                   || data.BossEnemyTypes.Count != data.BossEnemySpawnPositions.Count
                   || data.BossEnemyTypes.Count != data.BossEnemyArrivalPositions.Count
                   || data.BossEnemyTypes.Count != data.InitiateFightGameObjectIDs.Count)))
            throw new ArgumentException("Enemy spawner has issues.");

        List<Vector2> spawnPositions = data.EnemySpawnPositions;

        if (data.EnemyTypes != null)
        {
            for (int i = 0; i < data.EnemyTypes.Count; i++)
            {
                spawnPositions[i] = data.EnemySpawnPositions[i] * GameConstantsAndValues.SQUARE_TILE_WIDTH;
            }
        }

        List<Vector2> bossSpawnPositions = data.BossEnemySpawnPositions;
        List<Vector2> bossArrivalPositions = data.BossEnemyArrivalPositions;
        if (data.BossEnemyTypes != null)
        {
            for (int i = 0; i < data.BossEnemyTypes.Count; i++)
            {
                bossSpawnPositions[i] = data.BossEnemySpawnPositions[i] * GameConstantsAndValues.SQUARE_TILE_WIDTH;
                bossArrivalPositions[i] = data.BossEnemyArrivalPositions[i] * GameConstantsAndValues.SQUARE_TILE_WIDTH;
            }
        }

        return new EnemySpawner(data.EnemyTypes, spawnPositions, data.BossEnemyTypes, bossSpawnPositions, bossArrivalPositions, data.InitiateFightGameObjectIDs);
    }
}
