using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

public class Panel : Sprite, IResizableVisualComponent
{
    public IResizableVisualComponent.ResizeType resizeType { get; set; }
    public float width { get; set; }
    public float height { get; set; }

    public Panel(int width, int height, Texture2D texture2D, Color colorTint) : base(texture2D, colorTint)
    {
        this.width = width;
        this.height = height;
    }

    protected override Vector2 GetScale()
    {
        return ResizedScale();
    }

    public Vector2 ResizedScale()
    {
        float scaleX = width / MathF.Max(sourceRectangle.Width, 0.01f);
        float scaleY = height / MathF.Max(sourceRectangle.Height, 0.01f);
        return new Vector2(gameObject.transform.globalScale.X * scaleX, gameObject.transform.globalScale.Y * scaleY);
    }

    private int nineSliceBorderSize;
    public void EnableNineSliceDraw(int borderSize)
    {
        if (borderSize <= 0 || texture2D == null)
        {
            Debug.WriteLine("Nine-slice parameters are invalid.");
        }

        nineSliceBorderSize = borderSize;
        resizeType = IResizableVisualComponent.ResizeType.Nine_Slice;
    }


    public override void Draw(SpriteBatch spriteBatch)
    {
        if (nineSliceBorderSize > 0 && resizeType == IResizableVisualComponent.ResizeType.Nine_Slice)
        {
            DrawNineSlice(spriteBatch);
            return;
        }

        base.Draw(spriteBatch);
    }

    private void DrawNineSlice(SpriteBatch spriteBatch)
    {
        if (texture2D == null || sourceRectangle.Width < 1 || sourceRectangle.Height < 1)
            return;

        var transform = gameObject?.transform;
        if (transform == null)
            return;

        Rectangle srcRect = sourceRectangle != Rectangle.Empty
            ? sourceRectangle
            : new Rectangle(0, 0, texture2D.Width, texture2D.Height);

        /*
        if (srcRect.Width < nineSliceBorderSize * 2 || srcRect.Height < nineSliceBorderSize * 2)
        {
            Debug.WriteLine("Source rectangle is too small for nine-slice.");
            return;
        }*/

        // Scale borders based on transform's global scale
        Vector2 scale = transform.globalScale;
        int borderX = (int)(nineSliceBorderSize * scale.X);
        int borderY = (int)(nineSliceBorderSize * scale.Y);

        Rectangle[] src = new Rectangle[9]
        {
        new Rectangle(srcRect.X, srcRect.Y, nineSliceBorderSize, nineSliceBorderSize),
        new Rectangle(srcRect.X + nineSliceBorderSize, srcRect.Y, srcRect.Width - 2 * nineSliceBorderSize, nineSliceBorderSize),
        new Rectangle(srcRect.Right - nineSliceBorderSize, srcRect.Y, nineSliceBorderSize, nineSliceBorderSize),

        new Rectangle(srcRect.X, srcRect.Y + nineSliceBorderSize, nineSliceBorderSize, srcRect.Height - 2 * nineSliceBorderSize),
        new Rectangle(srcRect.X + nineSliceBorderSize, srcRect.Y + nineSliceBorderSize, srcRect.Width - 2 * nineSliceBorderSize, srcRect.Height - 2 * nineSliceBorderSize),
        new Rectangle(srcRect.Right - nineSliceBorderSize, srcRect.Y + nineSliceBorderSize, nineSliceBorderSize, srcRect.Height - 2 * nineSliceBorderSize),

        new Rectangle(srcRect.X, srcRect.Bottom - nineSliceBorderSize, nineSliceBorderSize, nineSliceBorderSize),
        new Rectangle(srcRect.X + nineSliceBorderSize, srcRect.Bottom - nineSliceBorderSize, srcRect.Width - 2 * nineSliceBorderSize, nineSliceBorderSize),
        new Rectangle(srcRect.Right - nineSliceBorderSize, srcRect.Bottom - nineSliceBorderSize, nineSliceBorderSize, nineSliceBorderSize),
        };

        Rectangle destRect = new Rectangle(
            (int)(transform.globalPosition.X - width / 2f),
            (int)(transform.globalPosition.Y - height / 2f),
            (int)(width * scale.X),
            (int)(height * scale.Y)
        );

        Rectangle[] dst = new Rectangle[9]
        {
        new Rectangle(destRect.X, destRect.Y, borderX, borderY),
        new Rectangle(destRect.X + borderX, destRect.Y, destRect.Width - 2 * borderX, borderY),
        new Rectangle(destRect.Right - borderX, destRect.Y, borderX, borderY),

        new Rectangle(destRect.X, destRect.Y + borderY, borderX, destRect.Height - 2 * borderY),
        new Rectangle(destRect.X + borderX, destRect.Y + borderY, destRect.Width - 2 * borderX, destRect.Height - 2 * borderY),
        new Rectangle(destRect.Right - borderX, destRect.Y + borderY, borderX, destRect.Height - 2 * borderY),

        new Rectangle(destRect.X, destRect.Bottom - borderY, borderX, borderY),
        new Rectangle(destRect.X + borderX, destRect.Bottom - borderY, destRect.Width - 2 * borderX, borderY),
        new Rectangle(destRect.Right - borderX, destRect.Bottom - borderY, borderX, borderY),
        };

        for (int i = 0; i < 9; i++)
        {
            spriteBatch.Draw(
                texture2D,
                dst[i],
                src[i],
                colorTint,
                transform.globalRotationAngle,
                Vector2.Zero,
                spriteEffects,
                layerDepth
            );
        }
    }
}
