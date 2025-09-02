using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

internal class TeleportObject : ObjectComponent, IResettable
{
    private bool originalIsActive;
    protected Vector2 portalLocation;
    protected string portalSpriteName { get; set; }

    public event EventHandler OnPlayerTeleport;

    public TeleportObject(Vector2 portalLocation, string portalSpriteName)
    {
        this.portalSpriteName = portalSpriteName;
        this.portalLocation = portalLocation * GameConstantsAndValues.SQUARE_TILE_WIDTH; // location in tiles
    }
    public void AdjustToLevelStartPosition(Vector2 startPosition)
    {
        portalLocation = portalLocation + startPosition;

        originalIsActive = gameObject.isActive;
    }
    public override void Initialize()
    {
        base.Initialize();

        Sprite portalSprite = new Sprite(JSON_Manager.playerSpriteSheet, colorTint: Color.White);
        portalSprite.sourceRectangle = JSON_Manager.GetPlayerSourceRectangle(tileName: portalSpriteName, 1)[0];
        portalSprite.origin = new Vector2(portalSprite.sourceRectangle.Width / 2, portalSprite.sourceRectangle.Height / 2);

        OBBRectangleCollider oBBRectangleCollider = new OBBRectangleCollider(
            width: portalSprite.sourceRectangle.Width * gameObject.transform.globalScale.X * 0.7f,
            height: portalSprite.sourceRectangle.Height * gameObject.transform.globalScale.Y,
            isAftermath: false
        );

        oBBRectangleCollider.AddTagsToIgnoreList(
            [ GameConstantsAndValues.Tags.Terrain.ToString(),
            GameConstantsAndValues.Tags.Enemy.ToString(),
            GameConstantsAndValues.Tags.EnemySpawned.ToString()]
        );

        gameObject.AddComponent(portalSprite);
        gameObject.AddComponent(oBBRectangleCollider);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void OnDetectionRange(Collider collider)
    {
        base.OnDetectionRange(collider);

        Projectile projectile = collider.gameObject.GetComponent<Projectile>();
        if (projectile != null && !projectile.exitedTeleport)
        {
            // projectiles can only go through a teleport once, while spawned
            projectile.exitedTeleport = true;
            projectile.gameObject.transform.globalPosition = portalLocation;
            Debug.WriteLine("before" + projectile.Velocity);
            projectile.gameObject.transform.globalRotationAngle = gameObject.transform.globalRotationAngle;
            float angle = gameObject.transform.globalRotationAngle; // in radians
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            Vector2 v = projectile.Velocity;
            projectile.Velocity = new Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
            Debug.WriteLine("after" + projectile.Velocity);
        }
        else if (collider.gameObject.tag == GameConstantsAndValues.Tags.Player.ToString() && !Player.Instance.enteredTeleport)
        {
            // teleport player to location
            Player.Instance.Teleport(teleportLocation: portalLocation);
            OnPlayerTeleport?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Reset()
    {
        gameObject.SetActive(originalIsActive);
    }
}
