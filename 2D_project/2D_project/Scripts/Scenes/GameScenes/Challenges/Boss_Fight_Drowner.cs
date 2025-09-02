using Microsoft.Xna.Framework;
using System.IO;

internal class Boss_Fight_Drowner(Game game) : ChallengeScene(game)
{
    protected override void InitializeContent()
    {
        base.InitializeContent();

        // reset player
        //Player.Instance.ResetPlayer(new Vector2(0, 1040));

        challengeData = ExisitingPlayerSessions.GetActiveSession().Challenges[1];

        // TESTING
        SoundController.instance.PlayMusic(GameConstantsAndValues.FactionType.Drowner.ToString(), isRepeating: true);
        SoundController.instance.volume_music = 0;
    }
    protected override void AddJsonLevels()
    {
        Vector2 startPos = new Vector2(0, -1600);

        respawnPoints = [
            new RespawnPointSystem.RespawnPoint(new Vector2(0, -560)),
        ];
        // create levels

        // 1.) Burner
        string jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge2.json");
        Scene_Stage.CreateLevel(this, jsonFilePath, levelStartPos: startPos);

        // TODO -> make it also work with respawn system
        Player.Instance.ResetPlayer(new Vector2(0, -560));
        //Player.Instance.EnableCheatMode();
    }
}

