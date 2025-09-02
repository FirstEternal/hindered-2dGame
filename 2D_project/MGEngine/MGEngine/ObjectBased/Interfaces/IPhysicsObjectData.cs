
using Microsoft.Xna.Framework;

public interface IPhysicsObjectData
{
    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }

    // Linear motion
    public Vector2 Velocity { get; set; }

    // Angular motion
    public float AngularVelocity { get; set; }
    //public float Rotation { get; set; }
}
