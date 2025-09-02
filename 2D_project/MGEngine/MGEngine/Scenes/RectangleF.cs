using Microsoft.Xna.Framework;

public struct RectangleF
{
    public float X, Y, Width, Height;

    public float Left => X;
    public float Right => X + Width;
    public float Top => Y;
    public float Bottom => Y + Height;

    public RectangleF(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Contains(Vector2 point)
    {
        return point.X >= Left && point.X <= Right &&
               point.Y >= Top && point.Y <= Bottom;
    }

    public bool Intersects(RectangleF other)
    {
        return !(Right < other.Left || Left > other.Right || Bottom < other.Top || Top > other.Bottom);
    }
}
