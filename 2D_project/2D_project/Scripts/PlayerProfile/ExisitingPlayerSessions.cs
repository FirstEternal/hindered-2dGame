using System;

internal class ExisitingPlayerSessions
{
    public enum SessionIndex
    {
        SESSION_0,
        SESSION_1,
        SESSION_2
    }
    private const int SESSION_COUNT = 3;

    private static PlayerSession[] playerSessions = new PlayerSession[SESSION_COUNT];

    private static PlayerSession activeSession;

    public static EventHandler OnSaveLoadDelete;

    public static void LoadPlayerSessions()
    {
        for (int i = 0; i < SESSION_COUNT; i++)
        {
            string player_session_data = SaveSystem.Load($"SESSION_{i}.ps");

            playerSessions[i] = (player_session_data is null) ? null : PlayerSessionData.DeserializePlayerSessionData(player_session_data);
        }

        OnSaveLoadDelete?.Invoke(null, EventArgs.Empty);
    }

    public static void SavePlayerSessions()
    {
        for (int i = 0; i < SESSION_COUNT; i++)
        {
            if (playerSessions[i] is null) continue;
            string player_session_data = SaveSystem.Load($"SESSION_{i}.ps");

            //SaveSystem.Save($"sessionFileName_{i}.ps", PlayerSessionData.SerializePlayerSessionData(playerSessions[i]));
            SaveSystem.Save($"SESSION_{i}.ps", PlayerSessionData.SerializePlayerSessionData(playerSessions[i]));
        }

        OnSaveLoadDelete?.Invoke(null, EventArgs.Empty);
    }

    public static PlayerSession GetPlayerSession(SessionIndex sessionIndex)
    {
        return playerSessions[(int)sessionIndex];
    }

    public static PlayerSession GetActiveSession()
    {
        return activeSession;
    }

    public static void MakeSessionActive(SessionIndex sessionIndex)
    {
        activeSession = GetPlayerSession(sessionIndex);
    }

    public static void CreateSession(SessionIndex sessionIndex)
    {
        string sessionName = sessionIndex.ToString();
        string fileName = $"{sessionName}.ps";

        // TODO ADJUST 
        ChallengeData[] challanges = [
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
            new ChallengeData(starConditionArray:[90, 60, 30]),
        ];

        PlayerSession playerSession = new PlayerSession(
            sessionName,
            Stages: [true, false, false, false],
            Challanges: challanges,
            AcquiredElements: [false, false, false, false, false, false, false],
            playerStats: [100, 50, 5, 0.5f, 0.2f] // hp, shield, atk dmg, crit rate, crit dmg
        );

        SaveSystem.Save(fileName, PlayerSessionData.SerializePlayerSessionData(playerSession));

        // load all sessions again
        LoadPlayerSessions();

        //playerSessions[(int)sessionIndex] = playerSession;
    }

    public static void DeleteSession(SessionIndex sessionIndex)
    {
        // TODO
        playerSessions[(int)sessionIndex] = null;
        SaveSystem.Delete($"SESSION_{sessionIndex}.ps");
        OnSaveLoadDelete?.Invoke(null, EventArgs.Empty);
    }

    public static void SaveActiveSession()
    {
        if (activeSession is null) return;

        for (int i = 0; i < SESSION_COUNT; i++)
        {
            PlayerSession session = playerSessions[i];
            if (session == activeSession)
            {
                SaveSystem.Save($"SESSION_{i}.ps", PlayerSessionData.SerializePlayerSessionData(activeSession));
                break;
            }
        }

        OnSaveLoadDelete?.Invoke(null, EventArgs.Empty);
    }
}