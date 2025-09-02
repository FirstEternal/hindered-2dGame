using MGEngine.Collision.Colliders;

public class ConvexCollider(bool isScalable, ConvexPolygon bounds, bool isAftermath, bool isRelaxPosition = true, bool isEnergyExchange = false) : Collider(isAftermath, isRelaxPosition, isEnergyExchange)
{
    public bool isScalable = isScalable;
    public ConvexPolygon Bounds { get; set; } = bounds;
}