using MGEngine.Collision.Colliders;

public class HalfPlaneCollider(HalfPlane halfPlane, bool isAftermath, bool isRelaxPosition = true, bool isEnergyExchange = false) : Collider(isAftermath, isRelaxPosition, isEnergyExchange)
{
    public HalfPlane HalfPlane { get; } = halfPlane;
}
