using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class ParticleConvexCollision
{
    public static bool Particle_ConvexCollision(Collider cA, Collider cB)
    {
        if (cA is not ParticleCollider pA || cB is not ConvexCollider cxB) return false;

        // is there collision
        Vector2 pointOfImpact = new();
        Vector2 relaxDistance = CalculateRelaxDistance(pA, cxB, ref pointOfImpact);

        if (relaxDistance.LengthSquared() > 0)
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(pA, cxB);
            return true;
        }
        return false;
    }
    public static Vector2 CalculateRelaxDistance(ParticleCollider particle, ConvexCollider convex,
    ref Vector2 pointOfImpact)
    {
        // Get scaling information for the convex collider
        // AT THE MOMENT, this is the only fix i know -> problem is because convex colliders are scalable
        Vector2 scale = Vector2.One; //particle.isScalable && convex.isScalable? convex.gameObject.transform.globalScale : Vector2.One;

        // First move particle in coordinate space of the convex.
        Vector2 offset = convex.gameObject.transform.globalPosition;
        float angle = convex.gameObject.transform.globalRotationAngle;

        // Apply scaling to the transformation matrix
        Matrix transform = Matrix.CreateScale(scale.X, scale.Y, 1) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(offset.X, offset.Y, 0);

        // Adjust the particle's radius based on the scaling of the convex collider
        float adjustedRadius = particle.radius; // Consider enabling scaling if needed

        // Transform the particle position into the convex's coordinate space
        Vector2 relativeParticlePosition = Vector2.Transform(particle.gameObject.transform.globalPosition, Matrix.Invert(transform));

        List<Vector2> vertices = convex.Bounds.Vertices;
        List<HalfPlane> halfPlanes = convex.Bounds.HalfPlanes;
        bool voronoiNearEdge = false;
        float smallestDifference = float.MinValue;
        int smallestDifferenceIndex = -1;
        float smallestDistance = float.MaxValue;
        int smallestDistanceIndex = -1;

        int timesCenterUnderEdge = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            // Relax distance from the plane, now using the adjusted particle radius
            HalfPlane halfPlane = halfPlanes[i];
            float nearPoint = Vector2.Dot(relativeParticlePosition, halfPlane.Normal) - adjustedRadius;
            float relaxDifference = nearPoint - halfPlane.Distance;

            if (relaxDifference > 0)
            {
                return Vector2.Zero; // No collision
            }

            if (smallestDifferenceIndex == -1 || relaxDifference > smallestDifference)
            {
                smallestDifference = relaxDifference;
                smallestDifferenceIndex = i;
            }

            // Distance to vertex
            float distance = (vertices[i] - relativeParticlePosition).Length();
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                smallestDistanceIndex = i;
            }

            // Are we in the voronoi region of this edge?
            float centerDifference = Vector2.Dot(relativeParticlePosition, halfPlane.Normal) - halfPlane.Distance;
            if (centerDifference > 0)
            {
                // Center is above edge, check if between start and end
                Vector2 edge = convex.Bounds.Edges[i];
                Vector2 edgeNormal = Vector2.Normalize(edge);
                float start = Vector2.Dot(vertices[i], edgeNormal);
                float end = Vector2.Dot(edge + vertices[i], edgeNormal);
                float center = Vector2.Dot(relativeParticlePosition, edgeNormal);

                if (start < center && center < end)
                {
                    voronoiNearEdge = true;
                    if (smallestDifferenceIndex == i)
                    {
                        pointOfImpact = vertices[i] + (edge * ((center - start) / (end - start)));
                    }
                }
            }
            else
            {
                timesCenterUnderEdge++;
            }
        }

        Vector2 relaxDistance;

        // Triangle-specific fix: Avoid incorrect Voronoi calculations
        if (vertices.Count == 3)
        {
            voronoiNearEdge = false;
        }

        if (voronoiNearEdge || timesCenterUnderEdge == vertices.Count)
        {
            // The edge is closer than the nearest vertex, so relax in the direction of edge normal
            HalfPlane nearestPlane = halfPlanes[smallestDifferenceIndex];
            relaxDistance = nearestPlane.Normal * Math.Max(smallestDifference, -adjustedRadius * 0.1f);
        }
        else
        {
            // Voronoi region next to nearest vertex
            Vector2 voronoiVertex = vertices[smallestDistanceIndex];
            Vector2 voronoiNormal = Vector2.Normalize(relativeParticlePosition - voronoiVertex);
            float nearPoint = Vector2.Dot(relativeParticlePosition, voronoiNormal) - adjustedRadius;
            float distance = Vector2.Dot(voronoiVertex, voronoiNormal);
            float relaxDifference = nearPoint - distance;

            if (relaxDifference > 0)
            {
                return Vector2.Zero;
            }

            relaxDistance = voronoiNormal * Math.Max(relaxDifference, -adjustedRadius * 0.1f);
            pointOfImpact = voronoiVertex;
        }
        // Transform result vector back into absolute space with scaling, rotation, and translation.
        pointOfImpact = Vector2.Transform(pointOfImpact, transform);
        return Vector2.TransformNormal(relaxDistance, transform);
    }

    protected static void RelaxStep(ParticleCollider particle, ConvexCollider convex)
    {
        if (!particle.isRelaxPosition && !convex.isRelaxPosition) return;
        Vector2 pointOfImpact = new();
        Vector2 relaxDistance = CalculateRelaxDistance(particle, convex, ref pointOfImpact);
        CollisionLogic.RelaxCollision(particle.gameObject.GetComponent<PhysicsComponent>(), particle.isRelaxPosition,
                                      convex.gameObject.GetComponent<PhysicsComponent>(), convex.isRelaxPosition,
                                      relaxDistance);
        Vector2 collisionNormal = Vector2.Normalize(relaxDistance);
        //ExchangeEnergy(particle, convex, collisionNormal, pointOfImpact);
    }

}