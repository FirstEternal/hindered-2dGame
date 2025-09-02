using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

internal class Projectile_AfterAntiVerseHand : Projectile
{
    protected string spriteAnimatedName;
    SpriteAnimated spriteAnimated;
    float currAnimationTimer;
    float animationLength;
    public override void Initialize()
    {
        base.Initialize();
        //element = Weapon.ImbuedElement.Shader;

        // to do assign attributes
        baseLinearSpeed = 500;
        knockBackForce = 0.5f;

        spriteAnimatedName = "AntiVerseHand_Projectile";
    }

    private bool isGrabbingPlayer;
    public override void LoadContent()
    {
        Vector2 scale = new Vector2(2.2f, 2.2f);

        int frameCount = 4;
        animationLength = 1;
        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.weaponBowSpriteSheet,
            sourceRectangles: JSON_Manager.GetProjectileSourceRectangles(spriteAnimatedName, frameCount),
            origins: JSON_Manager.GetProjectileOrigin(spriteAnimatedName, frameCount, scale),
            frameTimers: [int.MaxValue, 0.2f, 0.2f, 0.2f],
            colorTints: [Color.White, Color.White, Color.White, Color.White]
        );

        // add sprite 
        gameObject.AddComponent(spriteAnimated);

        originalWidth = 40;
        originalHeight = 80; // cover slightly less than full

        OBBRectangleCollider collider = new OBBRectangleCollider(originalWidth, originalHeight, isAftermath: false, isRelaxPosition: false);
        gameObject.AddComponent(collider);

        gameObject.tag = GameConstantsAndValues.Tags.EnemyMeleeSpawned.ToString();

        base.LoadContent();
    }

    float originalWidth;
    float originalHeight;
    protected override void UpdateCollider()
    {
        OBBRectangleCollider collider = (OBBRectangleCollider)gameObject.GetComponent<Collider>();
        collider.Width = originalWidth;
        collider.Height = originalHeight * gameObject.transform.localScale.X;
    }

    public override void OnCollisionEnter(Collider collider)
    {
        return;
    }
    public override void OnDetectionRange(Collider collider)
    {
        // already grabbing player return
        if (isGrabbingPlayer) return;

        //base.OnDetectionRange(collider);
        if (collider.gameObject.GetComponent<Player>() != null)
        {
            // stop movement
            isMovable = false;

            isGrabbingPlayer = true;
            Player.Instance.isGravity = false;
            Player.Instance.grabbed = true;
            Player.Instance.Velocity = Vector2.Zero;
            Player.Instance.cumulatedVelocity = Vector2.Zero;
            Player.Instance.SetVisuals(false);
            Player.Instance.gameObject.transform.globalPosition = gameObject.transform.globalPosition;

            spriteAnimated.SetFrame(1);

            currAnimationTimer = animationLength;
            // grab player and drag him with you
        }
    }

    public override void OnEnable()
    {
        isMovable = true;
        spriteAnimated.SetFrame(0);
        isGrabbingPlayer = false;
        base.OnEnable();
    }

    public override void Update(GameTime gameTime)
    {

        spriteAnimated.spriteEffects = (Velocity.X > 0) ?
            Microsoft.Xna.Framework.Graphics.SpriteEffects.None
            : Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically;


        if (isGrabbingPlayer)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currAnimationTimer -= deltaTime;
            if (currAnimationTimer < 0)
            {
                Player player = Player.Instance;
                player.isGravity = true;
                player.grabbed = false;
                deathTimer = 0; //kill the hand
                player.SetVisuals(true);

                // apply damage to player
                StatChangeFunctions.DamageCalculation(entity: player, damage: 30, isCrit: true);
            }
        }

        base.Update(gameTime);
    }
}
