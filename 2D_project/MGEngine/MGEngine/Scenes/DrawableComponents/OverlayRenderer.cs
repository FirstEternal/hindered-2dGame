using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
/// <summary>
/// Renders overlay UI after entire scene renders
/// </summary>
public class OverlayRenderer : DrawableGameComponent
{
    public SpriteBatch spriteBatch { get; private set; }

    public OverlayRenderer(Game game) : base(game)
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    /// <summary>
    ///  Draw overlay UI over active scene
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
        if (!Enabled || SceneManager.Instance.activeScene is null) return;

        Matrix renderTransformMatrix = GameWindow.Instance?.gameScaleMatrix ?? Matrix.CreateScale(1, 1, 1);
        spriteBatch.Begin(transformMatrix: renderTransformMatrix);

        // change batch but do not enter camera coordinates
        //spriteBatch.Begin(); 

        foreach (GameObject gameObject in SceneManager.Instance.activeScene.overlayGameObjects)
        {
            if (!gameObject.isActive) continue;
            gameObject.GetComponent<Sprite>()?.Draw(spriteBatch);
            gameObject.GetComponent<SpriteTextComponent>()?.Draw(spriteBatch);
        }
        spriteBatch.End();

        base.Draw(gameTime);
    }
}
