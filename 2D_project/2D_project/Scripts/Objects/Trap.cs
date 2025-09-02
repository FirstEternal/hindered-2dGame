using MGEngine.Collision.Colliders;

public class Trap : ObjectComponent
{
    int damage;
    bool isOnlyPlayerAffected;

    public Trap(int initialDamage, bool isOnlyPlayerAffected)
    {
        damage = initialDamage;
        this.isOnlyPlayerAffected = isOnlyPlayerAffected;
        propagatedCollisionEnabled = false;
    }
    /*
    float currCooldown = 0.5f;
    float maxCooldown = 0.5f;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        currCooldown -= (float)gameTime.ElapsedGameTime.Seconds;
    }*/
    /// <summary>
    /// apply damage calculation to an entity (if isOnlyPlayerAffected, apply to player entity only)
    /// </summary>
    public override void OnCollisionEnter(Collider collider)
    {
        /*
        Debug.WriteLine(currCooldown);
        if (currCooldown > 0) return;

        Entity entity = collider.gameObject.GetComponent<Entity>();
        if (entity != null) {

            Player player = entity as Player;
            if (isOnlyPlayerAffected && player is null) return;
            StatChangeFunctions.DamageCalculation(entity, damage, isCrit: false);
        }
        base.OnCollisionEnter(collider);
        */
        Player player = collider.gameObject.GetComponent<Player>();
        if (player is not null)
        {
            Player.Instance.ApplyDeath();
            return;
        }
        /*
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy is not null)
        {
            StatChangeFunctions.DamageCalculation(enemy, damage: GameConstantsAndValues.instaKillDamage, isCrit: true);
            return;
        }*/
    }
}
