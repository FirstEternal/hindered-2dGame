using Microsoft.Xna.Framework;

public class CircularMovement(Vector2? CenterPoint, float Radius) : ObjectComponent
{
    public Vector2? CenterPoint { get; set; } = CenterPoint ?? Vector2.Zero;
    public float? Radius { get; set; } = Radius;
}