using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class RectangleCollider(float x, float y, Vector2 scale, int width, int height, bool isAftermath, float rotation, bool isRelaxPosition = true, bool isEnergyExchange = false) : Collider(isAftermath, isRelaxPosition, isEnergyExchange)
{
    /*
    public RectangleCollider(int x, int y, int width, int height, bool isAftermath) : base(isAftermath)
    {
        rotatedRectangle { get; private set; } = RotatedRectangle(x, y, width, height);
    }*/
    public RotatedRectangle rotatedRectangle { get; private set; } = new RotatedRectangle(new Vector2(x, y), scale, width, height, rotation);
    //Rectangle rectangle = new Rectangle(x,y,height,width);
    //RotatedRectangle rectangle = new RotatedRectangle(x,y,height,width, Transform);
    // TODO

    // todo calculate hybrid(pixel perfect, rectangle) collision
    // todo calculate (rectangle, rectangle) // can just use rectangle.intersect probably

    /*
    public void DrawPixels()
    {
        rotatedRectangle = new RotatedRectangle(gameObject.transform.globalPosition, gameObject.transform.globalScale, width, height, gameObject.transform.globalRotationAngle);
        Texture2D pixelTexture = new Texture2D(Game2DPlatformer.Instance.GraphicsDevice, 1, 1);

        rotatedRectangle.DrawOutlinePixels(SceneManager.Instance.activeRenderer.spriteBatch, pixelTexture, Color.White);
        return;
    }*/
}
