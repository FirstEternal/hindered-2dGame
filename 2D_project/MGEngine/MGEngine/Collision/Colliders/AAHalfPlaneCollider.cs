using MGEngine.Collision.Colliders;

public class AAHalfPlaneCollider(bool isAftermath, AAHalfPlane aaHalfPlane, bool isRelaxPosition = true, bool isEnergyExchange = false) : Collider(isAftermath, isRelaxPosition, isEnergyExchange)
{
    public AAHalfPlane AAHalfPlane { get; } = aaHalfPlane;
}