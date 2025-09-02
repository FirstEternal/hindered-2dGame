using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
public class ConvexConvexCollision
{
    private const float epsilon = 1e-5f; // Small epsilon for floating-point comparison

    public static bool Convex_ConvexCollision(Collider cA, Collider cB)
    {
        if (cA is not ConvexCollider cxA || cB is not ConvexCollider cxB) return false;

        // Check SAT in both directions
        if (CheckSAT(cxA, cxB) && CheckSAT(cxB, cxA))
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(cxA, cxB);
            return true;
        }
        return false;
    }

    private static bool CheckSAT(ConvexCollider cxA, ConvexCollider cxB)
    {
        List<Vector2> edges = cxA.Bounds.Edges;
        foreach (Vector2 edge in edges)
        {
            Vector2 axis = Vector2.Normalize(new Vector2(-edge.Y, edge.X)); // Perpendicular axis
            if (!ProjectionsOverlap(GetGlobalVertices(cxA), GetGlobalVertices(cxB), axis))
            {
                return false; // Found a separating axis, no collision
            }
        }
        return true;
    }

    private static bool ProjectionsOverlap(List<Vector2> verticesA, List<Vector2> verticesB, Vector2 axis)
    {
        float minA = float.MaxValue, maxA = float.MinValue;
        float minB = float.MaxValue, maxB = float.MinValue;

        // Project vertices of A onto the axis
        foreach (Vector2 v in verticesA)
        {
            float projection = Vector2.Dot(v, axis);
            minA = Math.Min(minA, projection);
            maxA = Math.Max(maxA, projection);
        }

        // Project vertices of B onto the axis
        foreach (Vector2 v in verticesB)
        {
            float projection = Vector2.Dot(v, axis);
            minB = Math.Min(minB, projection);
            maxB = Math.Max(maxB, projection);
        }

        // Use epsilon to handle floating point comparison
        return !(maxA + epsilon < minB || maxB + epsilon < minA); // True if projections overlap
    }

    protected static void RelaxStep(ConvexCollider cxA, ConvexCollider cxB)
    {
        if (!cxA.isRelaxPosition && !cxB.isRelaxPosition) return;

        // Calculate relax distance based on the smallest overlap
        Vector2 relaxDistance = CalculateRelaxDistance(cxA, cxB);
        CollisionLogic.RelaxCollision(
            cxA.gameObject.GetComponent<PhysicsComponent>(), cxA.isRelaxPosition,
            cxB.gameObject.GetComponent<PhysicsComponent>(), cxB.isRelaxPosition,
            relaxDistance);
    }

    private static Vector2 CalculateRelaxDistance(ConvexCollider cxA, ConvexCollider cxB)
    {
        Vector2 smallestAxis = Vector2.Zero;
        float smallestOverlap = float.MaxValue;

        List<Vector2> edges = cxA.Bounds.Edges.Concat(cxB.Bounds.Edges).ToList();
        foreach (Vector2 edge in edges)
        {
            Vector2 axis = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
            float overlap = GetOverlap(GetGlobalVertices(cxA), GetGlobalVertices(cxB), axis);

            if (overlap < smallestOverlap)
            {
                smallestOverlap = overlap;
                smallestAxis = axis;
            }
        }

        // Multiply the axis by the overlap distance
        return smallestAxis * smallestOverlap;
    }

    private static float GetOverlap(List<Vector2> verticesA, List<Vector2> verticesB, Vector2 axis)
    {
        float minA = float.MaxValue, maxA = float.MinValue;
        float minB = float.MaxValue, maxB = float.MinValue;

        // Project vertices of A onto the axis
        foreach (Vector2 v in verticesA)
        {
            float projection = Vector2.Dot(v, axis);
            minA = Math.Min(minA, projection);
            maxA = Math.Max(maxA, projection);
        }

        // Project vertices of B onto the axis
        foreach (Vector2 v in verticesB)
        {
            float projection = Vector2.Dot(v, axis);
            minB = Math.Min(minB, projection);
            maxB = Math.Max(maxB, projection);
        }

        // Use epsilon for precision issues
        return Math.Max(0, Math.Min(maxA + epsilon, maxB + epsilon) - Math.Max(minA - epsilon, minB - epsilon));
    }

    /// <summary>
    /// Converts local vertices to global positions by adding gameObject.transform.globalPosition.
    /// </summary>
    private static List<Vector2> GetGlobalVertices(ConvexCollider collider)
    {
        Vector2 offset = collider.gameObject.transform.globalPosition;
        float angle = collider.gameObject.transform.globalRotationAngle; // Rotation in radians
        Vector2 scale = collider.isScalable ? collider.gameObject.transform.globalScale : Vector2.One; // Scaling

        // Create transformation matrix (rotation + translation + scaling)
        Matrix transformMatrix = Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(offset.X, offset.Y, 0);

        List<Vector2> vertices = collider.Bounds.Vertices;
        List<Vector2> transformedVertices = new List<Vector2>(vertices.Count);

        // Transform all vertices using the matrix (with scaling, rotation, and translation)
        for (int i = 0; i < vertices.Count; i++)
        {
            transformedVertices.Add(Vector2.Transform(vertices[i], transformMatrix));
        }

        return transformedVertices;
    }
}
