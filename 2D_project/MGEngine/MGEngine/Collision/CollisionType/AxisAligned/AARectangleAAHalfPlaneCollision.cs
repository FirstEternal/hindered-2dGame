using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class AARectangleAAHalfPlaneCollision
{
    public static bool AARectangle_AAHalfPlaneColliderCollision(Collider cA, Collider cB)
    {
        if (cA is not AARectangleCollider aarectA || cB is not AAHalfPlaneCollider aahpB) return false;

        bool isCollision = false;
        // is there collision
        switch (aahpB.AAHalfPlane.Direction)
        {
            default:
            case AxisDirection.PositiveX:
                isCollision = aarectA.gameObject.transform.globalPosition.X - aarectA.Width / 2 < aahpB.AAHalfPlane.Distance;
                break;
            case AxisDirection.NegativeX:
                isCollision = aarectA.gameObject.transform.globalPosition.X + aarectA.Width / 2 > -aahpB.AAHalfPlane.Distance;
                break;
            case AxisDirection.PositiveY:
                isCollision = aarectA.gameObject.transform.globalPosition.Y - aarectA.Height / 2 < aahpB.AAHalfPlane.Distance;
                break;
            case AxisDirection.NegativeY:
                isCollision = aarectA.gameObject.transform.globalPosition.Y + aarectA.Height / 2 > -aahpB.AAHalfPlane.Distance;
                break;
        }

        if (isCollision)
        {
            if (cA.isAftermath && cB.isAftermath) RelaxStep(aarectA, aahpB);
        }
        return isCollision;
    }
    protected static void RelaxStep(AARectangleCollider aaRectangle, AAHalfPlaneCollider aaHalfPlane)
    {
        if (!aaRectangle.isRelaxPosition && !aaHalfPlane.isRelaxPosition) return;
        // RELAXATION STEP
        // First we relax the collision, so the two objects don't collide any more.
        Vector2 relaxDistance;
        switch (aaHalfPlane.AAHalfPlane.Direction)
        {
            case AxisDirection.PositiveX:
                relaxDistance = new Vector2(aaRectangle.gameObject.transform.globalPosition.X - aaRectangle.Width / 2 - aaHalfPlane.AAHalfPlane.Distance, 0);
                break;
            case AxisDirection.NegativeX:
                relaxDistance = new Vector2(aaRectangle.gameObject.transform.globalPosition.X + aaRectangle.Width / 2 + aaHalfPlane.AAHalfPlane.Distance, 0);
                break;
            case AxisDirection.PositiveY:
                relaxDistance = new Vector2(0, aaRectangle.gameObject.transform.globalPosition.Y - aaRectangle.Height / 2 - aaHalfPlane.AAHalfPlane.Distance);
                break;
            case AxisDirection.NegativeY:
                relaxDistance = new Vector2(0, aaRectangle.gameObject.transform.globalPosition.Y + aaRectangle.Height / 2 + aaHalfPlane.AAHalfPlane.Distance);
                break;
            default:
                relaxDistance = Vector2.Zero;
                break;
        }

        CollisionLogic.RelaxCollision(aaRectangle.gameObject.GetComponent<PhysicsComponent>(), aaRectangle.isRelaxPosition,
                                      aaHalfPlane.gameObject.GetComponent<PhysicsComponent>(), aaHalfPlane.isRelaxPosition,
                                      relaxDistance);
        // ENERGY EXCHANGE STEP
        // In a collision, energy is exchanged only along the collision normal.
        // For particles this is simply the line between both centers.
        Vector2 collisionNormal = Vector2.Normalize(relaxDistance);
        //ExchangeEnergy(aaRectangle, aaHalfPlane, collisionNormal);
    }
}