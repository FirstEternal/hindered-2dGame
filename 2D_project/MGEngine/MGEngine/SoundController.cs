using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

public class SoundController : GameComponent
{
    public float volume_soundEffects
    {
        get => _volume_soundEffects;
        set
        {
            _volume_soundEffects = Math.Clamp(value, 0f, 1f);
            UpdateAllVolumes();
            OnVolumeChanged?.Invoke();
        }
    }
    private float _volume_soundEffects = 0.5f;

    public float volume_UI
    {
        get => _volume_UI;
        set
        {
            _volume_UI = Math.Clamp(value, 0f, 1f);
            OnVolumeChanged?.Invoke();
        }
    }
    private float _volume_UI = 0.5f;

    public float volume_Ambient
    {
        get => _volume_Ambient;
        set
        {
            _volume_Ambient = Math.Clamp(value, 0f, 1f);
            OnVolumeChanged?.Invoke();
        }
    }
    private float _volume_Ambient = 0.5f;

    private float _volume_master = 1f;
    public float volume_master
    {
        get => _volume_master;
        set
        {
            _volume_master = Math.Clamp(value, 0f, 1f);
            UpdateAllVolumes();
            OnVolumeChanged?.Invoke();
        }
    }

    private float _volume_music = 1f;
    public float volume_music
    {
        get => _volume_music;
        set
        {
            _volume_music = Math.Clamp(value, 0f, 1f);
            UpdateMusicVolume();
            OnVolumeChanged?.Invoke();
        }
    }

    public event Action? OnVolumeChanged;

    public static SoundController? instance { get; private set; }
    public SoundController(Game game) : base(game)
    {
        if (instance is null) instance = this;
    }

    private double cleanupTimer = 0;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        cleanupTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (cleanupTimer >= 1.0) // Perform cleanup every 1 second
        {
            CleanUpFinishedSoundEffects();
            cleanupTimer = 0;
        }
    }

    Dictionary<string, SoundEffect> gameSoundEffectsDictionary = new Dictionary<string, SoundEffect>();
    Dictionary<string, Song> gameMusicDictionary = new Dictionary<string, Song>();

    private List<(SoundEffect, SoundEffectInstance)> activeSoundEffects = new List<(SoundEffect, SoundEffectInstance)>();


    private void UpdateAllVolumes()
    {
        foreach (var (_, effectInstance) in activeSoundEffects) // Destructure the tuple
        {
            if (effectInstance.State != SoundState.Stopped) // Only update active instances
            {
                effectInstance.Volume = volume_master * volume_soundEffects;
            }
        }

        UpdateMusicVolume();
    }

    private void UpdateMusicVolume() { MediaPlayer.Volume = volume_master * volume_music; }

    public void AddSoundEffect(string name, SoundEffect effect)
    {
        gameSoundEffectsDictionary[name] = effect;
    }
    public void PlaySoundEffect(string name, float volume = 1, float pitch = 0, float pan = 0)
    {
        if (volume < 0.0f || volume > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be between 0.0 and 1.0.");

        if (pitch < -1.0f || pitch > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(pitch), "Pitch must be between -1.0 and 1.0.");

        if (pan < -1.0f || pan > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(pan), "Pan must be between -1.0 and 1.0.");

        // Check if there are already 5 instances of the same sound effect
        int currentInstances = activeSoundEffects.Count(pair =>
            pair.Item1 == gameSoundEffectsDictionary[name] && pair.Item2.State != SoundState.Stopped);

        if (currentInstances >= 5)
            return; // Skip if there are already 5 active instances

        // Create and configure the new instance
        var effectInstance = gameSoundEffectsDictionary[name].CreateInstance();
        effectInstance.Volume = volume_master * volume_soundEffects * volume;
        effectInstance.Pitch = pitch;
        effectInstance.Pan = pan;

        activeSoundEffects.Add((gameSoundEffectsDictionary[name], effectInstance));
        effectInstance.Play();
    }

    private void CleanUpFinishedSoundEffects()
    {
        // Remove instances that are no longer playing
        activeSoundEffects.RemoveAll(pair => pair.Item2.State == SoundState.Stopped);
    }

    public void AddMusic(string name, Song song)
    {
        gameMusicDictionary[name] = song;
    }

    public void PlayMusic(string name, bool isRepeating, float volume = 1)
    {
        MediaPlayer.Volume = volume_master * volume_music * volume;
        MediaPlayer.Play(gameMusicDictionary[name]);
        MediaPlayer.IsRepeating = isRepeating;
    }

    public void StopMusic() { MediaPlayer.Stop(); }
    public void PauseMusic() { MediaPlayer.Pause(); }
    public void ResumeMusic() { MediaPlayer.Resume(); }
}
