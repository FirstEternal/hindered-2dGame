using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
public class ParticleHalfPlaneCollision
{
    public static bool ParticleHalfPlaneColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not ParticleCollider pA || cB is not HalfPlaneCollider hpB) return false;

        // is there collision
        float nearPoint = Vector2.Dot(pA.gameObject.transform.globalPosition, hpB.HalfPlane.Normal) - pA.radius;
        if (nearPoint < hpB.HalfPlane.Distance)
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(pA, hpB);
            return true;
        }

        return false;
    }
    protected static void RelaxStep(ParticleCollider particle, HalfPlaneCollider halfPlane)
    {
        if (!particle.isRelaxPosition && !halfPlane.isRelaxPosition) return;

        float nearPoint = Vector2.Dot(particle.gameObject.transform.globalPosition, halfPlane.HalfPlane.Normal) - particle.radius;
        float relaxDistance = nearPoint - halfPlane.HalfPlane.Distance;

        Vector2 relaxDistanceVector = halfPlane.HalfPlane.Normal * relaxDistance;
        CollisionLogic.RelaxCollision(particle.gameObject.GetComponent<PhysicsComponent>(), particle.isRelaxPosition,
                                      halfPlane.gameObject.GetComponent<PhysicsComponent>(), halfPlane.isRelaxPosition,
                                      relaxDistanceVector);

        Vector2 collisionNormal = Vector2.Normalize(relaxDistanceVector);
        Vector2 pointOfImpact = (particle.gameObject.transform.globalPosition + (collisionNormal * (relaxDistance + particle.radius)));
        //ExchangeEnergy(particle, halfPlane, collisionNormal, pointOfImpact);
    }

}