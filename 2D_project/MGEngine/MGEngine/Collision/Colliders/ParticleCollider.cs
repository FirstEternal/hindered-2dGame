using MGEngine.Collision.Colliders;
public class ParticleCollider(float radius, bool isAftermath, bool isRelaxPosition = true, bool isEnergyExchange = false) : Collider(isAftermath, isRelaxPosition, isEnergyExchange)
{
    public float radius { set; get; } = radius;
}
