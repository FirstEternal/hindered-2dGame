using Microsoft.Xna.Framework;
using System.IO;

internal class Boss_Fight_Boulderer(Game game) : ChallengeScene(game)
{
    protected override void InitializeContent()
    {
        base.InitializeContent();

        // reset player
        //Player.Instance.ResetPlayer(globalPosition: new Vector2(0, -1650));

        // Burner is the first boss
        challengeData = ExisitingPlayerSessions.GetActiveSession().Challenges[2];

        // TESTING
        SoundController.instance.PlayMusic(GameConstantsAndValues.FactionType.Boulderer.ToString(), isRepeating: true);
        SoundController.instance.volume_music = 0;
    }
    protected override void AddJsonLevels()
    {
        Vector2 startPos = new Vector2(0, 250);

        /*
        respawnPoints = [
            new RespawnPointSystem.RespawnPoint(new Vector2(0, 0)),
        ];
        */
        // create levels

        // 1.) Burner
        string jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge3.json");
        Scene_Stage.CreateLevel(this, jsonFilePath, levelStartPos: startPos);

        // TODO -> make it also work with respawn system
        Player.Instance.ResetPlayer(new Vector2(0, -1400));
        //Player.Instance.EnableCheatMode();
    }
}
