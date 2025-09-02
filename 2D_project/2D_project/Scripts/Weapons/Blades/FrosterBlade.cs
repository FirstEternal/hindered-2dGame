using Microsoft.Xna.Framework;
using System;

internal class FrosterBlade : Weapon_Blade
{
    float currDamageReductionTimer = 0;
    float damageReductionTimer = 0.5f;

    float maxSpriteAnimationFrameIndex;
    public FrosterBlade(float chargeTime = 1f) : base(chargeTime)
    {
        // weapon stats
        weaponType = WeaponType.Blade;
        imbuedElement = ImbuedElement.Froster;
        damage = 35;
        critRate = 0.3f;
        critMultiplier = 1f;
        reloadTimer = 0.5f;

        this.chargeTime = chargeTime;
        damageReductionTimer = reloadTimer * chargeTime;
    }

    protected override void CreateSpecialAttackAnimatedSprite()
    {
        // 1.) create GameObject under instance of Player(SpecialAttackObject)
        base.CreateSpecialAttackAnimatedSprite();

        // 2.) add sprite
        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("SpecialAttack_Froster", 3),
            frameTimers: [reloadTimer / 4, reloadTimer / 4, reloadTimer / 4],
            origins: JSON_Manager.GetPlayerOrigin("SpecialAttack_Froster", 3, gameObject.transform.globalScale),
            layerDepths: [0.1f, 0.1f, 0.1f]
        );

        spriteAnimated.loopEnabled = false;

        spriteAnimated.animationEnded += (object sender, EventArgs e) =>
        {
            specialAttackObject.SetActive(false);
            gameObject.SetActive(true);
            Player.Instance.PerformSpecialAttack(false, true);
        };

        OBBRectangleCollider spriteAnimatedCollider = new OBBRectangleCollider(
            width: spriteAnimated.sourceRectangle.Width * gameObject.transform.globalScale.X,
            height: spriteAnimated.sourceRectangle.Height * gameObject.transform.globalScale.Y,
            isAftermath: false
        );
        /*
        ParticleCollider spriteAnimatedCollider = new ParticleCollider(
            radius: spriteAnimated.sourceRectangle.Width/2 * gameObject.transform.globalScale.X,
            isAftermath: false
        );*/

        // only collision with enemy projectiles
        spriteAnimatedCollider.AddTagsToIgnoreList([
            GameConstantsAndValues.Tags.Player.ToString(),
            GameConstantsAndValues.Tags.PlayerSpawned.ToString(),
            GameConstantsAndValues.Tags.Enemy.ToString(),
            GameConstantsAndValues.Tags.Hidden.ToString(),
            GameConstantsAndValues.Tags.Terrain.ToString(),
            ]
        );

        specialAttackObject.AddComponent(spriteAnimated);
        specialAttackObject.AddComponent(spriteAnimatedCollider);
    }

    protected override void SpecialAttack()
    {
        isPerformingSpecialAttack = true;
        Player.Instance.PerformSpecialAttack(true, true);

        float damageReduction = 0;
        switch (currChargingLevel)
        {
            case CHARGED_LEVEL1:
                damageReduction = 0.3f;
                break;
            case CHARGED_LEVEL2:
                damageReduction = 0.6f;
                break;
            case CHARGED_LEVEL3:
                damageReduction = 1f;
                break;
        }
        Player.Instance.dmgReduction = damageReduction;

        currDamageReductionTimer = damageReductionTimer;
        isPerformingSpecialAttack = true;

        gameObject.SetActive(false);
        spriteAnimated.gameObject.SetActive(true);
        spriteAnimated.SetFrame(0);
        spriteAnimated.ResumeAnimation();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (isPerformingSpecialAttack)
        {
            currDamageReductionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currDamageReductionTimer < 0)
            {
                isPerformingSpecialAttack = false;
            }
        }
    }
}
