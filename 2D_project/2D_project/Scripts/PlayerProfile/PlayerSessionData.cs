using Newtonsoft.Json;
using System;

internal class PlayerSessionData
{
    public string SessionName { get; set; }

    // TODO -> add everything else

    // 0). unlocked stages
    public bool[] Stages { get; set; }

    // 1.) unlocked challenges
    public ChallengeData[] Challanges { get; set; }

    // 2.) unlocked elements
    public bool[] AcquiredElements { get; set; }

    // 3.) HP, Shield, Attack Damage, Crit Rate, Crit Damage
    public float[] PlayerStats { get; set; }


    // 4.) sound settings -> master, music, sfx
    public float[] soundValues { get; set; }

    // 5.) graphic settings -> resolution
    public int resolutionIndex;

    // 6.) gameplay controls -> keybinds
    public char[] keybindChars;

    public static string SerializePlayerSessionData(PlayerSession playerSession)
    {
        var data = new PlayerSessionData
        {
            SessionName = playerSession.SessionName,
            Stages = playerSession.Stages,
            Challanges = playerSession.Challenges,
            AcquiredElements = playerSession.AcquiredElements,
            PlayerStats = playerSession.PlayerStats
        };

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        return json;
    }
    public static PlayerSession DeserializePlayerSessionData(string json)
    {
        PlayerSessionData data = JsonConvert.DeserializeObject<PlayerSessionData>(json);

        // Basic validation (optional but recommended)
        if (string.IsNullOrEmpty(data.SessionName))
        {
            throw new ArgumentException("Invalid JSON data");
        }
        PlayerSession sessionData = new PlayerSession(
            SessionName: data.SessionName,
            Stages: data.Stages,
            Challanges: data.Challanges,
            AcquiredElements: data.AcquiredElements,
            playerStats: data.PlayerStats
        );

        return sessionData;
    }
}
