using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

internal class Scene_Stage2(Game game) : Scene_Stage(game)
{
    List<Enemy> enemies = new List<Enemy>();
    protected override void AddJsonLevels()
    {
        Vector2 part1 = new Vector2(-60000, 0);
        Vector2 part2 = new Vector2(0, 0);
        Vector2 part3 = new Vector2(20000, 0);
        Vector2 part4 = new Vector2(35000, 0);
        Vector2 part5 = new Vector2(-2080, -200);
        Vector2 part6 = new Vector2(-22080, -2000);
        Vector2 part7 = new Vector2(-42080, -2000);
        Vector2 part8 = new Vector2(-51680, -3050);

        // create levels
        respawnPoints = [
            //new RespawnPointSystem.RespawnPoint(part7 + spawnPointAdjustment),
            //
            new RespawnPointSystem.RespawnPoint(part1 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part2 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part3 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part4 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part5 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part6 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part7 + spawnPointAdjustment),
            new RespawnPointSystem.RespawnPoint(part8 + spawnPointAdjustment),
        ];

        // 0.) Choice
        string part0_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level2_part0.json");
        CreateLevel(this, jsonFilePath: part0_jsonFilePath, levelStartPos: part1);

        // 1.) Boulderer
        string part1_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level2_part1.json");
        string part2_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level2_part2.json");

        string boulderer_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge3.json");

        //CreateLevel(this, jsonFilePath: part1_jsonFilePath, levelStartPos: part2);
        //CreateLevel(this, jsonFilePath: part2_jsonFilePath, levelStartPos: part3);
        //CreateLevel(this, jsonFilePath: boulderer_jsonFilePath, levelStartPos: part4);


        string part3_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level2_part3.json");
        string part4_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level2_part4.json");
        string part5_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_level2_part5.json");
        string froster_jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Serialization", "JsonFiles", "JSON_challenge4.json");
        CreateLevel(this, jsonFilePath: part3_jsonFilePath, levelStartPos: part5);
        CreateLevel(this, jsonFilePath: part4_jsonFilePath, levelStartPos: part6);
        //CreateLevel(this, jsonFilePath: part5_jsonFilePath, levelStartPos: part7);
        //CreateLevel(this, jsonFilePath: froster_jsonFilePath, levelStartPos: part8);
    }
}