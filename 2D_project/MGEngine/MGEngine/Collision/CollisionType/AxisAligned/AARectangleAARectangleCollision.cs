using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

/*
public class AARectangleAARectangleCollision
{
    public static bool AARectangle_AARectangleColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not AARectangleCollider aarectA || cB is not AARectangleCollider aarectB) return false;

        if (isRotatedRectangle(aarectA) || isRotatedRectangle(aarectB))
        {
            // OBB rect - OBB rect colllision
            return OBBRectangleOBBRectangleCollision.OBBRectangle_OBBRectangleColliderCollision(cA, cB);
            //return false;
        }
        else
        {
            // AA rect - AA rect colllision
            float horizontalDistance = System.Math.Abs(aarectA.gameObject.transform.globalPosition.X - aarectB.gameObject.transform.globalPosition.X);
            float verticalDistance = System.Math.Abs(aarectA.gameObject.transform.globalPosition.Y - aarectB.gameObject.transform.globalPosition.Y);
            if (horizontalDistance < aarectA.Width / 2 + aarectB.Width / 2 && verticalDistance < aarectA.Height / 2 + aarectB.Height / 2)
            {
                if (cA.isAftermath && cB.isAftermath) RelaxStep(aarectA, aarectB);
                return true;
            }
            return false;
        }
    }

    private static bool isRotatedRectangle(AARectangleCollider aa)
    {
        if (aa is not OBBRectangleCollider oBB) return false;

        Transform transform = oBB.gameObject.transform;
        float rotationAngle = transform.globalRotationAngle % (2 * MathF.PI); // Ensure angle stays within 0 - 2π radians

        if (rotationAngle == 0
            || MathF.Abs(rotationAngle - MathF.PI / 2) < 0.0001f
            || MathF.Abs(rotationAngle - MathF.PI) < 0.0001f
            || MathF.Abs(rotationAngle - (3 * MathF.PI / 2)) < 0.0001f)
        {
            return false;
        }

        return true;
    }

    private static bool isRotated90Or270(AARectangleCollider aa)
    {
        if (aa is not OBBRectangleCollider oBB) return false;

        Transform transform = oBB.gameObject.transform;
        float rotationAngle = transform.globalRotationAngle % (2 * MathF.PI); // Ensure angle stays within 0 - 2π radians

        // Check if the rotation is either 90° (π/2) or 270° (3π/2)
        return MathF.Abs(rotationAngle - MathF.PI / 2) < 0.0001f || MathF.Abs(rotationAngle - (3 * MathF.PI / 2)) < 0.0001f;
    }

    protected static void RelaxStep1(AARectangleCollider aaRectangle1, AARectangleCollider aaRectangle2)
    {
        if (!aaRectangle1.isRelaxPosition && !aaRectangle2.isRelaxPosition) return;

        // Get the actual rectangles (considering rotation)
        Rectangle rectangleA = GetRectangle(aaRectangle1);
        Rectangle rectangleB = GetRectangle(aaRectangle2);

        // Calculate the horizontal and vertical distances using the rectangles
        float horizontalDifference = rectangleA.X - rectangleB.X;
        float verticalDifference = rectangleA.Y - rectangleB.Y;

        // Calculate the collided distances and relaxation distances
        float horizontalCollidedDistance = System.Math.Abs(horizontalDifference);
        float verticalCollidedDistance = System.Math.Abs(verticalDifference);

        float horizontalMinimumDistance = rectangleA.Width / 2 + rectangleB.Width / 2;
        float verticalMinimumDistance = rectangleA.Height / 2 + rectangleB.Height / 2;

        // Relaxation distances
        float horizontalRelaxDistance = horizontalMinimumDistance - horizontalCollidedDistance;
        float verticalRelaxDistance = verticalMinimumDistance - verticalCollidedDistance;

        // Minimum threshold to avoid small artifacts
        const float minRelaxDistanceThreshold = 0.01f;  // Adjust this based on your needs
        horizontalRelaxDistance = MathF.Max(horizontalRelaxDistance, minRelaxDistanceThreshold);
        verticalRelaxDistance = MathF.Max(verticalRelaxDistance, minRelaxDistanceThreshold);

        // Determine the smaller relaxation distance and set the normal
        Vector2 collisionNormal;
        float relaxDistance;

        if (horizontalRelaxDistance < verticalRelaxDistance)
        {
            relaxDistance = horizontalRelaxDistance;
            collisionNormal = new Vector2(horizontalDifference < 0 ? 1 : -1, 0);
        }
        else
        {
            relaxDistance = verticalRelaxDistance;
            collisionNormal = new Vector2(0, verticalDifference < 0 ? 1 : -1);
        }

        // Apply the relaxation step
        Vector2 relaxDistanceVector = collisionNormal * relaxDistance;
        CollisionLogic.RelaxCollision(aaRectangle1.gameObject.GetComponent<PhysicsComponent>(), aaRectangle1.isRelaxPosition,
                                      aaRectangle2.gameObject.GetComponent<PhysicsComponent>(), aaRectangle2.isRelaxPosition,
                                      relaxDistanceVector);

        // Optionally handle energy exchange if needed
        //ExchangeEnergy(aaRectangle1, aaRectangle2, collisionNormal);
    }


    private static Rectangle GetRectangle(AARectangleCollider aaRectangle)
    {
        return isRotated90Or270(aaRectangle) ?
            new Rectangle(
                x: (int)aaRectangle.gameObject.transform.globalPosition.X,
                y: (int)aaRectangle.gameObject.transform.globalPosition.Y,
               width: (int)aaRectangle.Height,
               height: (int)aaRectangle.Width
            ) :
            new Rectangle(
                x: (int)aaRectangle.gameObject.transform.globalPosition.X,
                y: (int)aaRectangle.gameObject.transform.globalPosition.Y,
               width: (int)aaRectangle.Width,
               height: (int)aaRectangle.Height
            );
    }


    protected static void RelaxStep(AARectangleCollider aaRectangle1, AARectangleCollider aaRectangle2)
    {
        if (!aaRectangle1.isRelaxPosition && !aaRectangle1.isRelaxPosition) return;

        float horizontalDifference = aaRectangle1.gameObject.transform.globalPosition.X - aaRectangle2.gameObject.transform.globalPosition.X;
        float horizontalCollidedDistance = System.Math.Abs(horizontalDifference);
        float horizontalMinimumDistance = aaRectangle1.Width / 2 + aaRectangle2.Width / 2;
        float horizontalRelaxDistance = horizontalMinimumDistance - horizontalCollidedDistance;
        float verticalDifference = aaRectangle1.gameObject.transform.globalPosition.Y - aaRectangle2.gameObject.transform.globalPosition.Y;
        float verticalCollidedDistance = System.Math.Abs(verticalDifference);
        float verticalMinimumDistance = aaRectangle1.Height / 2 + aaRectangle2.Height / 2;
        float verticalRelaxDistance = verticalMinimumDistance - verticalCollidedDistance;
        Vector2 collisionNormal;
        float relaxDistance;
        if (horizontalRelaxDistance < verticalRelaxDistance)
        {
            relaxDistance = horizontalRelaxDistance;
            collisionNormal = new Vector2(horizontalDifference < 0 ? 1 : -1, 0);
        }
        else
        {
            relaxDistance = verticalRelaxDistance;
            collisionNormal = new Vector2(0, verticalDifference < 0 ? 1 : -1);
        }

        Vector2 relaxDistanceVector = collisionNormal * relaxDistance;
        CollisionLogic.RelaxCollision(aaRectangle1.gameObject.GetComponent<PhysicsComponent>(), aaRectangle1.isRelaxPosition,
                                      aaRectangle2.gameObject.GetComponent<PhysicsComponent>(), aaRectangle2.isRelaxPosition,
                                      relaxDistanceVector);
        //ExchangeEnergy(aaRectangle1, aaRectangle2, collisionNormal);
    }
}
*/


public class AARectangleAARectangleCollision
{
    public static bool AARectangle_AARectangleColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not AARectangleCollider aarectA || cB is not AARectangleCollider aarectB) return false;

        if (isRotatedRectangle(aarectA) || isRotatedRectangle(aarectB))
        {
            // OBB rect - OBB rect colllision
            return OBBRectangleOBBRectangleCollision.OBBRectangle_OBBRectangleColliderCollision(cA, cB);
            //return false;
        }
        else
        {

            // AA rect - AA rect colllision
            float horizontalDistance = Math.Abs(aarectA.gameObject.transform.globalPosition.X - aarectB.gameObject.transform.globalPosition.X);
            float verticalDistance = Math.Abs(aarectA.gameObject.transform.globalPosition.Y - aarectB.gameObject.transform.globalPosition.Y);

            FloatRect rA = GetFloatRectangle(aarectA);
            FloatRect rB = GetFloatRectangle(aarectB);

            if (horizontalDistance < rA.Width / 2 + rB.Width / 2 && verticalDistance < rA.Height / 2 + rB.Height / 2)
            {
                if (cA.isAftermath && cB.isAftermath) RelaxStep(aarectA, rA, aarectB, rB);
                return true;
            }

            return false;
        }
    }

    private static bool isRotatedRectangle(AARectangleCollider aa)
    {
        if (aa is not OBBRectangleCollider oBB) return false;

        Transform transform = oBB.gameObject.transform;
        //float rotationAngle = transform.globalRotationAngle % (2 * MathF.PI); // Ensure angle stays within 0 - 2π radians
        float rotationAngle = (transform.globalRotationAngle % (2 * MathF.PI) + 2 * MathF.PI) % (2 * MathF.PI);


        // 90, 180, 270 have an easier logic being calculated as AACOllision -> return false
        if (rotationAngle == 0
            || MathF.Abs(rotationAngle - MathF.PI / 2) < 0.0001f
            || MathF.Abs(rotationAngle - MathF.PI) < 0.0001f
            || MathF.Abs(rotationAngle - (3 * MathF.PI / 2)) < 0.0001f)
        {
            return false;
        }

        return true;
    }

    private static bool isRotated90Or270(AARectangleCollider aa)
    {
        if (aa is not OBBRectangleCollider oBB) return false;

        Transform transform = oBB.gameObject.transform;
        //float rotationAngle = transform.globalRotationAngle % (2 * MathF.PI); // Ensure angle stays within 0 - 2π radians
        float rotationAngle = (transform.globalRotationAngle % (2 * MathF.PI) + 2 * MathF.PI) % (2 * MathF.PI);

        // Check if the rotation is either 90° (π/2) or 270° (3π/2)
        return MathF.Abs(rotationAngle - MathF.PI / 2) < 0.0001f || MathF.Abs(rotationAngle - (3 * MathF.PI / 2)) < 0.0001f;
    }

    public struct FloatRect
    {
        public float Left;
        public float Top;
        public float Width;
        public float Height;

        public float Right => Left + Width;
        public float Bottom => Top + Height;

        public float CenterX => Left + Width / 2f;
        public float CenterY => Top + Height / 2f;
    }

    private static FloatRect GetFloatRectangle(AARectangleCollider aaRectangle)
    {
        Vector2 center = aaRectangle.gameObject.transform.globalPosition;

        bool rotated = isRotated90Or270(aaRectangle);
        float width = rotated ? aaRectangle.Height : aaRectangle.Width;
        float height = rotated ? aaRectangle.Width : aaRectangle.Height;

        return new FloatRect
        {
            Left = center.X - width / 2f,
            Top = center.Y - height / 2f,
            Width = width,
            Height = height
        };
    }
    protected static void RelaxStep(AARectangleCollider aaRectangle1, FloatRect rectangleA, AARectangleCollider aaRectangle2, FloatRect rectangleB)
    {
        if (!aaRectangle1.isRelaxPosition && !aaRectangle2.isRelaxPosition) return;

        // Calculate the differences between rectangle centers
        float horizontalDifference = rectangleA.CenterX - rectangleB.CenterX;
        float verticalDifference = rectangleA.CenterY - rectangleB.CenterY;

        // Calculate the actual overlap (collided distances)
        float horizontalCollidedDistance = MathF.Abs(horizontalDifference);
        float verticalCollidedDistance = MathF.Abs(verticalDifference);

        // Calculate the minimum distances for overlap to begin
        float horizontalMinimumDistance = rectangleA.Width / 2f + rectangleB.Width / 2f;
        float verticalMinimumDistance = rectangleA.Height / 2f + rectangleB.Height / 2f;

        // Calculate how much to move objects to resolve the overlap (relaxation)
        float horizontalRelaxDistance = horizontalMinimumDistance - horizontalCollidedDistance;
        float verticalRelaxDistance = verticalMinimumDistance - verticalCollidedDistance;

        // Minimum threshold to avoid micro-jitter
        const float minRelaxDistanceThreshold = 0.01f;
        horizontalRelaxDistance = MathF.Max(horizontalRelaxDistance, minRelaxDistanceThreshold);
        verticalRelaxDistance = MathF.Max(verticalRelaxDistance, minRelaxDistanceThreshold);

        // Decide whether to relax along X or Y axis
        Vector2 collisionNormal;
        float relaxDistance;

        if (horizontalRelaxDistance < verticalRelaxDistance)
        {
            relaxDistance = horizontalRelaxDistance;
            collisionNormal = new Vector2(horizontalDifference < 0 ? 1 : -1, 0);
        }
        else
        {
            relaxDistance = verticalRelaxDistance;
            collisionNormal = new Vector2(0, verticalDifference < 0 ? 1 : -1);
        }

        // Apply relaxation vector
        Vector2 relaxDistanceVector = collisionNormal * relaxDistance;

        CollisionLogic.RelaxCollision(
            aaRectangle1.gameObject.GetComponent<PhysicsComponent>(), aaRectangle1.isRelaxPosition,
            aaRectangle2.gameObject.GetComponent<PhysicsComponent>(), aaRectangle2.isRelaxPosition,
            relaxDistanceVector
        );

        // Optional: Handle energy transfer
        // ExchangeEnergy(aaRectangle1, aaRectangle2, collisionNormal);
    }


    /*
    protected static void RelaxStep(AARectangleCollider aaRectangle1, FloatRect rectangleA, AARectangleCollider aaRectangle2, FloatRect rectangleB)
    {
        if (!aaRectangle1.isRelaxPosition && !aaRectangle2.isRelaxPosition) return;

        // Get the actual rectangles (considering rotation)

        // Calculate the horizontal and vertical distances using the rectangles
        float horizontalDifference = rectangleA.X - rectangleB.X;
        float verticalDifference = rectangleA.Y - rectangleB.Y;

        // Calculate the collided distances and relaxation distances
        float horizontalCollidedDistance = System.Math.Abs(horizontalDifference);
        float verticalCollidedDistance = System.Math.Abs(verticalDifference);

        float horizontalMinimumDistance = rectangleA.Width / 2 + rectangleB.Width / 2;
        float verticalMinimumDistance = rectangleA.Height / 2 + rectangleB.Height / 2;

        // Relaxation distances
        float horizontalRelaxDistance = horizontalMinimumDistance - horizontalCollidedDistance;
        float verticalRelaxDistance = verticalMinimumDistance - verticalCollidedDistance;

        // Minimum threshold to avoid small artifacts
        const float minRelaxDistanceThreshold = 0.01f;  // Adjust this based on your needs
        horizontalRelaxDistance = MathF.Max(horizontalRelaxDistance, minRelaxDistanceThreshold);
        verticalRelaxDistance = MathF.Max(verticalRelaxDistance, minRelaxDistanceThreshold);

        // Determine the smaller relaxation distance and set the normal
        Vector2 collisionNormal;
        float relaxDistance;

        if (horizontalRelaxDistance < verticalRelaxDistance)
        {
            relaxDistance = horizontalRelaxDistance;
            collisionNormal = new Vector2(horizontalDifference < 0 ? 1 : -1, 0);
        }
        else
        {
            relaxDistance = verticalRelaxDistance;
            collisionNormal = new Vector2(0, verticalDifference < 0 ? 1 : -1);
        }

        // Apply the relaxation step
        Vector2 relaxDistanceVector = collisionNormal * relaxDistance;
        CollisionLogic.RelaxCollision(aaRectangle1.gameObject.GetComponent<PhysicsComponent>(), aaRectangle1.isRelaxPosition,
                                      aaRectangle2.gameObject.GetComponent<PhysicsComponent>(), aaRectangle2.isRelaxPosition,
                                      relaxDistanceVector);

        // Optionally handle energy exchange if needed
        //ExchangeEnergy(aaRectangle1, aaRectangle2, collisionNormal);
    }

    /*
    private static Rectangle GetRectangle(AARectangleCollider aaRectangle)
    {
        Vector2 center = aaRectangle.gameObject.transform.globalPosition;

        // Handle 90° or 270° rotation case: width and height are flipped
        if (isRotated90Or270(aaRectangle))
        {
            return new Rectangle(
                x: (int)(center.X - aaRectangle.Height / 2),
                y: (int)(center.Y - aaRectangle.Width / 2),
                width: (int)aaRectangle.Height,
                height: (int)aaRectangle.Width
            );
        }
        else
        {
            return new Rectangle(
                x: (int)(center.X - aaRectangle.Width / 2),
                y: (int)(center.Y - aaRectangle.Height / 2),
                width: (int)aaRectangle.Width,
                height: (int)aaRectangle.Height
            );
        }
    }*/

    /*
    protected static void RelaxStep(AARectangleCollider aaRectangle1, AARectangleCollider aaRectangle2)
    {
        if (!aaRectangle1.isRelaxPosition && !aaRectangle2.isRelaxPosition) return;

        float horizontalDifference = aaRectangle1.gameObject.transform.globalPosition.X - aaRectangle2.gameObject.transform.globalPosition.X;
        float horizontalCollidedDistance = System.Math.Abs(horizontalDifference);
        float horizontalMinimumDistance = aaRectangle1.Width / 2 + aaRectangle2.Width / 2;
        float horizontalRelaxDistance = horizontalMinimumDistance - horizontalCollidedDistance;
        float verticalDifference = aaRectangle1.gameObject.transform.globalPosition.Y - aaRectangle2.gameObject.transform.globalPosition.Y;
        float verticalCollidedDistance = System.Math.Abs(verticalDifference);
        float verticalMinimumDistance = aaRectangle1.Height / 2 + aaRectangle2.Height / 2;
        float verticalRelaxDistance = verticalMinimumDistance - verticalCollidedDistance;
        Vector2 collisionNormal;
        float relaxDistance;
        if (horizontalRelaxDistance < verticalRelaxDistance)
        {
            relaxDistance = horizontalRelaxDistance;
            collisionNormal = new Vector2(horizontalDifference < 0 ? 1 : -1, 0);
        }
        else
        {
            relaxDistance = verticalRelaxDistance;
            collisionNormal = new Vector2(0, verticalDifference < 0 ? 1 : -1);
        }

        Vector2 relaxDistanceVector = collisionNormal * relaxDistance;
        CollisionLogic.RelaxCollision(aaRectangle1.gameObject.GetComponent<PhysicsComponent>(), aaRectangle1.isRelaxPosition,
                                      aaRectangle2.gameObject.GetComponent<PhysicsComponent>(), aaRectangle2.isRelaxPosition,
                                      relaxDistanceVector);
        //ExchangeEnergy(aaRectangle1, aaRectangle2, collisionNormal);
    }*/
}
