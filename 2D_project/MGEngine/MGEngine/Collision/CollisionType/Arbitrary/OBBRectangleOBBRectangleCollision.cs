using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
public class OBBRectangleOBBRectangleCollision
{
    public static bool OBBRectangle_OBBRectangleColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not OBBRectangleCollider obbA || cB is not OBBRectangleCollider obbB) return false;

        // Get rectangle properties including rotation
        Vector2[] verticesA = GetRotatedRectangleVertices(obbA);
        Vector2[] verticesB = GetRotatedRectangleVertices(obbB);

        Vector2[] axes = new Vector2[4]
        {
            GetEdgeNormal(verticesA[0], verticesA[1]), // Normal of one edge of A
            GetEdgeNormal(verticesA[1], verticesA[2]),
            GetEdgeNormal(verticesB[0], verticesB[1]), // Normal of one edge of B
            GetEdgeNormal(verticesB[1], verticesB[2])
        };

        Vector2 minTranslationVector = Vector2.Zero;
        float minOverlap = float.MaxValue;

        // Check for separation along each axis
        foreach (Vector2 axis in axes)
        {
            float minA, maxA, minB, maxB;
            ProjectVertices(verticesA, axis, out minA, out maxA);
            ProjectVertices(verticesB, axis, out minB, out maxB);

            float overlap = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
            if (overlap <= 0) return false; // No collision

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                minTranslationVector = axis * overlap;
            }
        }

        RelaxStep(obbA, obbB, minTranslationVector);
        return true;
    }

    private static Vector2[] GetRotatedRectangleVertices(OBBRectangleCollider rect)
    {
        Vector2 center = rect.gameObject.transform.globalPosition;
        float angle = rect.gameObject.transform.globalRotationAngle;
        float halfWidth = rect.Width / 2;
        float halfHeight = rect.Height / 2;

        Vector2[] localVertices = new Vector2[]
        {
            new Vector2(-halfWidth, -halfHeight),
            new Vector2(halfWidth, -halfHeight),
            new Vector2(halfWidth, halfHeight),
            new Vector2(-halfWidth, halfHeight)
        };

        Matrix rotationMatrix = Matrix.CreateRotationZ(angle);
        for (int i = 0; i < localVertices.Length; i++)
        {
            localVertices[i] = Vector2.Transform(localVertices[i], rotationMatrix) + center;
        }

        return localVertices;
    }

    private static Vector2 GetEdgeNormal(Vector2 start, Vector2 end)
    {
        Vector2 edge = end - start;
        return new Vector2(-edge.Y, edge.X); // Perpendicular vector
    }

    private static void ProjectVertices(Vector2[] vertices, Vector2 axis, out float min, out float max)
    {
        min = max = Vector2.Dot(vertices[0], axis);
        for (int i = 1; i < vertices.Length; i++)
        {
            float projection = Vector2.Dot(vertices[i], axis);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }

    private static void RelaxStep(Collider rectA, Collider rectB, Vector2 relaxVector)
    {
        if (!rectA.isRelaxPosition && !rectB.isRelaxPosition) return;

        Vector2 moveA = Vector2.Zero;
        Vector2 moveB = Vector2.Zero;

        // If only one object can move, it should take full displacement
        if (!rectA.isRelaxPosition) moveB = -relaxVector;
        else if (!rectB.isRelaxPosition) moveA = relaxVector;
        else
        {
            // Otherwise, split displacement equally
            moveA = relaxVector / 2;
            moveB = -relaxVector / 2;
        }

        // Constrain movement for AABBs (they should only move in X or Y)
        if (rectA is AARectangleCollider)
        {
            if (Math.Abs(moveA.X) > Math.Abs(moveA.Y)) moveA.Y = 0;
            else moveA.X = 0;
        }
        if (rectB is AARectangleCollider)
        {
            if (Math.Abs(moveB.X) > Math.Abs(moveB.Y)) moveB.Y = 0;
            else moveB.X = 0;
        }
        //Debug.WriteLine(rectA.gameObject.transform.globalRotationAngle);
        //Debug.WriteLine(rectB.gameObject.transform.globalRotationAngle);
        CollisionLogic.RelaxCollision(rectA.gameObject.GetComponent<PhysicsComponent>(), rectA.isRelaxPosition,
                                      rectB.gameObject.GetComponent<PhysicsComponent>(), rectB.isRelaxPosition,
                                      moveA + moveB);
    }

}
