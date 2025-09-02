using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Diagnostics;

public class SceneManager //: Singleton<SceneManager>
{
    // The singleton instance
    private static SceneManager? instance;

    // Lock object for thread safety
    private static readonly object lockObject = new object();

    // Private constructor to prevent direct instantiation
    private SceneManager()
    {
        // Initialization logic, if needed
    }

    // Public property to access the singleton instance
    public static SceneManager Instance
    {
        get
        {
            // First check without locking for better performance
            if (instance == null)
            {
                // Lock to ensure only one thread can enter this block
                lock (lockObject)
                {
                    // Double-check in case another thread created the instance
                    if (instance == null)
                    {
                        instance = new SceneManager();
                    }
                }
            }
            return instance;
        }
    }

    private Dictionary<int, Scene> scenes = new Dictionary<int, Scene>();
    public EventHandler OnSceneChange;

    public Scene? GetGameObjectScene(GameObject gameObject)
    {
        for (int i = 0; i < scenes.Count; i++)
        {
            // check if object is in the gameObjects
            if (scenes[i].gameObjects.Contains(gameObject))
            {
                return scenes[i];
            }

            // check if object is in the overlay gameObjects
            if (scenes[i].overlayGameObjects.Contains(gameObject))
            {
                return scenes[i];
            }
        }
        return null;
    }

    public void RemoveGameObjectFromScene(GameObject gameObject)
    {
        for (int i = 0; i < scenes.Count; i++)
        {
            // check if object is in the gameObjects
            if (scenes[i].gameObjects.Contains(gameObject))
            {
                scenes[i].gameObjects.Remove(gameObject);
            }

            // check if object is in the overlay gameObjects
            if (scenes[i].overlayGameObjects.Contains(gameObject))
            {
                scenes[i].overlayGameObjects.Remove(gameObject);
            }
        }
    }

    public Scene? activeScene { get; private set; }
    public Renderer? activeRenderer { get; private set; }
    public DebugRenderer? activeDebugRenderer { get; private set; }
    public OverlayRenderer? activeOverlayRenderer { get; private set; }

    public void AddScene(Scene scene, int sceneIndex)
    {
        if (scenes.ContainsKey(sceneIndex)) return;
        scenes[sceneIndex] = scene;
    }

    public void LoadScene(int sceneIndex)
    {
        if (!scenes.TryGetValue(sceneIndex, out Scene? scene)) return;
        activeScene = scene;
        OnSceneChange?.Invoke(this, EventArgs.Empty);
        scene.Initialize();
    }
    public void LoadScene(string sceneScriptName)
    {
        Debug.WriteLine($"load initiated: {sceneScriptName}");
        // Loop through the dictionary to find the scene by its name
        foreach (var key in scenes)
        {
            Scene scene = key.Value;

            if (scene.GetType().Name.Equals(sceneScriptName, StringComparison.OrdinalIgnoreCase))
            {
                activeScene = scene;
                OnSceneChange?.Invoke(this, EventArgs.Empty);
                scene.Initialize();
                return;
            }
        }

        // no matching scene was found
    }

    /// <summary>
    /// Renders in order: 
    /// Scene Renderer -> Debug Renderer -> Overlay Renderer
    /// </summary>
    /// <param name="gameTime"></param>
    public void RenderScene(GameTime gameTime)
    {
        activeRenderer?.Draw(gameTime);
        activeDebugRenderer?.Draw(gameTime);
        activeOverlayRenderer?.Draw(gameTime);
    }

    public void ChangeRenderer(Renderer renderer)
    {
        activeRenderer = renderer;
    }

    public void ChangeDebugRenderer(DebugRenderer renderer)
    {
        activeDebugRenderer = renderer;
    }

    public void ChangeOverlayRenderer(OverlayRenderer renderer)
    {
        activeOverlayRenderer = renderer;
    }
}
