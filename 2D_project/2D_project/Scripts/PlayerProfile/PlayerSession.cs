internal class PlayerSession(string SessionName, bool[] Stages, ChallengeData[] Challanges, bool[] AcquiredElements, float[] playerStats)
{
    public string SessionName { get; set; } = SessionName;
    public bool[] Stages { get; set; } = Stages;
    public ChallengeData[] Challenges { get; set; } = Challanges;
    public bool[] AcquiredElements { get; set; } = AcquiredElements;
    public float[] PlayerStats { get; set; } = playerStats; // hp, shield, atk dmg, crit rate, crit dmg

    // TODO -> add level saves
}
