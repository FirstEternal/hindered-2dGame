using Microsoft.Xna.Framework;
public class FPSCounter : GameComponent
{
    public static FPSCounter? Instance { get; private set; } = null;
    public FPSCounter(Game game) : base(game)
    {
        if (Instance != null) return;

        Instance = this;
    }

    public float fps { get; private set; } = 0f;       // Current FPS
    public float frameTime { get; private set; } = 0f;     // Time for the current frame (in seconds)
    public int frameCount { get; private set; } = 0;      // Number of frames since last update

    public string GetFrameCount()
    {
        return fps.ToString();
    }

    public override void Update(GameTime gameTime)
    {
        // Calculate the time taken for this frame
        frameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        frameCount++;

        // Calculate FPS every 0.5 seconds
        if (frameTime >= 0.5f)  // Update FPS every 0.5 seconds
        {
            fps = frameCount / frameTime;
            frameTime = 0f;
            frameCount = 0;
        }

        base.Update(gameTime);
    }
}
