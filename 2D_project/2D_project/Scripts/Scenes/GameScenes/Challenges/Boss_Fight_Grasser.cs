using Microsoft.Xna.Framework;
using System.IO;

internal class Boss_Fight_Grasser(Game game) : ChallengeScene(game)
{
    protected override void InitializeContent()
    {
        base.InitializeContent();

        // reset player

        // Burner is the first boss
        challengeData = ExisitingPlayerSessions.GetActiveSession().Challenges[4];

        // TESTING
        SoundController.instance.PlayMusic(GameConstantsAndValues.FactionType.Grasser.ToString(), isRepeating: true);
        SoundController.instance.volume_music = 0;
    }
    protected override void AddJsonLevels()
    {
        Vector2 startPos = new Vector2(0, 0);

        
        respawnPoints = [
            new RespawnPointSystem.RespawnPoint(new Vector2(0, -32)),
        ];
        
        // create levels

        // 1.) Burner
        string jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge5.json");
        Scene_Stage.CreateLevel(this, jsonFilePath, levelStartPos: startPos);

        // TODO -> make it also work with respawn system
        Player.Instance.ResetPlayer(new Vector2(0, -32));
        //Player.Instance.EnableCheatMode();
    }
}

