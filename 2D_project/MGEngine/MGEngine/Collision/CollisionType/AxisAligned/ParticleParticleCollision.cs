using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class ParticleParticleCollision
{
    public static bool Particle_ParticleColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not ParticleCollider pA || cB is not ParticleCollider pB) return false;

        float radiusA = pA.radius;
        float radiusB = pB.radius;
        // is there collision
        float distanceBetweenParticles = (pA.gameObject.transform.globalPosition - pB.gameObject.transform.globalPosition).Length();
        //return distanceBetweenParticles < pA.radius + pB.radius;
        if (distanceBetweenParticles < radiusA + radiusB)
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(pA, pB, radiusA, radiusB);
            return true;
        }

        return false;
    }
    private static void RelaxStep(ParticleCollider pA, ParticleCollider pB, float radiusA, float radiusB)
    {
        if (!pA.isRelaxPosition && !pB.isRelaxPosition) return;

        // RELAXATION STEP

        // First we relax the collision, so the two objects don't collide any more.
        // We need to calculate by how much to move them apart. We will move them in the shortest direction
        // possible which is simply the difference between both centers.

        Vector2 positionDifference = pB.gameObject.transform.globalPosition - pA.gameObject.transform.globalPosition;

        float collidedDistance = positionDifference.Length();
        float minimumDistance = radiusA + radiusB;
        float relaxDistance = minimumDistance - collidedDistance;

        Vector2 collisionNormal = collidedDistance != 0f ? Vector2.Normalize(positionDifference) : Vector2.UnitX;
        Vector2 relaxDistanceVector = collisionNormal * relaxDistance;

        CollisionLogic.RelaxCollision(pA.gameObject.GetComponent<PhysicsComponent>(), pA.isRelaxPosition,
                                      pB.gameObject.GetComponent<PhysicsComponent>(), pB.isRelaxPosition,
                                      relaxDistanceVector);

        // ENERGY EXCHANGE STEP

        // In a collision, energy is exchanged only along the collision normal.
        // For particles this is simply the line between both centers.
        //ExchangeEnergy(pA, pB, collisionNormal);
    }
}