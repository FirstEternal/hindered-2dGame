using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
namespace MGEngine.Physics;
public static class Movement
{
    public static void AssignVelocity(Vector2 finalPosition, PhysicsComponent physicsComponent, float speed)
    {
        if (physicsComponent?.gameObject?.transform is null) return;

        Vector2 direction = finalPosition - physicsComponent.gameObject.transform.globalPosition;
        if (direction != Vector2.Zero) direction.Normalize();

        physicsComponent.Velocity = direction * speed;
    }

    public static bool IsInPosition(Vector2 finalPosition, Transform transform, float tolerationDistance)
    {
        float distance = Vector2.Distance(transform.globalPosition, finalPosition);
        return distance <= tolerationDistance;
    }

    /*
    /// <summary>
    /// Before using set up arcMovementDegreeAngle 
    /// </summary>
    /// 
    public static void MoveInArc(GameTime gameTime, float moveSpeed, Vector2 centerPosition, float radius, bool isReversedDirection, Entity entity)
    {
        // TODO: MAKE IT SO THAT IT WORKS WITH ANY TRANSFORM FOR GAME ENGINE !!

        // Update the angle based on time and speed
        // speed is quite dependant on the radius size -> there fore adjusted arc movement slightly
        float angularVelocity = 50 * moveSpeed / radius;

        entity.arcMovementDegreeAngle -= angularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (entity.arcMovementDegreeAngle <= 0)
        {
            // x amount of degrees have been made -> stop moving
            entity.arcMovementDegreeAngle = 0;
            entity.moveSpeed = 0;
        }

        // check if movement in reverse direction
        int directionMultiplier = isReversedDirection ? -1 : 1;
        // Convert degrees to radians because Math.Cos and Math.Sin use radians
        float angleInRadians = directionMultiplier * MathF.PI / 180 * entity.arcMovementDegreeAngle;


        // Calculate the direction vector using the radius
        Vector2 directionVector = new Vector2(-MathF.Sin(angleInRadians), MathF.Cos(angleInRadians));

        // Update the object's depending on the center of the arc
        entity.gameObject.transform.globalPosition = centerPosition + radius * directionVector;
    }

    /// <summary>
    /// Before using set up arcMovementDegreeAngle 
    /// </summary>
    public static void MoveInArcUntilPosition(GameTime gameTime, float moveSpeed, Vector2 centerPosition, Vector2 finalPosition, float radius, bool isReversedDirection, Entity entity)
    {
        // TODO: MAKE IT SO THAT IT WORKS WITH ANY TRANSFORM FOR GAME ENGINE !!

        // check if at position x
        // if total distance from curr position and final position is less than 10 assume it is at position 
        if (IsInPosition(finalPosition, entity.gameObject.transform, 30))
        {
            entity.gameObject.transform.globalPosition = finalPosition;
            entity.moveSpeed = 0;
        }
        else
        {
            MoveInArc(gameTime, moveSpeed, centerPosition, radius, isReversedDirection, entity);
        }
    }

    public static void MoveInLine(GameTime gameTime, Vector2 translationVector, float moveSpeed, Transform transform)
    {
        if (moveSpeed == 0 || translationVector == Vector2.Zero || transform?.gameObject is null) return;

        transform.gameObject.GetComponent<PhysicsComponent>().Velocity = translationVector;

        //Vector2 translation = moveSpeed * translationVector * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //if (translation != Vector2.Zero) transform.globalPosition += translation;
    }

    public static void MoveInLineUntilPosition(GameTime gameTime, Vector2 translationVector, float moveSpeed, Vector2 finalPosition, Entity entity)
    {
        // TODO: MAKE IT SO THAT IT WORKS WITH ANY TRANSFORM FOR GAME ENGINE !!

        Transform transform = entity.gameObject.transform; // TODO: MAKE IT SO THAT IT WORKS WITH ANY TRANSFORM FOR GAME ENGINE !!
                                                           // check if at position x
                                                           // if total distance from curr position and final position is less than 10 assume it is at position 
        if (IsInPosition(finalPosition, transform, 30))
        {
            transform.globalPosition = finalPosition;
            entity.moveSpeed = 0;
        }
        else
        {
            MoveInLine(gameTime, translationVector, moveSpeed, entity.gameObject.transform);
        }
    }*/
}

