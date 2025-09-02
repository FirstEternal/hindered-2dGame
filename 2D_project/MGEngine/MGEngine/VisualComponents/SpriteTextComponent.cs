using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class SpriteTextComponent : ObjectComponent, IVisualComponent, IResizableVisualComponent
{
    // ALIGMNMENT WORK IN PROGRESS
    public Color textColor;
    public BitmapFont_equalHeight_dynamicWidth font;
    public BitmapFont_equalHeight_dynamicWidth.FontStyle fontStyle;

    public string text = "";
    public float fontSize;
    public int spacingX = 10;
    public int spacingY = 10;
    public float width { get; set; }
    public float height { get; set; }

    public bool cutWordOnly = false;

    public BitmapFont_equalHeight_dynamicWidth.CenterX textCenterX = BitmapFont_equalHeight_dynamicWidth.CenterX.Left;
    public BitmapFont_equalHeight_dynamicWidth.CenterY textCenterY = BitmapFont_equalHeight_dynamicWidth.CenterY.Middle;

    public IResizableVisualComponent.ResizeType resizeType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    GraphicsDevice graphicsDevice; // remove later
    public bool showDebug;

    private Dictionary<int, Vector2> charLocalPositions = new Dictionary<int, Vector2>();

    public SpriteTextComponent(
        int width,
        int height,
        BitmapFont_equalHeight_dynamicWidth font,
        string text,
        BitmapFont_equalHeight_dynamicWidth.FontStyle fontStyle = BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
        BitmapFont_equalHeight_dynamicWidth.CenterX textCenterX = BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
        BitmapFont_equalHeight_dynamicWidth.CenterY textCenterY = BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
        int spacingX = 10, int spacingY = 10, int fontSize = 16, Color? color = null,
        GraphicsDevice? graphicsDevice = null)
    {
        this.graphicsDevice = graphicsDevice;
        this.textColor = color ?? Color.Black;
        this.font = font;
        this.fontStyle = fontStyle;
        this.text = text;
        this.spacingX = spacingX;
        this.spacingY = spacingY;
        this.fontSize = fontSize;
        this.width = width;
        this.height = height;

        this.textCenterX = textCenterX;
        this.textCenterY = textCenterY;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (gameObject?.transform is null) return;

        //font.Draw(spriteBatch, text, gameObject.transform.globalPosition, charLocalPositions, fontScale * gameObject.transform.globalScale, textColor);
        int drawSpaceWidth = (int)(width - 10);
        int drawSpaceHeight = (int)(height - 10);

        Rectangle drawSpace = new Rectangle(
                width: drawSpaceWidth,
                height: drawSpaceHeight,
                x: (int)(gameObject.transform.globalPosition.X - drawSpaceWidth / 2),
                y: (int)(gameObject.transform.globalPosition.Y - drawSpaceHeight / 2)
        );

        //Draw rectangle (outline) using a 1x1 white pixel texture

        if (graphicsDevice != null && showDebug)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // Top
            spriteBatch.Draw(pixel, new Rectangle(drawSpace.X, drawSpace.Y, drawSpace.Width, 1), Color.Red);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(drawSpace.X, drawSpace.Y, 1, drawSpace.Height), Color.Red);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(drawSpace.Right - 1, drawSpace.Y, 1, drawSpace.Height), Color.Red);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(drawSpace.X, drawSpace.Bottom - 1, drawSpace.Width, 1), Color.Red);
        }

        font.Draw(
            spriteBatch: spriteBatch,
            drawSpace: drawSpace,
            centerX: textCenterX,
            centerY: textCenterY,
            fontStyle: fontStyle,
            fontSize: fontSize,
            spacingX: spacingX,
            spacingY: spacingY,
            rotation: gameObject.transform.globalRotationAngle,
            text: text,
            defaultColor: textColor,
            objectScale: gameObject.transform.globalScale,
            cutWordOnly: cutWordOnly
        );
    }

    public (float width, float height) MeasureText()
    {
        int drawSpaceWidth = (int)(width - 10);
        int drawSpaceHeight = (int)(height - 10);

        Rectangle drawSpace = new Rectangle(
            width: drawSpaceWidth,
            height: drawSpaceHeight,
            x: (int)(gameObject.transform.globalPosition.X - drawSpaceWidth / 2),
            y: (int)(gameObject.transform.globalPosition.Y - drawSpaceHeight / 2)
        );

        return font.MeasureText(
            drawSpace,
            fontStyle,
            fontSize,
            spacingX,
            spacingY,
            text,
            objectScale: gameObject.transform.globalScale,
            cutWordOnly: cutWordOnly
        );
    }


    public Vector2 ResizedScale()
    {
        throw new System.NotImplementedException();
    }
}
