using MGEngine.ObjectBased;
using System.Collections.Generic;

internal class StopBossMovementComponent(int BossGameObjectID, int stoppedTimeSeconds) : ObjectComponent
{
    private int BossGameObjectID = BossGameObjectID;
    public BossEnemy bossEnemy { get; private set; }
    public int stoppedTimeSeconds { get; private set; } = stoppedTimeSeconds;

    public void AssignBossEnemy(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.id == BossGameObjectID)
            {
                bossEnemy = gameObject.GetComponent<BossEnemy>();
                break; // Break early when found
            }
        }
    }
}