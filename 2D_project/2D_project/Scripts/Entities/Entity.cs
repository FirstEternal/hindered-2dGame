using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
internal abstract class Entity : PhysicsComponent
{
    public Entity(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = isGravity;
        this.isMovable = isMovable;
    }

    public float dmgReduction;
    public float moveSpeed = 0;
    public float arcMovementDegreeAngle = 0;

    public HealthBar healthBar;
    public EventHandler onDeath;

    private bool isRespawning;

    public Rectangle boundsRectangle;
    private Sprite _boundsSprite;
    public Sprite boundsSprite
    {
        protected set
        {
            _boundsSprite = value;
            boundsRectangle = _boundsSprite.sourceRectangle;
        }
        get { return _boundsSprite; }
    }

    protected void CreateHealthBar(int maxHealth, int maxShield = 0)
    {
        healthBar = new HealthBar(parent: gameObject, maxHealth, maxShield, localPosition: new Vector2(0, -200));
    }

    public override void Update(GameTime gameTime)
    {
        if (!gameObject.isActive) return;
        isRespawning = false;
        UpdateCollider();
    }

    protected virtual void UpdateCollider()
    {

    }

    public virtual void ApplyDeath()
    {
        if (isRespawning) return;
        isRespawning = true;
        onDeath?.Invoke(this, EventArgs.Empty);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        base.OnCollisionEnter(collider);
        if (collider.gameObject.tag.Contains(GameConstantsAndValues.Tags.Terrain.ToString()))
        {
            if (this is BossEnemy) Debug.WriteLine("collided with terrain");
            TerrainCollision(collider);
        }
    }

    protected virtual void TerrainCollision(Collider collider)
    {
        float halfWidth = boundsRectangle.Width / 2 * gameObject.transform.globalScale.X;
        float halfHeight = boundsRectangle.Height / 2 * gameObject.transform.globalScale.Y;
        /*
        float halfTileWidth = GameConstantsAndValues.SQUARE_TILE_WIDTH / 2 * collider.gameObject.transform.globalScale.X;
        float halfTileHeight = GameConstantsAndValues.SQUARE_TILE_WIDTH / 2 * collider.gameObject.transform.globalScale.Y;
        */
        Terrain_Tile terrainTile = collider.gameObject?.parent?.GetComponent<PhysicsComponent>() as Terrain_Tile;
        if (terrainTile is not null)
        {
            terrainTile.SnapToPosition(this.gameObject.transform, halfWidth: halfWidth, halfHeight: halfHeight);
        }

        Velocity.Y = 0;
        //Velocity = new Vector2(Velocity.X, 0);
        cumulatedVelocity.Y = 0;
    }
}
