using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class ParticleAAHalfPlaneCollision
{
    public static bool Particle_AAHalfPlaneColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not ParticleCollider pA || cB is not AAHalfPlaneCollider aahpB) return false;

        bool isColliding = false;
        // is there collision
        switch (aahpB.AAHalfPlane.Direction)
        {
            default:
            case AxisDirection.PositiveX:
                isColliding = pA.gameObject.transform.globalPosition.X - pA.radius < aahpB.AAHalfPlane.Distance;
                break;
            case AxisDirection.NegativeX:
                isColliding = pA.gameObject.transform.globalPosition.X + pA.radius > -aahpB.AAHalfPlane.Distance;
                break;
            case AxisDirection.PositiveY:
                isColliding = pA.gameObject.transform.globalPosition.Y - pA.radius < aahpB.AAHalfPlane.Distance;
                break;
            case AxisDirection.NegativeY:
                isColliding = pA.gameObject.transform.globalPosition.Y + pA.radius > -aahpB.AAHalfPlane.Distance;
                break;
        }

        if (isColliding)
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(pA, aahpB);
        }

        return isColliding;
    }

    protected static void RelaxStep(ParticleCollider particle, AAHalfPlaneCollider aaHalfPlane)
    {
        if (!particle.isRelaxPosition && !aaHalfPlane.isRelaxPosition) return;

        // RELAXATION STEP
        // First we relax the collision, so the two objects don't collide any more.
        Vector2 relaxDistance = Vector2.Zero;
        Vector2 pointOfImpact = Vector2.Zero;
        switch (aaHalfPlane.AAHalfPlane.Direction)
        {
            case AxisDirection.PositiveX:
                relaxDistance = new Vector2(particle.gameObject.transform.globalPosition.X - particle.radius - aaHalfPlane.AAHalfPlane.Distance,
                    0);
                pointOfImpact = new Vector2(aaHalfPlane.AAHalfPlane.Distance, particle.gameObject.transform.globalPosition.Y);
                break;
            case AxisDirection.NegativeX:
                relaxDistance = new Vector2(particle.gameObject.transform.globalPosition.X + particle.radius + aaHalfPlane.AAHalfPlane.Distance,
                    0);
                pointOfImpact = new Vector2(-aaHalfPlane.AAHalfPlane.Distance, particle.gameObject.transform.globalPosition.Y);
                break;
            case AxisDirection.PositiveY:
                relaxDistance = new Vector2(0,
                    particle.gameObject.transform.globalPosition.Y - particle.radius - aaHalfPlane.AAHalfPlane.Distance);
                pointOfImpact = new Vector2(particle.gameObject.transform.globalPosition.X, aaHalfPlane.AAHalfPlane.Distance);
                break;
            case AxisDirection.NegativeY:
                relaxDistance = new Vector2(0,
                    particle.gameObject.transform.globalPosition.Y + particle.radius + aaHalfPlane.AAHalfPlane.Distance);
                pointOfImpact = new Vector2(particle.gameObject.transform.globalPosition.X, -aaHalfPlane.AAHalfPlane.Distance);
                break;
        }

        CollisionLogic.RelaxCollision(particle.gameObject.GetComponent<PhysicsComponent>(), particle.isRelaxPosition,
                                      aaHalfPlane.gameObject.GetComponent<PhysicsComponent>(), aaHalfPlane.isRelaxPosition,
                                      relaxDistance);
        // ENERGY EXCHANGE STEP
        // In a collision, energy is exchanged only along the collision normal.
        // For particles this is simply the line between both centers.
        Vector2 collisionNormal = Vector2.Normalize(relaxDistance);
        //ExchangeEnergy(particle, aaHalfPlane, collisionNormal, pointOfImpact);
    }
}