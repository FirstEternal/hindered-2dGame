using Microsoft.Xna.Framework;
public class Physics
{
    // Constants
    //public static readonly float earthGravity = -9.81f;      // Gravity constant (m/s²)
    //public static float gameGravity = earthGravity;      // Change as you wish
    //private const float TimeStep = 1.0f / 60.0f; // Time step for 60 FPS (seconds)

    public static readonly Vector2 earthGravityVector = new Vector2(0, -9.81f);
    public static Vector2 gameGravityVector = earthGravityVector;

    // change this part
    private const double GroundLevel = 0;    // Ground level position

    public static Vector2 LinearCumulationSpeedLimit = new Vector2(5000, 5000);
    public static float AngularCumulationSpeedLimit = 5000;
    public static void UpdatePhysics(PhysicsComponent physicsComponent, GameTime gameTime)
    {
        if (physicsComponent == null) return;

        if (!physicsComponent.isMovable || physicsComponent.gameObject?.transform is null) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // --- LINEAR MOTION ---
        LinearMotion(physicsComponent, deltaTime);

        // --- ANGULAR MOTION ---
        AngularMotion(physicsComponent, deltaTime);

        // --- OTHER MOTION ---
        ApplyOtherMotions(physicsComponent, deltaTime);

        physicsComponent.gameObject.transform.UpdateTransform();
    }

    private static void ApplyOtherMotions(PhysicsComponent physicsComponent, float deltaTime)
    {
        // --- CIRCULAR MOTION ---
        CircularMotion(
            physicsComponent.gameObject.GetComponent<CircularMovement>(),
            physicsComponent,
            deltaTime);
    }

    // --- LINEAR MOTION ---
    public static void LinearMotion(PhysicsComponent physicsComponent, float deltaTime)
    {
        Vector2 acceleration = physicsComponent.Acceleration - (!physicsComponent.isGravity ? Vector2.Zero : gameGravityVector);
        physicsComponent.cumulatedVelocity += acceleration * deltaTime;

        if (physicsComponent.cumulatedVelocity.X > LinearCumulationSpeedLimit.X) physicsComponent.cumulatedVelocity.X = LinearCumulationSpeedLimit.X;
        if (physicsComponent.cumulatedVelocity.Y > LinearCumulationSpeedLimit.Y) physicsComponent.cumulatedVelocity.Y = LinearCumulationSpeedLimit.Y;

        physicsComponent.gameObject.transform.globalPosition += deltaTime * (physicsComponent.Velocity + physicsComponent.cumulatedVelocity);
    }

    // --- ANGULAR MOTION ---
    public static void AngularMotion(PhysicsComponent physicsComponent, float deltaTime)
    {
        if (physicsComponent.AngularVelocity == 0) return;
        physicsComponent.cumulatedAngularVelocity += physicsComponent.AngularAcceleration * deltaTime;

        if (physicsComponent.cumulatedAngularVelocity > AngularCumulationSpeedLimit) physicsComponent.cumulatedAngularVelocity = AngularCumulationSpeedLimit;

        physicsComponent.Rotation += deltaTime * (physicsComponent.AngularVelocity + physicsComponent.cumulatedAngularVelocity);
        // Apply rotation to the game object if it supports rotation
        if (physicsComponent.Rotation != 0)
        {
            physicsComponent.gameObject.transform.localRotationAngle = physicsComponent.Rotation;
        }
    }

    // --- CIRCULAR MOTION --- (Angular Motion around a circle)
    public static void CircularMotion(CircularMovement circularMovement, PhysicsComponent physicsComponent, float deltaTime)
    {
        if (circularMovement is null) return;

        if (circularMovement.CenterPoint != null && circularMovement.Radius != null)
        {
            physicsComponent.Rotation += physicsComponent.AngularVelocity * deltaTime;

            float radius = circularMovement.Radius.Value;
            Vector2 center = circularMovement.CenterPoint.Value;

            // Compute new position due to circular motion
            Vector2 circularPosition = center + new Vector2(
                MathF.Cos(physicsComponent.Rotation),
                MathF.Sin(physicsComponent.Rotation)
            ) * radius;

            // Compute velocity contribution from circular motion
            Vector2 circularVelocity = new Vector2(
                -MathF.Sin(physicsComponent.Rotation),
                 MathF.Cos(physicsComponent.Rotation)
            ) * (physicsComponent.AngularVelocity * radius);

            // circular motion
            physicsComponent.gameObject.transform.globalPosition = circularPosition;
        }
    }
}

