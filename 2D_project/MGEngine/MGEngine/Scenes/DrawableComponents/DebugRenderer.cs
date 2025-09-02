using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class DebugRenderer : Renderer//(Game game) : Renderer(game)
{
    public PrimitiveBatch primitiveBatch;
    protected Color itemColor;
    protected Color movementColor;
    protected Color colliderColor;
    protected Color gameObjectIdColor;
    protected Color parentGameObjectIdColor;
    protected BlendState? blendState;
    protected DepthStencilState? depthStencilState;
    protected RasterizerState? rasterizerState;
    protected Effect? effect;
    protected Matrix transformMatrix;
    protected SpriteFont? font;

    public bool shouldShowGameObjectIDs = false;
    public bool shouldShowGParentameObjectIDs = false;

    public DebugRenderer(Game Game) : base(Game)
    {
        ItemColor = Color.OrangeRed;
        MovementColor = Color.SkyBlue;
        ColliderColor = Color.Lime;
        gameObjectIdColor = Color.Red;
        parentGameObjectIdColor = Color.DarkRed;


        transformMatrix = Matrix.Identity;

        primitiveBatch = new PrimitiveBatch(GraphicsDevice);
    }

    public void LoadSpriteFont(SpriteFont spriteFont)
    {
        font = spriteFont;
    }

    public Color ItemColor
    {
        get => itemColor;
        set => itemColor = value;
    }

    public Color MovementColor
    {
        get => movementColor;
        set => movementColor = value;
    }

    public Color ColliderColor
    {
        get => colliderColor;
        set => colliderColor = value;
    }

    public BlendState? BlendState
    {
        get => blendState;
        set => blendState = value;
    }

    public DepthStencilState? DepthStencilState
    {
        get => depthStencilState;
        set => depthStencilState = value;
    }

    public RasterizerState? RasterizerState
    {
        get => rasterizerState;
        set => rasterizerState = value;
    }

    public Effect? Effect
    {
        get => effect;
        set => effect = value;
    }

    public Matrix TransformMatrix
    {
        get => transformMatrix;
        set => transformMatrix = value;
    }
    /*
    protected override void LoadContent()
    {
        primitiveBatch = new PrimitiveBatch(GraphicsDevice);
    }
    */

    private void DrawObjects(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (!gameObject.isActive) continue;

            Transform? transform = gameObject.transform;
            if (transform is null) continue;

            primitiveBatch.DrawPoint(transform.globalPosition, itemColor);

            PhysicsComponent? physicsComponent = gameObject.GetComponent<PhysicsComponent>();
            if (physicsComponent is not null) // itemWithVelocity is not null
            {
                primitiveBatch.DrawLine(transform.globalPosition, (transform.globalPosition + physicsComponent.Velocity), movementColor);
            }

            Collider? collider = gameObject.GetComponent<Collider>();
            if (collider is null) continue;

            if (collider is ParticleCollider particleCollider)
            {
                primitiveBatch.DrawCircle(transform.globalPosition, particleCollider.radius, 32, colliderColor);
            }

            if (collider is AARectangleCollider aaRectangleCollider)
            {
                if (collider is OBBRectangleCollider obbRectangleCollider)
                {
                    primitiveBatch.DrawRotatedRectangle(transform.globalPosition, obbRectangleCollider.Width, obbRectangleCollider.Height, transform.globalRotationAngle, colliderColor);
                }
                else
                {
                    primitiveBatch.DrawRectangle(transform.globalPosition, aaRectangleCollider.Width, aaRectangleCollider.Height, colliderColor);

                }
            }

            if (collider is ConvexCollider)
            {
                ConvexCollider convexCollider = (ConvexCollider)collider;
                Vector2 offset = transform.globalPosition;
                float angle = transform.globalRotationAngle;
                Vector2 scale = collider.gameObject.transform.globalScale; // Scaling
                Matrix transformMatrix = Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationZ(angle) * (Matrix.CreateTranslation(offset.X, offset.Y, 0));
                List<Vector2> vertices = convexCollider.Bounds.Vertices;

                for (int i = 0; i < vertices.Count; i++)
                {
                    int j = (i + 1) % vertices.Count;
                    Vector2 start = Vector2.Transform(vertices[i], transformMatrix);
                    Vector2 end = Vector2.Transform(vertices[j], transformMatrix);
                    primitiveBatch.DrawLine(start, end, ColliderColor);
                }
            }
        }
    }
    public override void Draw(GameTime gameTime)
    {
        if (!Enabled || SceneManager.Instance.activeScene is null) return;

        // if renderer is not enabled, clear screen before drawing
        //if(SceneManager.Instance.activeRenderer is not null && !SceneManager.Instance.activeRenderer.Enabled) GraphicsDevice.Clear(clearColor);

        Matrix cameraMatrix = SceneManager.Instance.activeScene.mainCamera.GetTransformation(GraphicsDevice);
        Matrix scaleMatrix = GameWindow.Instance?.gameScaleMatrix ?? Matrix.CreateScale(1, 1, 1);
        Matrix transformInverse = Matrix.Invert(cameraMatrix);
        Vector2 topLeft = Vector2.Transform(Vector2.Zero, transformInverse);
        Vector2 bottomRight = Vector2.Transform(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), transformInverse);

        // draw overlay gameObjects
        primitiveBatch.Begin(theTransformMatrix: scaleMatrix);
        DrawObjects(SceneManager.Instance.activeScene.overlayGameObjects);
        primitiveBatch.End();

        // draw world space gameObjects
        primitiveBatch.Begin(theTransformMatrix: cameraMatrix);
        DrawObjects(SceneManager.Instance.activeScene.gameObjects);
        primitiveBatch.DrawCircle(Vector2.Zero, 50, 32, Color.Orange);
        primitiveBatch.DrawLine(new Vector2(-100000, 500), new Vector2(100000, 500), color: Color.Red);
        primitiveBatch.End();

        // draw gameObject id's
        if (font is not null)
        {
            // overlay
            spriteBatch.Begin(transformMatrix: scaleMatrix);

            // -> Draw fps
            Vector2 fpsPosition = new Vector2(GameWindow.Instance.GetWindowSize().X - 100, 50);
            Vector2 frameTimePosition = new Vector2(GameWindow.Instance.GetWindowSize().X - 100, 75);
            Vector2 frameCountPosition = new Vector2(GameWindow.Instance.GetWindowSize().X - 100, 100);

            spriteBatch.DrawString(font, $"FPS: {FPSCounter.Instance.fps.ToString()}", fpsPosition, Color.Black);
            spriteBatch.DrawString(font, $"frameTime: {FPSCounter.Instance.frameTime.ToString("F2")}", frameTimePosition, Color.Black);
            spriteBatch.DrawString(font, $"frameCount: {FPSCounter.Instance.frameCount.ToString("F2")}", frameCountPosition, Color.Black);

            if (shouldShowGameObjectIDs || shouldShowGParentameObjectIDs)
            {
                // -> overlay objects
                foreach (GameObject gameObject in SceneManager.Instance.activeScene.overlayGameObjects)
                {
                    if (!gameObject.isActive) continue;
                    bool isParent = gameObject.parent is null;

                    // game Object is parent 
                    if (isParent && !shouldShowGParentameObjectIDs) continue;

                    // game Object is not parent, 
                    else if (!isParent && !shouldShowGameObjectIDs) continue;

                    DrawGameObjectId(gameObject, isParent);
                }
                spriteBatch.End();


                // world space
                spriteBatch.Begin(transformMatrix: cameraMatrix);

                foreach (GameObject gameObject in SceneManager.Instance.activeScene.gameObjects)
                {
                    if (!gameObject.isActive) continue;
                    bool isParent = gameObject.parent is null;

                    // game Object is parent 
                    if (isParent && !shouldShowGParentameObjectIDs) continue;

                    // game Object is not parent, 
                    else if (!isParent && !shouldShowGameObjectIDs) continue;

                    DrawGameObjectId(gameObject, isParent);
                }
            }

            spriteBatch.End();
        }
    }
    protected void DrawGameObjectId(GameObject gameObject, bool isParent)
    {
        if (gameObject.transform is null || font is null) return;
        // parameters
        string id = gameObject.id.ToString();
        Vector2 fontSize = font.MeasureString(id);
        Color fontColor = isParent ? parentGameObjectIdColor : gameObjectIdColor;
        float x = gameObject.transform.globalPosition.X - fontSize.X / 2;
        float y = gameObject.transform.globalPosition.Y;// + (!isParent ? 0 : fontSize.Y);

        // show game object ids
        //spriteBatch.DrawString(font, id, new Vector2(x, y), itemColor);
        spriteBatch.DrawString(font, id, gameObject.transform.globalPosition, itemColor);
    }
}