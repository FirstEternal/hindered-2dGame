using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


internal class ThundererBlade : Weapon_Blade
{
    float dashSpeed;
    float currDashDuration;

    GameObject colliderObject;
    Vector2 colliderLocalPosition = new Vector2(-100, 0);
    public ThundererBlade(float chargeTime = 1f) : base(chargeTime)
    {
        // weapon stats
        weaponType = WeaponType.Blade;
        imbuedElement = ImbuedElement.Thunderer;
        damage = 16;
        critRate = 0.5f;
        critMultiplier = 1f;
        reloadTimer = 0.4f;

        projectileSaparatorDistance = 20;
        projectileCount = 2;

        this.chargeTime = chargeTime;
    }

    protected override void CreateSpecialAttackAnimatedSprite()
    {
        // 1.) create GameObject under instance of Player(SpecialAttackObject)
        base.CreateSpecialAttackAnimatedSprite();

        colliderObject = new GameObject();
        colliderObject.CreateTransform(localPosition: colliderLocalPosition);

        specialAttackObject.AddChild(colliderObject);

        // 2.) add sprite
        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("SpecialAttack_Thunderer", 4),
            frameTimers: [0.1f, 0.1f, 0.1f, 0.1f],
            origins: JSON_Manager.GetPlayerOrigin("SpecialAttack_Thunderer", 4, gameObject.transform.globalScale),
            layerDepths: [0.1f, 0.1f, 0.1f, 0.1f]
        );

        spriteAnimated.loopEnabled = false;

        spriteAnimated.animationEnded += (object sender, EventArgs e) =>
        {
            specialAttackObject.SetActive(false);
            gameObject.SetActive(true);
            Player.Instance.PerformSpecialAttack(false, true);
            Player.Instance.isGravity = true;
            Player.Instance.dmgReduction = 0f;
        };

        OBBRectangleCollider spriteAnimatedCollider = new OBBRectangleCollider(
            width: spriteAnimated.sourceRectangle.Width * gameObject.transform.globalScale.X,
            height: spriteAnimated.sourceRectangle.Height * gameObject.transform.globalScale.Y * 0.8f,
            isAftermath: true
        );

        specialAttackObject.tag = GameConstantsAndValues.Tags.PlayerSpawned.ToString();
        specialAttackObject.AddComponent(spriteAnimated);


        colliderObject.tag = GameConstantsAndValues.Tags.PlayerSpawned.ToString();
        colliderObject.AddComponent(spriteAnimatedCollider);
    }

    protected override void SpecialAttack()
    {
        Player.Instance.PerformSpecialAttack(true, true);
        Player.Instance.dmgReduction = 1f;
        isPerformingSpecialAttack = true;
        dashSpeed = 0.5f * currChargingLevel;

        gameObject.SetActive(false);
        spriteAnimated.gameObject.SetActive(true);
        spriteAnimated.SetFrame(0);
        spriteAnimated.ResumeAnimation();

        // define translation
        Vector2 direction = MouseGameObject.Singleton.GetMouseWorldSpacePosition() - gameObject.transform.globalPosition;
        if (direction != Vector2.Zero) direction.Normalize();

        // Check if sprite needs to be flipped
        bool isFlipped = direction.X <= 0;
        specialAttackObject.GetComponent<Sprite>().spriteEffects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        // Apply rotation
        colliderObject.transform.localPosition = isFlipped ? colliderLocalPosition : -colliderLocalPosition;

        currDashDuration = 0.6f;
        Player.Instance.BeginKnockBack(pushPower: dashSpeed, direction: direction, knockDuration: currDashDuration);
        Player.Instance.isGravity = false;

        return;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (specialAttackObject.isActive)
        {
            currDashDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currDashDuration < 0)
            {
                Player.Instance.Velocity = Vector2.Zero;
            }
        }
    }

    public override void CancelSpecialAttack()
    {
        isPerformingSpecialAttack = false;
        Player.Instance.PerformSpecialAttack(false, true);

        spriteAnimated.gameObject.SetActive(false);
        Player.Instance.isGravity = true;
    }
}