using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
public class ParticleOBBRectangleCollision
{
    public static bool Particle_OBBRectangleColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not ParticleCollider pA || cB is not OBBRectangleCollider obbRectB) return false;

        // Ensure the OBB is rotated
        if (obbRectB.gameObject.transform.globalRotationAngle == 0) return false;

        // Calculate the relaxation distance
        Vector2 relaxDistance = CalculateRelaxDistance(pA, obbRectB);
        if (relaxDistance.LengthSquared() > 0) // If there is a valid overlap
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(pA, obbRectB, relaxDistance); // Perform the relaxation step
            return true;
        }

        return false;
    }


    /*
    private static Vector2 CalculateRelaxDistance(ParticleCollider particle, OBBRectangleCollider obbRectangle)
    {
        Vector2 relaxDistance = Vector2.Zero;

        // Get the rectangle vertices based on rotation
        Vector2[] rectangleVertices = OBBRectangleCollider.GetRotatedRectangleVertices(obbRectangle);

        // Find the nearest point on the OBB to the particle
        Vector2 nearestPoint = FindNearestPointOnOBB(particle.gameObject.transform.globalPosition, rectangleVertices);

        // Calculate the vector from particle to the nearest point on the rectangle
        Vector2 distanceVector = particle.gameObject.transform.globalPosition - nearestPoint;
        float distance = distanceVector.Length();

        // If the distance is less than or equal to the particle's radius, a collision occurred
        if (distance < particle.radius)
        {
            // Return the vector to push the particle out of the collision
            relaxDistance = Vector2.Normalize(distanceVector) * (particle.radius - distance);
        }

        if (obbRectangle.gameObject.id == 7777)
        {
            Debug.WriteLine("particlePosition: " + particle.gameObject.transform.globalPosition);
            Debug.WriteLine("radius: " + particle.radius);
            Debug.WriteLine("rectPos: " + obbRectangle.gameObject.transform.globalPosition);
            Debug.WriteLine("rectVertices: [" + rectangleVertices[0] + ", "+ rectangleVertices[1] + ", " + rectangleVertices[2] + ", " + rectangleVertices[2] + "]");
            Debug.WriteLine("rectPos: " + obbRectangle.gameObject.transform.globalPosition);
            Debug.WriteLine("nearest point: "+ nearestPoint);
            Debug.WriteLine("distanceVector: " + distanceVector);
            Debug.WriteLine("distance: " + distance);
        }

        return relaxDistance;
    }

    private static Vector2 FindNearestPointOnOBB(Vector2 point, Vector2[] vertices)
    {
        // Using the Separating Axis Theorem to project the point onto the axes of the OBB's edges
        Vector2 nearestPoint = Vector2.Zero;

        // Loop through each edge of the OBB and project the point onto the edge's normal
        for (int i = 0; i < 4; i++)
        {
            Vector2 edgeStart = vertices[i];
            Vector2 edgeEnd = vertices[(i + 1) % 4];
            Vector2 edgeNormal = GetEdgeNormal(edgeStart, edgeEnd);

            // Project the point onto the edge's normal
            float projection = Vector2.Dot(point - edgeStart, edgeNormal);
            nearestPoint += edgeNormal * projection;
        }

        return nearestPoint;
    }

    private static Vector2 GetEdgeNormal(Vector2 start, Vector2 end)
    {
        Vector2 edge = end - start;
        return new Vector2(-edge.Y, edge.X); // Perpendicular vector
    }

    protected static void RelaxStep(ParticleCollider particle, OBBRectangleCollider obbRectangle, Vector2 relaxDistance)
    {
        if (!particle.isRelaxPosition && !obbRectangle.isRelaxPosition) return;

        // Perform the relaxation step by moving the particle and the rectangle
        CollisionLogic.RelaxCollision(particle.gameObject.GetComponent<PhysicsComponent>(), particle.isRelaxPosition,
                                      obbRectangle.gameObject.GetComponent<PhysicsComponent>(), obbRectangle.isRelaxPosition,
                                      relaxDistance);
    }
    */

    private static Vector2 CalculateRelaxDistance(ParticleCollider particle, OBBRectangleCollider obbRectangle)
    {
        Vector2 relaxDistance = Vector2.Zero;

        // Get the rectangle vertices based on rotation
        Vector2[] rectangleVertices = OBBRectangleCollider.GetRotatedRectangleVertices(obbRectangle);

        // Find the nearest point on the OBB to the particle
        Vector2 nearestPoint = FindNearestPointOnOBB(particle.gameObject.transform.globalPosition, rectangleVertices);

        // Calculate the vector from particle to the nearest point on the rectangle
        Vector2 distanceVector = particle.gameObject.transform.globalPosition - nearestPoint;
        float distance = distanceVector.Length();

        // If the distance is less than or equal to the particle's radius, a collision occurred
        if (distance < particle.radius)
        {
            // Return the vector to push the particle out of the collision
            relaxDistance = Vector2.Normalize(distanceVector) * (particle.radius - distance);
        }

        return relaxDistance;
    }

    private static Vector2 FindNearestPointOnOBB(Vector2 point, Vector2[] vertices)
    {
        // Assume vertices are ordered clockwise or counterclockwise:
        // vertices[0]: origin corner
        // vertices[1]: adjacent corner on one edge
        // vertices[3]: adjacent corner on the other edge

        Vector2 origin = vertices[0];
        Vector2 edge1 = vertices[1] - origin; // One edge vector
        Vector2 edge2 = vertices[3] - origin; // Other edge vector

        // Compute squared lengths for clamping
        float edge1LengthSquared = edge1.LengthSquared();
        float edge2LengthSquared = edge2.LengthSquared();

        // Vector from origin to point
        Vector2 pointVector = point - origin;

        // Project pointVector onto each edge to get coordinates in OBB local space
        float proj1 = Vector2.Dot(pointVector, edge1);
        float proj2 = Vector2.Dot(pointVector, edge2);

        // Clamp projections to edge lengths to stay inside the rectangle
        proj1 = MathHelper.Clamp(proj1, 0, edge1LengthSquared);
        proj2 = MathHelper.Clamp(proj2, 0, edge2LengthSquared);

        // Convert back to world space nearest point
        Vector2 nearestPoint = origin + (edge1 * (proj1 / edge1LengthSquared)) + (edge2 * (proj2 / edge2LengthSquared));
        return nearestPoint;
    }

    protected static void RelaxStep(ParticleCollider particle, OBBRectangleCollider obbRectangle, Vector2 relaxDistance)
    {
        if (!particle.isRelaxPosition && !obbRectangle.isRelaxPosition) return;

        // Perform the relaxation step by moving the particle and the rectangle
        CollisionLogic.RelaxCollision(particle.gameObject.GetComponent<PhysicsComponent>(), particle.isRelaxPosition,
                                      obbRectangle.gameObject.GetComponent<PhysicsComponent>(), obbRectangle.isRelaxPosition,
                                      relaxDistance);
    }
}
