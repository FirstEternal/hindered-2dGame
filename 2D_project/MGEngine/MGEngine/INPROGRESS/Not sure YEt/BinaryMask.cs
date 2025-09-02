using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BinaryMask
{
    private GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;

    public Matrix transformMatrix { get; private set; }

    public Texture2D MaskTexture { get; private set; }
    public bool[,] MaskBools { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public BinaryMask(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        this.graphicsDevice = graphicsDevice;
        this.spriteBatch = spriteBatch;
    }

    public void GenerateMask(Texture2D texture, Color color, Vector2? translation = null, float rotationAngle = 0f, Vector2? scale = null)
    {
        scale ??= Vector2.One;
        scale ??= Vector2.Zero;
        Width = texture.Width;
        Height = texture.Height;

        if (rotationAngle != 0f || scale != Vector2.One || translation != Vector2.Zero)
        {
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, Width, Height);
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);

            transformMatrix = Matrix.CreateScale(scale.Value.X, scale.Value.Y, 1) *
                                          Matrix.CreateRotationZ(rotationAngle) *
                                          Matrix.CreateTranslation(translation.Value.X, translation.Value.Y, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, transformMatrix);
            spriteBatch.Draw(texture, new Vector2(translation.Value.X, translation.Value.Y), color);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
            MaskTexture = CreateMaskTexture(renderTarget);
            MaskBools = CreateBinaryMask(renderTarget);
            renderTarget.Dispose();
        }
        else
        {
            MaskTexture = CreateMaskTexture(texture);
        }
    }

    private Texture2D CreateMaskTexture(Texture2D texture)
    {
        Color[] pixelData = new Color[Width * Height];
        texture.GetData(pixelData);
        Color[] maskData = new Color[pixelData.Length];
        for (int i = 0; i < pixelData.Length; i++)
        {
            maskData[i] = pixelData[i].A > 0 ? Color.White : Color.Black;
        }
        Texture2D maskTexture = new Texture2D(graphicsDevice, Width, Height);
        maskTexture.SetData(maskData);
        return maskTexture;
    }

    bool[,] CreateBinaryMask(Texture2D texture)
    {
        int width = texture.Width;
        int height = texture.Height;

        // Retrieve the texture data (each pixel's color)
        Color[] textureData = new Color[width * height];
        texture.GetData(textureData);

        // Create the binary mask array
        bool[,] binaryMask = new bool[width, height];

        // Loop through each pixel
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;

                // Set binary mask pixel to true if alpha > 0, otherwise false
                binaryMask[x, y] = textureData[index].A > 0;
            }
        }

        return binaryMask;
    }

}

