using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class DrownerBlade : Weapon_Blade
{
    GameObject colliderObject;
    Vector2 colliderLocalPosition = new Vector2(-100, 0);

    // TileSprite for the sword
    SpriteTiled tileSpriteAnimated;

    public DrownerBlade(float chargeTime = 1f) : base(chargeTime)
    {
        // weapon stats
        weaponType = WeaponType.Blade;
        imbuedElement = ImbuedElement.Drowner;
        damage = 16;
        critRate = 0.5f;
        critMultiplier = 1f;
        reloadTimer = 0.3f;

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

        // 2.) create TileSprite (instead of SpriteAnimated)
        Rectangle[] sourceRectangles = JSON_Manager.GetPlayerSourceRectangle("SpecialAttack_Drowner", 3);

        tileSpriteAnimated = new SpriteTiledAnimated(
            JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("SpecialAttack_Drowner", 3),
            //origins: JSON_Manager.GetPlayerOrigin("SpecialAttack_Drowner", 3);
            layerDepths: [0.1f, 0.1f, 0.1f]
        );
        /* TODO
        spriteAnimated.loopEnabled = false;

        spriteAnimated.animationEnded += (object sender, EventArgs e) =>
        {
            specialAttackObject.SetActive(false);
            gameObject.SetActive(true);
            Player.Instance.PerformSpecialAttack(false, true);
            Player.Instance.isGravity = true;
            Player.Instance.dmgReduction = 0f;
        };*/

        specialAttackObject.tag = GameConstantsAndValues.Tags.PlayerSpawned.ToString();
        specialAttackObject.AddComponent(tileSpriteAnimated);

        colliderObject.tag = GameConstantsAndValues.Tags.PlayerSpawned.ToString();
    }

    protected override void SpecialAttack()
    {
        Player.Instance.PerformSpecialAttack(true, true);
        Player.Instance.dmgReduction = 1f;
        isPerformingSpecialAttack = true;

        tileSpriteAnimated.gameObject.SetActive(true);

        // hide normal weapon sprite
        gameObject.SetActive(false);
        //spriteAnimated.gameObject.SetActive(true);
        /*
        spriteAnimated.gameObject.SetActive(true);
        spriteAnimated.SetFrame(0);
        spriteAnimated.ResumeAnimation();*/

        // direction to mouse
        Vector2 mousePos = MouseGameObject.Singleton.GetMouseWorldSpacePosition();
        Vector2 direction = mousePos - gameObject.transform.globalPosition;
        float length = direction.Length();
        if (direction != Vector2.Zero) direction.Normalize();

        // rotation angle
        float angle = (float)Math.Atan2(direction.Y, direction.X);

        // flip check
        bool isFlipped = direction.X <= 0;
        tileSpriteAnimated.spriteEffects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        // rotation + offset
        specialAttackObject.transform.localRotationAngle = isFlipped ? angle + MathF.PI : angle;
        colliderObject.transform.localPosition = isFlipped ? colliderLocalPosition : -colliderLocalPosition;

        // update target length for sword
        tileSpriteAnimated.targetLength = length;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (specialAttackObject.isActive)
        {
        }
    }

    public override void CancelSpecialAttack()
    {
        isPerformingSpecialAttack = false;
        Player.Instance.PerformSpecialAttack(false, true);

        //spriteAnimated.gameObject.SetActive(false);
        tileSpriteAnimated.gameObject.SetActive(false);
        Player.Instance.isGravity = true;
    }
}
