using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class OBBRectangleConvexCollision
{
    internal static bool OBBRectangle_ConvexCollision(Collider cA, Collider cB)
    {
        if (cA is not OBBRectangleCollider obbA || cB is not ConvexCollider convexB) return false;

        // Get the transformed vertices of both the OBB and the Convex Polygon
        List<Vector2> obbVertices = GetTransformedOBBVertices(cA as OBBRectangleCollider);
        List<Vector2> convexVertices = GetTransformedConvexVertices(cB as ConvexCollider);

        // Get the axes (OBB’s axes + Convex’s edge normals)
        List<Vector2> axes = GetEdgeNormals(obbVertices, convexVertices);

        // Perform SAT collision check by projecting both shapes onto each axis
        foreach (var axis in axes)
        {
            var obbProjection = ProjectShape(obbVertices, axis);
            var convexProjection = ProjectShape(convexVertices, axis);

            // If there is no overlap in the projections, no collision
            if (!IsOverlapping(obbProjection, convexProjection))
                return false;
        }

        // If projections overlap on all axes, then there is a collision
        return true;
    }

    private static List<Vector2> GetTransformedOBBVertices(OBBRectangleCollider obb)
    {
        Vector2 offset = obb.gameObject.transform.globalPosition;
        float angle = obb.gameObject.transform.globalRotationAngle;
        Matrix transformMatrix = Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(offset.X, offset.Y, 0);

        List<Vector2> vertices = new List<Vector2>(4);
        // OBB vertices (4 vertices forming a rotated rectangle)
        Vector2 topLeft = new Vector2(-obb.Width / 2, -obb.Height / 2);
        Vector2 topRight = new Vector2(obb.Width / 2, -obb.Height / 2);
        Vector2 bottomLeft = new Vector2(-obb.Width / 2, obb.Height / 2);
        Vector2 bottomRight = new Vector2(obb.Width / 2, obb.Height / 2);

        vertices.Add(Vector2.Transform(topLeft, transformMatrix));
        vertices.Add(Vector2.Transform(topRight, transformMatrix));
        vertices.Add(Vector2.Transform(bottomLeft, transformMatrix));
        vertices.Add(Vector2.Transform(bottomRight, transformMatrix));

        return vertices;
    }

    /// <summary>
    /// Converts local vertices to global positions by adding gameObject.transform.globalPosition.
    /// </summary>
    private static List<Vector2> GetTransformedConvexVertices(ConvexCollider collider)
    {
        Vector2 offset = collider.gameObject.transform.globalPosition;
        float angle = collider.gameObject.transform.globalRotationAngle; // Rotation in radians
        Vector2 scale = collider.gameObject.transform.globalScale; // Scaling

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

    private static List<Vector2> GetEdgeNormals(List<Vector2> obbVertices, List<Vector2> convexVertices)
    {
        List<Vector2> axes = new List<Vector2>();

        // OBB's edge normals (2 axes)
        Vector2 edge1 = obbVertices[1] - obbVertices[0];
        Vector2 edge2 = obbVertices[2] - obbVertices[1];
        axes.Add(Vector2.Normalize(new Vector2(-edge1.Y, edge1.X))); // Normal of edge 1
        axes.Add(Vector2.Normalize(new Vector2(-edge2.Y, edge2.X))); // Normal of edge 2

        // Convex shape's edge normals
        for (int i = 0; i < convexVertices.Count; i++)
        {
            Vector2 edge = convexVertices[(i + 1) % convexVertices.Count] - convexVertices[i];
            axes.Add(Vector2.Normalize(new Vector2(-edge.Y, edge.X))); // Normal of the edge
        }

        return axes;
    }
    private static (float min, float max) ProjectShape(List<Vector2> vertices, Vector2 axis)
    {
        float min = Vector2.Dot(vertices[0], axis);
        float max = min;

        foreach (var vertex in vertices)
        {
            float projection = Vector2.Dot(vertex, axis);
            min = Math.Min(min, projection);
            max = Math.Max(max, projection);
        }

        return (min, max);
    }

    private static bool IsOverlapping((float min, float max) projectionA, (float min, float max) projectionB)
    {
        return projectionA.max >= projectionB.min && projectionB.max >= projectionA.min;
    }
}
