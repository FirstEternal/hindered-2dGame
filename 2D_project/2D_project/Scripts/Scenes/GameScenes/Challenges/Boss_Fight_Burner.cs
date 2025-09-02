using Microsoft.Xna.Framework;
using System.IO;

internal class Boss_Fight_Burner(Game game) : ChallengeScene(game)
{
    protected override void InitializeContent()
    {
        base.InitializeContent();

        // reset player
        //Player.Instance.ResetPlayer(globalPosition: new Vector2(0, 1600));
        //Player.Instance.EnableCheatMode();

        // Burner is the first boss
        challengeData = ExisitingPlayerSessions.GetActiveSession().Challenges[0];

        // TESTING
        SoundController.instance.PlayMusic(GameConstantsAndValues.FactionType.Burner.ToString(), isRepeating: true);
        SoundController.instance.volume_music = 50;
    }

    protected override void AddJsonLevels()
    {
        Vector2 startPos = new Vector2(0, -1600);

        respawnPoints = [
            new RespawnPointSystem.RespawnPoint(new Vector2(0, 0)),
        ];
        // create levels

        // 1.) Burner
        string jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge1.json");
        Scene_Stage.CreateLevel(this, jsonFilePath, levelStartPos: startPos);

        // TODO -> make it also work with respawn system
        Player.Instance.ResetPlayer(new Vector2(0, 0));
        //Player.Instance.EnableCheatMode();
    }
}
