using Microsoft.Xna.Framework;

public class GameWorldSpaceMath
{
    public static Vector2 RotateAndScalePoint(Vector2 point, float angle, Vector2 scale)
    {
        // Step 1: Scale the point
        point.X *= scale.X;
        point.Y *= scale.Y;

        // Step 2: Rotate the scaled point
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);

        return new Vector2(
            point.X * cos - point.Y * sin,
            point.X * sin + point.Y * cos
        );
    }
}

