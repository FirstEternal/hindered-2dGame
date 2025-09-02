using MGEngine.Collision.Colliders;

internal class Terrain_InvisibleCollider : PhysicsComponent, ITerrain
{
    int tileWidth;
    int tileHeight;

    public Terrain_InvisibleCollider(int widthInTiles, int heightInTiles, float mass) : base(mass)
    {
        this.tileWidth = widthInTiles * GameConstantsAndValues.SQUARE_TILE_WIDTH;
        this.tileHeight = heightInTiles * GameConstantsAndValues.SQUARE_TILE_WIDTH;
        isMovable = false;
    }

    public override void Initialize()
    {
        OBBRectangleCollider aARectangleCollider = new OBBRectangleCollider(tileWidth, tileHeight, isAftermath: false);
        gameObject.AddComponent(aARectangleCollider);
    }


    public override void OnCollisionEnter(Collider collider)
    {
        //Debug.WriteLine("ok");
        base.OnCollisionEnter(collider);
    }

    public override void OnDetectionRange(Collider collider)
    {
        //Debug.WriteLine("ok1");
        base.OnDetectionRange(collider);
    }
}
