using Microsoft.Xna.Framework;

public class PhysicsComponent(float mass, Vector2? velocity = null, Vector2? acceleration = null,
    bool isGravity = true, bool isMovable = true,
    float angularVelocity = 0f, float angularAcceleration = 0f
    ) : ObjectComponent
{
    public float Mass { get; set; } = mass;

    // Linear motion
    public Vector2 Velocity = velocity ?? Vector2.Zero;
    /*
    public Vector2 _Velocity = velocity ?? Vector2.Zero;
    public Vector2 Velocity
    {
        set
        {
            _Velocity = value;
            if(value == Vector2.Zero) cumulatedVelocity = Vector2.Zero; // also reset cumulative velocity
        }
        get{
            return _Velocity;
        }
    } */


    public Vector2 cumulatedVelocity = velocity ?? Vector2.Zero; // active velocity cumulated through time with acceleration

    public Vector2 Acceleration = acceleration ?? Vector2.Zero;

    public bool isGravity = isGravity;
    public bool isMovable = isMovable;

    // Angular motion
    //public float AngularVelocity = angularVelocity;

    public float _AngularVelocity = angularVelocity;
    public float AngularVelocity
    {
        set
        {
            _AngularVelocity = value;
            if (value == 0) cumulatedAngularVelocity = 0; // also reset cumulative velocity
        }
        get
        {
            return _AngularVelocity;
        }
    }
    public float cumulatedAngularVelocity = angularVelocity; // Active angular velocity cumulated through time

    public float AngularAcceleration = angularAcceleration;
    public float Rotation;
}
