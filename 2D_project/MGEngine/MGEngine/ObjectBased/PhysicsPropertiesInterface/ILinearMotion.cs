using Microsoft.Xna.Framework;

public interface ILinearMotion
{
    public Vector2 Velocity { get; set; }
    public Vector2 cumulatedVelocity { get; set; }
    public Vector2 Acceleration { get; set; }

    public bool isGravity { get; set; }
    public bool isMovable { get; set; }
}
