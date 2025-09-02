using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
internal class Terrain_Tile : PhysicsComponent, ITerrain, IResettable
{
    public EventHandler onCollision;
    public bool topSnapEnabled = false;
    public bool leftSnapEnabled = false;
    public bool rightSnapEnabled = false;
    public bool bottomSnapEnabled = false;

    private bool isColliderFlipped = false;
    private bool breakable = false;
    public Terrain_Tile(string tileName, bool shouldSpawnCollider = false, bool isHorizontallyFlipped = false, float mass = int.MaxValue, bool isGravity = false, bool isMovable = false) : base(mass, isGravity: isGravity, isMovable: isMovable)
    {
        if (tileName.Contains("Wood")) breakable = true;// hardcoded
        this.tileName = tileName;
        this.isHorizontallyFlipped = isHorizontallyFlipped;
        this.shouldSpawnCollider = shouldSpawnCollider;
    }
    public Terrain_Tile(Rectangle tileSourceRectangle, bool shouldSpawnCollider = false, bool isHorizontallyFlipped = false, float mass = int.MaxValue, bool isGravity = false, bool isMovable = false) : base(mass, isGravity: isGravity, isMovable: isMovable)
    {
        this.tileSourceRectangle = tileSourceRectangle;
        this.isHorizontallyFlipped = isHorizontallyFlipped;
        this.shouldSpawnCollider = shouldSpawnCollider;
    }

    readonly bool isHorizontallyFlipped;

    private readonly string tileName; // if making public -> also get the tile name from tile data
    Rectangle tileSourceRectangle;
    private readonly bool shouldSpawnCollider;
    private bool isStairCase;

    public static void CreateTile(PhysicsComponent parentPhysicsComponent, string tileName, Vector2 localPosition, Vector2 localScale, float localRotation, bool isHorizontallyFlipped = false,
        bool shouldSpawnCollider = false, bool topSnapEnabled = false, bool bottomSnapEnabled = false, bool rightSnapEnabled = false, bool leftSnapEnabled = false, bool isTrap = false, bool isStairCase = false)
    {
        GameObject terrain = new GameObject(tag: parentPhysicsComponent.gameObject.tag);
        Terrain_Tile tile = new Terrain_Tile(tileName, shouldSpawnCollider: shouldSpawnCollider, isHorizontallyFlipped: isHorizontallyFlipped);

        // GLOBAL POSITION
        terrain.CreateTransform(localPosition: localPosition, localScale: localScale, localRotationAngle: localRotation);
        terrain.AddComponent(tile);

        parentPhysicsComponent.gameObject.AddChild(terrain);

        tile.isMovable = parentPhysicsComponent.isMovable;


        tile.bottomSnapEnabled = bottomSnapEnabled;
        tile.topSnapEnabled = topSnapEnabled;
        tile.leftSnapEnabled = leftSnapEnabled;
        tile.rightSnapEnabled = rightSnapEnabled;

        if (isTrap) terrain.AddComponent(new Trap(initialDamage: 10, isOnlyPlayerAffected: true));

        //  rotate accordingly
        tile.isStairCase = isStairCase;
    }

    public override void Initialize()
    {
        if (tileSourceRectangle == Rectangle.Empty)
        {
            tileSourceRectangle = JSON_Manager.GetTileSourceRectangle(tileName);
        }

        Sprite sprite = new Sprite(texture2D: JSON_Manager.tileSpriteSheet, colorTint: Color.White);
        sprite.sourceRectangle = tileSourceRectangle;

        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);
        sprite.spriteEffects = isHorizontallyFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        sprite.layerDepth = 0.1f;
        //sprite.origin = origin;
        gameObject.AddComponent(sprite);

        // create accurate collider type
        if (shouldSpawnCollider)
        {
            CreateAccurateCollider(sprite.texture2D);
        }

        originalIsActive = gameObject.isActive;
    }

    private bool originalIsActive;
    public void Reset()
    {
        gameObject.SetActive(originalIsActive);
    }

    private void FlipCollider(AARectangleCollider collider)
    {
        float rotationAngle = gameObject.transform.globalRotationAngle;  // Assuming this is the rotation in radians

        // Normalize the rotation angle to be within [0, 2*pi)
        rotationAngle = rotationAngle % (2 * MathF.PI);

        // Handle negative angles by wrapping them into the positive range
        if (rotationAngle < 0)
        {
            rotationAngle += 2 * MathF.PI;
        }

        // Convert from radians to fractions of pi to easily compare
        const float piOver2 = MathF.PI / 2;
        const float pi3Over4 = 3 * MathF.PI / 4;

        float epsilon = 0.0001f;  // Small tolerance for floating point comparison

        // Check if rotationAngle is close to any of the specified values and swap width and height
        if (MathF.Abs(rotationAngle - piOver2) < epsilon || MathF.Abs(rotationAngle - pi3Over4) < epsilon ||
            MathF.Abs(rotationAngle - (MathF.PI + piOver2)) < epsilon)
        {
            float height = collider.Width;
            collider.Width = collider.Height;
            collider.Height = height;
            if (!isMovable) isColliderFlipped = true;
        }
    }


    private void CreateAccurateCollider(Texture2D texture2D)
    {
        // loop through pixels in the sprite and correctly assign collider
        // 1.) convex collider
        // 2.) convex collider
        // 3.) convex collider
        //SpriteColliderGenerator.Instance.CreateCollider(texture2D);
        // for now not necessary
        GameObject colliderObject = new GameObject(tag: gameObject.tag, id: gameObject.id);
        colliderObject.CreateTransform();
        float width = GameConstantsAndValues.SQUARE_TILE_WIDTH * gameObject.transform.globalScale.X;
        float height = GameConstantsAndValues.SQUARE_TILE_WIDTH * gameObject.transform.globalScale.Y;

        // find rotation

        OBBRectangleCollider collider = new OBBRectangleCollider(width, height, isAftermath: true, isRelaxPosition: false);
        colliderObject.AddComponent(collider);
        collider.ignoreTagList.Add(GameConstantsAndValues.Tags.Terrain.ToString());
        gameObject.AddChild(colliderObject);

        FlipCollider(collider); // flip w, h if 90°, 270°

        collider.gameObject.transform.localRotationAngle = -gameObject.transform.globalRotationAngle;
    }

    public override void OnCollisionEnter(Collider collider)
    {
        if (breakable) gameObject.SetActive(false);
        onCollision?.Invoke(this, new CollisionEventArgs(collider, collider.isAftermath));
    }

    public void SnapToPosition(Transform transform, float halfWidth, float halfHeight)
    {
        float halfTileWidth = GameConstantsAndValues.SQUARE_TILE_WIDTH / 2 * gameObject.transform.globalScale.X;
        float halfTileHeight = GameConstantsAndValues.SQUARE_TILE_WIDTH / 2 * gameObject.transform.globalScale.Y;

        if (isColliderFlipped)
        {
            // flip x and y sizes
            float _ = halfTileWidth;
            halfTileWidth = halfTileHeight;
            halfTileHeight = _;
        }


        if (isStairCase && transform.gameObject.GetComponent<Player>() is not null)
        {
            StairCaseSnapToPosition(transform, halfTileWidth, halfTileHeight, halfHeight, halfWidth);
            return;
        }

        NormalSnapToPosition(transform, halfTileWidth, halfTileHeight, halfHeight, halfWidth);
    }

    private void NormalSnapToPosition(Transform transform, float halfTileWidth, float halfTileHeight, float halfHeight, float halfWidth)
    {
        // snap to top 
        if (topSnapEnabled) transform.globalPosition.Y = gameObject.transform.globalPosition.Y - halfTileHeight - halfHeight;

        // snap to left
        if (leftSnapEnabled) transform.globalPosition.X = gameObject.transform.globalPosition.X - halfTileWidth - halfWidth;

        // snap to right 
        if (rightSnapEnabled) transform.globalPosition.X = gameObject.transform.globalPosition.X + halfTileWidth + halfWidth;

        // snap to bottom
        if (bottomSnapEnabled) transform.globalPosition.Y = gameObject.transform.globalPosition.Y + halfTileHeight + halfHeight;
    }

    private void StairCaseSnapToPosition(Transform playerTransform, float halfTileWidth, float halfTileHeight, float halfHeight, float halfWidth)
    {
        // player is not moving
        if (Player.Instance.Velocity.X == 0)
        {
            // snap to top
            playerTransform.globalPosition.Y = gameObject.transform.globalPosition.Y - halfTileHeight - halfHeight;

            return;
        }

        // if going down snap to bottom + snap to right/left
        if (isHorizontallyFlipped && Player.Instance.Velocity.X < 0)
        {
            // going down left -> snap to bottom + left
            playerTransform.globalPosition.X = gameObject.transform.globalPosition.X - halfTileWidth - halfWidth;
            playerTransform.globalPosition.Y = gameObject.transform.globalPosition.Y + halfTileHeight - halfHeight;
            return;
        }
        else if (!isHorizontallyFlipped && Player.Instance.Velocity.X > 0)
        {
            // going down right -> snap to bottom + right
            if (Player.Instance.Velocity.X > 0)
            {
                playerTransform.globalPosition.X = gameObject.transform.globalPosition.X + halfTileWidth + halfWidth;
                playerTransform.globalPosition.Y = gameObject.transform.globalPosition.Y + halfTileHeight - halfHeight;
            }
            return;
        }

        // going up -> snap to top
        //playerTransform.globalPosition.Y = gameObject.transform.globalPosition.Y - halfTileHeight - halfHeight;
        playerTransform.globalPosition.Y = gameObject.transform.globalPosition.Y - halfTileHeight - halfHeight - 8;
        if (Player.Instance.Velocity.X > 0)
        {
            playerTransform.globalPosition.X = gameObject.transform.globalPosition.X - halfTileWidth + halfWidth * 0.01f;
        }
        else if (Player.Instance.Velocity.X < 0)
        {
            playerTransform.globalPosition.X = gameObject.transform.globalPosition.X + halfTileWidth - halfWidth * 0.01f;
        }
    }
}
