using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class PrimitiveBatch
{
    private BlendState? _blendState;
    private DepthStencilState? _depthStencilState;
    private RasterizerState? _rasterizerState;
    private Effect? _effect;
    private BasicEffect _basicEffect;
    private bool _beginCalled;
    private List<VertexPositionColor> _vertexArray = new List<VertexPositionColor>(256);
    private readonly GraphicsDevice _graphicsDevice;

    public PrimitiveBatch(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        _basicEffect = new BasicEffect(graphicsDevice);
        _basicEffect.VertexColorEnabled = true;
        _basicEffect.TextureEnabled = false;

        SetProjection();
        graphicsDevice.DeviceReset += SetProjection;
    }

    public void SetProjection(object? o = null, EventArgs? args = null)
    {
        _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(-0.5f, _graphicsDevice.Viewport.Width - 0.5f, _graphicsDevice.Viewport.Height - 0.5f, -0.5f, 0, -1);
    }

    public void Begin(BlendState? theBlendState = null, DepthStencilState? theDepthStencilState = null, RasterizerState? theRasterizerState = null, Effect? theEffect = null, Matrix? theTransformMatrix = null)
    {
        theBlendState ??= BlendState.AlphaBlend;
        theDepthStencilState ??= DepthStencilState.None;
        theRasterizerState ??= RasterizerState.CullCounterClockwise;
        theEffect ??= _basicEffect;
        // Matrix theTransformMatrix = theTransformMatrix0 ?? Matrix.Identity;

        _blendState = theBlendState;
        _depthStencilState = theDepthStencilState;
        _rasterizerState = theRasterizerState;
        _effect = theEffect;

        if (theTransformMatrix is not null && _effect is BasicEffect effect1)
        {
            effect1.World = theTransformMatrix.Value;
        }

        _beginCalled = true;
    }

    public void DrawPoint(Vector2 position, Color color, float layerDepth = 0f)
    {
        DrawLine(new Vector2(position.X - 0.5f, position.Y - 0.5f), new Vector2(position.X + 0.5f, position.Y + 0.5f), color, layerDepth);
    }

    public void DrawLine(Vector2 start, Vector2 end, Color color, float layerDepth = 0f)
    {
        VertexPositionColor vertex = new VertexPositionColor(new Vector3(start, layerDepth), color);
        _vertexArray.Add(vertex);
        vertex.Position.X = end.X;
        vertex.Position.Y = end.Y;
        _vertexArray.Add(vertex);
    }

    public void DrawCircle(Vector2 center, float radius, int divisions, Color color, float layerDepth = 0)
    {
        Vector2 start = new Vector2(center.X + radius, center.Y);
        Vector2 end = Vector2.Zero;
        for (int i = 1; i <= divisions; i++)
        {
            float angle = i / (float)divisions * (float)Math.PI * 2;
            end.X = center.X + radius * (float)Math.Cos(angle);
            end.Y = center.Y + radius * (float)Math.Sin(angle);
            DrawLine(start, end, color, layerDepth);
            start = end;
        }

    }

    public void DrawRectangle(Vector2 center, float width, float height, Color color, float layerDepth = 0)
    {
        VertexPositionColor vertex = new VertexPositionColor(
            new Vector3(center.X - width / 2, center.Y - height / 2, layerDepth), color
            );
        _vertexArray.Add(vertex);
        vertex.Position.X += width;
        _vertexArray.Add(vertex);
        _vertexArray.Add(vertex);
        vertex.Position.Y += height;
        _vertexArray.Add(vertex);
        _vertexArray.Add(vertex);
        vertex.Position.X -= width;
        _vertexArray.Add(vertex);
        _vertexArray.Add(vertex);
        vertex.Position.Y -= height;
        _vertexArray.Add(vertex);
    }
    public void DrawRotatedRectangle(Vector2 center, float width, float height, float rotation, Color color, float layerDepth = 0f)
    {
        // Half dimensions
        float halfWidth = width / 2;
        float halfHeight = height / 2;

        // Compute the four corners relative to center before rotation
        Vector2 topLeft = new Vector2(-halfWidth, -halfHeight);
        Vector2 topRight = new Vector2(halfWidth, -halfHeight);
        Vector2 bottomRight = new Vector2(halfWidth, halfHeight);
        Vector2 bottomLeft = new Vector2(-halfWidth, halfHeight);

        // Apply rotation matrix
        Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
        topLeft = Vector2.Transform(topLeft, rotationMatrix) + center;
        topRight = Vector2.Transform(topRight, rotationMatrix) + center;
        bottomRight = Vector2.Transform(bottomRight, rotationMatrix) + center;
        bottomLeft = Vector2.Transform(bottomLeft, rotationMatrix) + center;

        // Draw the four edges
        DrawLine(topLeft, topRight, color, layerDepth);
        DrawLine(topRight, bottomRight, color, layerDepth);
        DrawLine(bottomRight, bottomLeft, color, layerDepth);
        DrawLine(bottomLeft, topLeft, color, layerDepth);
    }

    public void End()
    {
        if (!_beginCalled)
        {
            throw new Exception("InvalidOperationException End was called before begin.");
        }

        Apply();
        Draw();
        _beginCalled = false;
    }

    public void Apply()
    {
        _graphicsDevice.BlendState = _blendState;
        _graphicsDevice.DepthStencilState = _depthStencilState;
        _graphicsDevice.RasterizerState = _rasterizerState;
        _effect?.CurrentTechnique.Passes[0].Apply();
    }

    public void Draw()
    {
        int lineCount = _vertexArray.Count() / 2;
        if (lineCount < 1)
        {
            return;
        }

        _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _vertexArray.ToArray(), 0, lineCount);
        _vertexArray.Clear();
    }
}