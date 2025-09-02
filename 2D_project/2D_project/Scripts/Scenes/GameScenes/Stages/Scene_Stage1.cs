using Microsoft.Xna.Framework;
using System.IO;

internal class Scene_Stage1(Game game) : Scene_Stage(game)
{
    protected override void AddJsonLevels()
    {
        Vector2 part1 = new Vector2(0, 0);
        Vector2 part2 = new Vector2(16000, -1600);
        Vector2 part3 = new Vector2(20000, 0);
        Vector2 part4 = new Vector2(40000, -1600);

        respawnPoints = [
            new RespawnPointSystem.RespawnPoint(new Vector2(200, -200)),
            new RespawnPointSystem.RespawnPoint(new Vector2(16000, 0)),
            new RespawnPointSystem.RespawnPoint(new Vector2(20000, -200)),
            new RespawnPointSystem.RespawnPoint(new Vector2(40000, 0)),
        ];

        // create levels

        // 1.) Burner
        string part1_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level1.json");
        string boss_Burner_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge1.json");

        CreateLevel(this, jsonFilePath: part1_jsonFilePath, levelStartPos: part1);
        CreateLevel(this, jsonFilePath: boss_Burner_jsonFilePath, levelStartPos: part2);

        // 2.) Drowner
        string part2_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level1_part2.json");
        string boss_Drowner_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge2.json");

        //CreateLevel(this, jsonFilePath: part2_jsonFilePath, levelStartPos: part3);
        //CreateLevel(this, jsonFilePath: boss_Drowner_jsonFilePath, levelStartPos: part4);
    }

    /*

    private void UnlockPart3()
    {
        // Requirement -> defeat Burner
    }*/
}