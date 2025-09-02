using System;

internal class ChallengeData(float[] starConditionArray)
{
    public bool isUnlocked { get; set; } = false;
    public double challangeTimeScore { get; set; } = 10000;
    public bool[] starAchievedArray { get; set; } = [false, false, false];
    public float[] starConditionArray { get; set; } = starConditionArray ?? [0, 0, 0]; // example: 90s, 60s, 30s (first one is the easiest)

    public void SerializeChallangeData()
    {

    }

    public ChallengeData DeSerializeChallangeData(object data)
    {
        return null;
    }

    public void UpdateTimeScore(TimeSpan challangeTotalSeconds)
    {
        challangeTimeScore = Math.Round(challangeTotalSeconds.TotalSeconds, 2);
    }
}
