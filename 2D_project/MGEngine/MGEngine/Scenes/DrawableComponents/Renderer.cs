using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Renderer : DrawableGameComponent
{
    public SpriteBatch spriteBatch { get; private set; }

    public Renderer(Game game) : base(game)
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }
    /*
    protected override void LoadContent()
    {
        base.LoadContent();
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }*/

    // TODO use all other functions
    public Color clearColor = Color.CornflowerBlue;

    // TODO create overlay layer
    // TODO create game canvas layer

    // TODO create draw order

    /// <summary>
    ///  Draw active scene
    /// </summary>
    ///    

    public override void Draw(GameTime gameTime)
    {
        if (!Enabled || SceneManager.Instance.activeScene is null) return;
        // clear screen
        //GraphicsDevice.Clear(clearColor);

        // transform matrix -> camera transform matrix already includes game scaling
        Matrix renderTransformMatrix = SceneManager.Instance.activeScene.mainCamera.GetTransformation(GraphicsDevice);
        // begin sprite batch
        spriteBatch.Begin(SpriteSortMode.BackToFront, transformMatrix: renderTransformMatrix, blendState: BlendState.AlphaBlend);
        //spriteBatch.Begin(SpriteSortMode.Texture, transformMatrix: renderTransformMatrix, blendState:BlendState.AlphaBlend);

        // TESTING
        Scene scene = SceneManager.Instance.activeScene;
        GridScene gridScene = scene as GridScene;
        if (gridScene != null)
        {
            var cameraBounds = gridScene.GetCameraBounds();
            gridScene.GetNearbyCells(cameraBounds);
        }
        //

        // draw each active Game Object
        foreach (GameObject gameObject in SceneManager.Instance.activeScene.gameObjects)
        {
            // check if gameObject is active and if it is spawned in world space
            if (!gameObject.isActive || gameObject.transform is null) continue;

            // check if gameObject has drawable object component 

            // TODO -> might need to check if gameobject has multiple sprites(should not be the case thought)
            gameObject.GetComponent<Sprite>()?.Draw(spriteBatch);
            gameObject.GetComponent<SpriteTextComponent>()?.Draw(spriteBatch);
        }

        // end sprite batch
        spriteBatch.End();

        base.Draw(gameTime);
    }
}
