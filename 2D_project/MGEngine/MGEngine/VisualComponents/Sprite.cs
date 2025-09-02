using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Sprite : ObjectComponent, IVisualComponent
{
    public SpriteEffects spriteEffects = SpriteEffects.None;
    public Texture2D? texture2D;
    public Rectangle sourceRectangle = Rectangle.Empty;
    public Vector2 origin = Vector2.Zero;
    public Color colorTint;
    public float layerDepth;

    public bool enabled = true;

    public Sprite(Texture2D texture2D, Color colorTint)
    {
        LoadTexture(texture2D, colorTint);
    }
    protected void LoadTexture(Texture2D texture2D, Color colorTint)
    {
        this.texture2D = texture2D;
        this.colorTint = colorTint;

        origin = texture2D is null ? Vector2.Zero : new Vector2(texture2D.Width / 2f, texture2D.Height / 2); // default at center
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!enabled) return;

        Transform? transform = gameObject?.transform;
        if (transform is null) return;

        Vector2 adjustedOrigin = origin;

        // Compensate for flip effects
        if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            adjustedOrigin.X = sourceRectangle.Width - origin.X;

        if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
            adjustedOrigin.Y = sourceRectangle.Height - origin.Y;

        // game scale stil work in progress
        spriteBatch.Draw(
           texture2D,              // Sprite texture
           transform.globalPosition,     // Position on the screen
           sourceRectangle != Rectangle.Empty ? sourceRectangle : null,       // Rectangle to define the frame from the sprite sheet
           colorTint,              // tint
           transform.globalRotationAngle,     // rotation
           adjustedOrigin,                 // origin of rotation Origin, with accounting for offset of pivot
           GetScale(), //* GameWindow.Instance.gameScale  <- work in progress for game scaling,        // Scale
           spriteEffects,          // flip effects
           layerDepth                      // Layer depth (render order)
        );
    }

    protected virtual Vector2 GetScale()
    {
        return gameObject?.transform?.globalScale ?? Vector2.One;
    }
}
