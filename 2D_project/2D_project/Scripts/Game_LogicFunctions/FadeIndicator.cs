using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class FadeIndicator : DrawableGameComponent
{
    private Texture2D pixelTexture;
    private Vector2 position;
    private int width, height;
    private float duration;
    private float elapsed;
    private Color color;

    private SpriteBatch spriteBatch;
    private bool isActive = false;
    private bool hasFiredCallback = false;

    public void Pause()
    {
        isActive = false;
    }

    public void Resume()
    {
        isActive = true;
    }

    // 🔔 Event triggered when fade completes
    public event Action<FadeIndicator> OnFadeComplete;

    public FadeIndicator(Game game, Vector2 position, int width = 50, int height = 50, float duration = 1.0f)
        : base(game)
    {
        this.position = position;
        this.width = width;
        this.height = height;
        this.duration = duration;
        this.elapsed = 0f;
        this.color = Color.Red;

        Game.Components.Add(this);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        pixelTexture.SetData(new[] { Color.White });
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (!isActive)
            return;

        elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (elapsed >= duration)
        {
            isActive = false;

            // 🔔 Trigger the event once when fade ends
            if (!hasFiredCallback)
            {
                hasFiredCallback = true;
                OnFadeComplete?.Invoke(this);
            }
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if (!isActive)
            return;

        float alpha = MathHelper.Clamp(1 - (elapsed / duration), 0, 1);

        Camera camera = SceneManager.Instance.activeScene.mainCamera;
        Matrix transform = camera.GetTransformation(GraphicsDevice);

        spriteBatch.Begin(transformMatrix: transform);
        spriteBatch.Draw(
            pixelTexture,
            new Rectangle(
                (int)(position.X - width / 2f),
                (int)(position.Y - height / 2f),
                width,
                height),
            color * alpha
        );
        spriteBatch.End();

        base.Draw(gameTime);
    }

    public void Start(Vector2 newPosition, float? newDuration = null, int width = 0, int height = 0)
    {
        position = newPosition;
        duration = newDuration ?? duration;
        elapsed = 0f;
        isActive = true;
        hasFiredCallback = false; // Reset callback flag
        if (width != 0) this.width = width;
        if (height != 0) this.height = height;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            pixelTexture?.Dispose();
            spriteBatch?.Dispose();
        }

        base.Dispose(disposing);
    }
}
