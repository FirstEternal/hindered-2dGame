using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class MouseGameObject : GameObject
{
    public static MouseGameObject? Singleton;

    private GraphicsDevice? graphicsDevice;
    private ParticleCollider? mouseCollider;
    public MouseState MouseState;
    public MouseGameObject(GraphicsDevice graphicsDevice, int id = int.MinValue) : base(id)
    {
        if (Singleton is not null) return;
        if (graphicsDevice is null) throw new ArgumentNullException(nameof(graphicsDevice));

        mouseCollider = new ParticleCollider(radius: 2, isAftermath: false, isRelaxPosition: false);
        if (mouseCollider is null) throw new ArgumentNullException(nameof(mouseCollider) + "creation failed.");
        AddComponent(mouseCollider);
        CreateTransform();

        this.graphicsDevice = graphicsDevice;
        Singleton = this;
    }

    new public void Update(GameTime gameTime)
    {
        MouseState = Mouse.GetState();
        transform.globalPosition = GetMouseScreenPosition();
        base.Update(gameTime);
    }

    public Vector2 GetMouseScreenPosition()
    {
        return MouseState.Position.ToVector2() / GameWindow.Instance.gameScale;
    }

    public Vector2 GetMouseWorldSpacePosition()
    {
        if (graphicsDevice is null) throw new ArgumentNullException(nameof(graphicsDevice));
        return SceneManager.Instance.activeScene.mainCamera.ScreenToWorld(MouseState.Position.ToVector2(), graphicsDevice);
    }
}
