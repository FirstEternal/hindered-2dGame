using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera : ObjectComponent
{
    /// <summary>
    /// if Game object is not given:
    /// creates gameobject with default id = 0
    /// creates transform with spawn position (0, 0)
    /// attaches it's self as component under game object
    /// </summary>
    /// 

    public const float BASE_ZOOM = 1f;
    public Camera(float zoom = BASE_ZOOM, GameObject? gameObject = null)
    {
        this._zoom = zoom;

        // add to game object as component
        if (gameObject != null)
        {
            gameObject.AddComponent(this);
            return;
        }

        // creates new game object
        GameObject newGameObject = new GameObject(0);
        newGameObject.CreateTransform();
        newGameObject.AddComponent(this);
    }

    public Matrix GetTransformation(GraphicsDevice graphicsDevice)
    {
        // Get screen center for smooth centering
        Vector3 screenCenter = new Vector3(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2, 0);

        if (gameObject?.transform is null) return Matrix.Identity;

        // Return transformation matrix
        return
            // transform translation
            Matrix.CreateTranslation(-gameObject.transform.globalPosition.X, -gameObject.transform.globalPosition.Y, 0) *
            // transform rotation
            Matrix.CreateRotationZ(gameObject.transform.globalRotationAngle) *
            // transform scale
            Matrix.CreateScale(_zoom, _zoom, 1) * (GameWindow.Instance?.gameScaleMatrix ?? Matrix.CreateScale(1, 1, 1)) *
            // screen center translation
            Matrix.CreateTranslation(screenCenter);
    }

    // Set position to follow transform
    public void FollowSmooth(Transform transform, float lerpFactor)
    {
        if (gameObject?.transform is null) return;

        this.gameObject.transform.globalPosition = Vector2.Lerp(
            this.gameObject.transform.globalPosition,
            transform.globalPosition,
            lerpFactor
        );
    }

    // Set position to follow a target
    public void FollowSmooth(GameObject gameObject, float lerpFactor)
    {
        if (this.gameObject?.transform is null || gameObject?.transform is null) return;

        this.gameObject.transform.globalPosition = Vector2.Lerp(
            this.gameObject.transform.globalPosition,
            gameObject.transform.globalPosition,
            lerpFactor
        );
    }

    // Optionally add properties for zoom and rotation if needed

    private float _zoom;
    public float Zoom
    {
        get => _zoom;
        set => _zoom = MathHelper.Clamp(value, 0.01f, 10f); // limited zoom range
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition, GraphicsDevice graphicsDevice)
    {
        Matrix inverseTransform = Matrix.Invert(GetTransformation(graphicsDevice));
        return Vector2.Transform(screenPosition, inverseTransform);
    }

    public bool IsOffScreen(GraphicsDevice graphicsDevice, Vector2 position, int spriteWidth, int spriteHeight, Vector2 spriteScale)
    {
        // Get the camera's transformation matrix and invert it to work in screen space
        Matrix transform = GetTransformation(graphicsDevice);
        Matrix inverseTransform = Matrix.Invert(transform);

        // Convert the position to screen space
        Vector2 screenPosition = Vector2.Transform(position, transform);

        // Calculate the screen-space dimensions of the sprite
        float scaledWidth = spriteWidth * spriteScale.X * _zoom;
        float scaledHeight = spriteHeight * spriteScale.Y * _zoom;

        // Check if the sprite is off-screen
        if (screenPosition.X + scaledWidth < 0 ||    // Off the left side
            screenPosition.X > graphicsDevice.Viewport.Width || // Off the right side
            screenPosition.Y + scaledHeight < 0 ||   // Off the top side
            screenPosition.Y > graphicsDevice.Viewport.Height)  // Off the bottom side
        {
            return true;
        }

        return false;
    }

    private float finalZoom;
    private float currAnimationTimer;
    private float durationInSeconds;
    private float zoomStep;

    private bool isZooming;

    public void ChangeZoomSmoothAnimation(float finalZoom, float durationInSeconds)
    {
        this.finalZoom = finalZoom;
        this.durationInSeconds = durationInSeconds;
        currAnimationTimer = durationInSeconds;

        float distance = MathF.Abs(finalZoom - Zoom);
        zoomStep = distance / durationInSeconds;

        isZooming = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (!isZooming)
            return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        currAnimationTimer -= deltaTime;

        float direction = MathF.Sign(finalZoom - Zoom);
        Zoom += zoomStep * deltaTime * direction;

        // Clamp to finalZoom if passed it or time is up
        if ((direction > 0 && Zoom >= finalZoom) ||
            (direction < 0 && Zoom <= finalZoom) ||
            currAnimationTimer <= 0f)
        {
            Zoom = finalZoom;
            isZooming = false;
        }
    }
}