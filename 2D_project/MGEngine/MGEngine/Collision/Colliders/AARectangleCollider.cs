using MGEngine.Collision.Colliders;

public class AARectangleCollider(float width, float height, bool isAftermath, bool isRelaxPosition = true, bool isEnergyExchange = false) : Collider(isAftermath, isRelaxPosition, isEnergyExchange)
{
    public float Width { set; get; } = width;
    public float Height { set; get; } = height;
}