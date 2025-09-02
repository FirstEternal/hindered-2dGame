using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

internal class Melee : ObjectComponent
{
    // NEEDED
    protected int width;
    protected int height;

    protected float knockDuration = 0;
    public Entity MeleeOwner { get; private set; }

    protected bool isAlive = true;
    protected float visibleDuration = 0.5f;

    protected int dmg;
    protected Vector2 knockDirection;
    protected float pushPower = 0;


    public override void LoadContent()
    {
        OBBRectangleCollider obbCollider = new OBBRectangleCollider(width: width, height: height, isAftermath: false, isRelaxPosition: false);
        gameObject.AddComponent(obbCollider);
        base.LoadContent();
        gameObject.SetActive(false);
    }

    public override void Update(GameTime gameTime)
    {
        visibleDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (visibleDuration <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void Spawn(Game game, float fadeDuration, Vector2 spawnPosition, int width, int height, Entity meleeOwner, int dmg, Vector2 knockDirection, float pushPower)
    {
        if (meleeOwner is null) return;

        this.MeleeOwner = meleeOwner;
        bool isPlayerSpawned = meleeOwner != null && meleeOwner.GetType() == typeof(Player);
        gameObject.tag = isPlayerSpawned ? GameConstantsAndValues.Tags.PlayerMeleeSpawned.ToString() : GameConstantsAndValues.Tags.EnemyMeleeSpawned.ToString();

        gameObject.transform.globalPosition = spawnPosition;

        this.width = width;
        this.height = height;
        this.visibleDuration = fadeDuration;
        this.dmg = dmg;
        this.knockDirection = knockDirection;
        this.pushPower = pushPower;

        SceneManager.Instance.RemoveGameObjectFromScene(gameObject); // remove from current scene
        SceneManager.Instance.activeScene.AddGameObjectToScene(gameObject, isOverlay: false); // add object to active scene

        gameObject.SetActive(true);

        UpdateCollider();
    }
    protected void UpdateCollider()
    {
        OBBRectangleCollider obbCollider = (OBBRectangleCollider)gameObject.GetComponent<Collider>();
        obbCollider.Width = width * gameObject.transform.localScale.X;
        obbCollider.Height = height * gameObject.transform.localScale.Y;
    }

    public override void OnCollisionEnter(Collider collider)
    {
        return;
    }

    public override void OnDetectionRange(Collider collider)
    {
        if (!isAlive) return;
        // apply knockback in opposite direction
        Player player = collider.gameObject.GetComponent<Player>();
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (player is not null)
        {
            // apply damage
            StatChangeFunctions.DamageCalculation(player, dmg, isCrit: true);
            // apply knock back
            if (pushPower > 0) player.BeginKnockBack(pushPower, knockDirection, knockDuration: 0.4f);

            gameObject.SetActive(false);
        }
        else if (enemy is not null)
        {
            // apply damage
            StatChangeFunctions.EnemyDamageCalculation(Player.Instance.equipedWeapon, enemy);

            // apply knock back
            if (enemy is not BossEnemy)
            {
                // to create player.BeginKnockBack(pushPower, -Player.Instance.Velocity, knockDuration: 0.3f);
            }

            gameObject.SetActive(false);
        }
    }
}
