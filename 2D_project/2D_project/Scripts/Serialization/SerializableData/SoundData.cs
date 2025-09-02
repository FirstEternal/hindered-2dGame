using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

internal class SoundData
{
    private static List<SoundTypeData> sounds;

    public class SoundTypeData
    {
        public bool IsSoundEffect { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
    }

    /// <summary>
    /// Loads all sound data from a JSON file and stores it in the `sounds` list.
    /// </summary>
    /// <param name="jsonFilePath">The path to the JSON file.</param>
    public static void LoadFromJson(string jsonFilePath)
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            sounds = JsonConvert.DeserializeObject<List<SoundTypeData>>(jsonContent);
        }
        else
        {
            throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");
        }
    }

    /// <summary>
    /// Loads all the sound assets from the `sounds` list into the `SoundController`.
    /// </summary>
    /// <param name="game">The Game instance to load content.</param>
    public static void LoadAllSounds(Game game)
    {
        if (sounds != null)
        {
            foreach (SoundTypeData soundTypeData in sounds)
            {
                if (soundTypeData.IsSoundEffect)
                {
                    string path = Path.Combine("sounds", "sfx", soundTypeData.FileName);
                    SoundController.instance.AddSoundEffect(soundTypeData.Name, game.Content.Load<SoundEffect>(path));
                }
                else
                {
                    string path = Path.Combine("sounds", "music", soundTypeData.FileName);
                    SoundController.instance.AddMusic(soundTypeData.Name, game.Content.Load<Song>(path));
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Sound data not loaded. Please call LoadFromJson first.");
        }
    }
}
