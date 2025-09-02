using MGEngine.ObjectBased;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

internal class LevelFactory
{
    /// <summary>
    /// make sure, that object list has exact name as json file for example "level1_objects", will find file: "level1_objects.json"
    /// </summary>
    /// <param name="level_Objects"></param>
    public static void DeserializeObjectList(List<GameObject> level_Objects, string jsonName)
    {
        string json = File.ReadAllText(string.Format("{0}.json", level_Objects.ToString())); // Load JSON file
        level_Objects = DeserializeLevel(json);

        foreach (var gameObject in level_Objects)
        {
            Console.WriteLine($"Loaded GameObject with tag: {gameObject.tag}");
        }
    }

    public static List<GameObject> DeserializeLevel(string json)
    {

        // Deserialize the JSON into the LevelData wrapper class
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(json);

        // Create the game objects + components
        List<GameObject> gameObjectList = new List<GameObject>();

        foreach (GameObjectData gameObjectData in levelData.gameObjects)
        {
            gameObjectList.Add(GameObjectData.Deserialize(gameObjectData));
        }
        return gameObjectList;
    }
}

internal class LevelData
{
    // GameObjects with Object components
    public List<GameObjectData> gameObjects { get; set; }
}

