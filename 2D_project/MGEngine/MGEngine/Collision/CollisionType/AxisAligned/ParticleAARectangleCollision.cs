using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class ParticleAARectangleCollision
{
    public static bool Particle_AARectangleColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not ParticleCollider pA || cB is not AARectangleCollider aarectB) return false;
        if (cB.gameObject.transform.globalRotationAngle != 0) return false;

        // is there collision
        Vector2 relaxDistance = CalculateRelaxDistance(pA, aarectB);
        if (relaxDistance.LengthSquared() > 0)
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(pA, aarectB);
            return true;
        }
        return false;
    }

    private static Vector2 CalculateRelaxDistance(ParticleCollider particle, AARectangleCollider aaRectangle)
    {
        Vector2 relaxDistance = Vector2.Zero;
        Vector2 nearestVertex = aaRectangle.gameObject.transform.globalPosition;
        float halfWidth = aaRectangle.Width / 2;
        float halfHeight = aaRectangle.Height / 2;
        float leftDifference = (aaRectangle.gameObject.transform.globalPosition.X - halfWidth) - (particle.gameObject.transform.globalPosition.X + particle.radius);
        if (leftDifference > 0) return relaxDistance;

        float rightDifference = (particle.gameObject.transform.globalPosition.X - particle.radius) - (aaRectangle.gameObject.transform.globalPosition.X + halfWidth);
        if (rightDifference > 0) return relaxDistance;

        float topDifference = (aaRectangle.gameObject.transform.globalPosition.Y - halfHeight) - (particle.gameObject.transform.globalPosition.Y + particle.radius);
        if (topDifference > 0) return relaxDistance;

        float bottomDifference = (particle.gameObject.transform.globalPosition.Y - particle.radius) - (aaRectangle.gameObject.transform.globalPosition.Y + halfHeight);
        if (bottomDifference > 0) return relaxDistance;

        bool horizontallyInside = false;
        bool verticallyInside = false;
        if (particle.gameObject.transform.globalPosition.X < aaRectangle.gameObject.transform.globalPosition.X - halfWidth)
        {
            nearestVertex.X -= halfWidth;
        }
        else if (particle.gameObject.transform.globalPosition.X > aaRectangle.gameObject.transform.globalPosition.X + halfWidth)
        {
            nearestVertex.X += halfWidth;
        }
        else
        {
            horizontallyInside = true;
        }

        if (particle.gameObject.transform.globalPosition.Y < aaRectangle.gameObject.transform.globalPosition.Y - halfHeight)
        {
            nearestVertex.Y -= halfHeight;
        }
        else if (particle.gameObject.transform.globalPosition.Y > aaRectangle.gameObject.transform.globalPosition.Y + halfHeight)
        {
            nearestVertex.Y += halfHeight;
        }
        else
        {
            verticallyInside = true;
        }

        if (!horizontallyInside && !verticallyInside)
        {
            Vector2 particleVertex = nearestVertex - particle.gameObject.transform.globalPosition;
            float vertexDistance = particleVertex.Length();
            if (vertexDistance > particle.radius)
            {
                return relaxDistance;
            }
            else
            {
                return Vector2.Normalize(particleVertex) * (particle.radius - vertexDistance);
            }

        }

        if (leftDifference > rightDifference)
        {
            relaxDistance.X = -leftDifference;
        }
        else
        {
            relaxDistance.X = rightDifference;
        }

        if (topDifference > bottomDifference)
        {
            relaxDistance.Y = -topDifference;
        }
        else
        {
            relaxDistance.Y = bottomDifference;
        }

        if (System.Math.Abs(relaxDistance.X) < System.Math.Abs(relaxDistance.Y))
        {
            relaxDistance.Y = 0;
        }
        else
        {
            relaxDistance.X = 0;
        }

        return relaxDistance;
    }

    protected static void RelaxStep(ParticleCollider particle, AARectangleCollider aaRectangle)
    {
        if (!particle.isRelaxPosition && !aaRectangle.isRelaxPosition) return;

        Vector2 relaxDistance = CalculateRelaxDistance(particle, aaRectangle);
        CollisionLogic.RelaxCollision(particle.gameObject.GetComponent<PhysicsComponent>(), particle.isRelaxPosition,
                                      aaRectangle.gameObject.GetComponent<PhysicsComponent>(), aaRectangle.isRelaxPosition,
                                      relaxDistance);
        Vector2 collisionNormal = Vector2.Normalize(relaxDistance);
        //ExchangeEnergy(particle, aaRectangle, collisionNormal);
    }
}