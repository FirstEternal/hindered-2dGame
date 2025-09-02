using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GameWindow
{
    public Game? game;
    public GraphicsDeviceManager? graphics;
    public static GameWindow? Instance { get; private set; }

    private readonly int defaultWindowWidth;
    private readonly int defaultWindowHeight;


    public int windowWidth => game?.GraphicsDevice.Viewport.Width ?? 0;
    public int windowHeight => game?.GraphicsDevice.Viewport.Height ?? 0;

    public GameWindow(Game game, GraphicsDeviceManager graphics, int defaultWidth, int defaultHeight)
    {
        if (Instance is not null) return;

        if (game is null) throw new ArgumentNullException(nameof(game));
        this.game = game;

        if (graphics is null) throw new ArgumentNullException(nameof(graphics));
        this.graphics = graphics;
        Instance = this;

        defaultWindowWidth = defaultWidth;
        defaultWindowHeight = defaultHeight;
        UpdateViewPort(defaultWidth, defaultHeight);
    }

    public void EnableResizing(bool enable)
    {
        if (game is null) return;

        game.Window.AllowUserResizing = enable;

        if (game.Window.AllowUserResizing)
        {
            game.Window.ClientSizeChanged -= OnDragResize;
            game.Window.ClientSizeChanged += OnDragResize;
        }
    }

    public void OnDragResize(object? sender, EventArgs e)
    {
        //gameScale = new Vector2(windowWidth / defaultWindowWidth, windowHeight / defaultWindowHeight);

        // update game scale
        gameScaleMatrix = GetScalingMatrix(windowWidth, windowHeight);
    }

    public enum ScreenResolution
    {
        // Standard smaller resolutions
        RES_1280x720, // HD (1280x720)
        RES_1280x1024, // SXGA (1280x1024)
        RES_1366x768, // HD (1366x768)
        RES_1440x900, // WXGA+ (1440x900)
        RES_1600x900, // HD+ (1600x900)

        // Standard larger resolutions
        RES_1920x1080, // Full HD (1920x1080)
        RES_2560x1440, // QHD (2560x1440)
        RES_3840x2160 // 4K (3840x2160)
    }

    public void SetToFullScreen()
    {
        if (graphics is null || game is null) return;

        // Get the current display mode's width and height
        int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        // Set the back buffer to match the screen
        graphics.PreferredBackBufferWidth = screenWidth;
        graphics.PreferredBackBufferHeight = screenHeight;

        // Set to full screen
        graphics.IsFullScreen = true;
        graphics.ApplyChanges();

        // Update the viewport and scaling
        UpdateViewPort(screenWidth, screenHeight);
    }


    public void SetResolution(ScreenResolution resolution)
    {
        if (graphics is null) return;

        if (graphics.IsFullScreen)
        {
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        string[] enumText = resolution.ToString().Split("_");
        string[] resValues = enumText[1].Split("x");
        UpdateViewPort(newWidth: int.Parse(resValues[0]), newHeight: int.Parse(resValues[1]));
    }
    private void UpdateViewPort(int newWidth, int newHeight)
    {
        if (game is null || graphics is null) return;

        // update gameScaleMatrix
        gameScaleMatrix = GetScalingMatrix(newWidth, newHeight);

        // Only manually set the viewport if NOT in fullscreen
        if (!graphics.IsFullScreen)
        {
            game.GraphicsDevice.Viewport = new Viewport(0, 0, newWidth, newHeight);
        }
        else
        {
            // In full screen, ensure the viewport matches the back buffer
            game.GraphicsDevice.Viewport = new Viewport(0, 0,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight);
        }

        // Set back buffer size
        graphics.PreferredBackBufferWidth = newWidth;
        graphics.PreferredBackBufferHeight = newHeight;

        // Apply changes
        graphics.ApplyChanges();
    }


    public Vector2 GetWindowSize()
    {

        return game is null ? Vector2.Zero : new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
    }

    public Matrix gameScaleMatrix;
    public Vector2 gameScale;

    private Matrix GetScalingMatrix(int newWidth, int newHeight)
    {
        float scaleX = (float)newWidth / defaultWindowWidth;
        float scaleY = (float)newHeight / defaultWindowHeight;

        gameScale = new Vector2(scaleX, scaleY);
        return Matrix.CreateScale(scaleX, scaleY, 1f);
    }
}