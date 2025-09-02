using System;

internal class StopBossMovementComponentData : IComponentTypeData
{
    public string Type { get; set; }
    public int BossGameObjectID { get; set; } = 0;
    public int StoppedTimeSeconds { get; set; } = 1;

    public static StopBossMovementComponent Deserialize(StopBossMovementComponentData data, string PerformActionName = null)
    {
        // Basic validation
        if (data.Type is null || data.Type == "")
        {
            throw new ArgumentException("Invalid JSON data for StopBossMovementComponent.");
        }

        // Return the StopBossMovementComponent instance
        return new StopBossMovementComponent(data.BossGameObjectID, data.StoppedTimeSeconds);
    }
}
