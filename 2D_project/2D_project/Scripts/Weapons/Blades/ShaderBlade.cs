internal class ShaderBlade : Weapon_Blade
{
    public ShaderBlade(float chargeTime = 1f) : base(chargeTime)
    {
        // weapon stats
        weaponType = WeaponType.Blade;
        imbuedElement = ImbuedElement.Shader;
        damage = 40;
        critRate = 0.25f;
        critMultiplier = 3f;
        reloadTimer = 0.5f;

        this.chargeTime = chargeTime;
    }
    /*
    protected override void CreateSpecialAttackAnimatedSprite()
    {
        base.CreateSpecialAttackAnimatedSprite();

        spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("SpecialAttack_Shader", 1),
            frameTimers: [int.MaxValue]
        );

        specialAttackObject.AddComponent(spriteAnimated);
    }*/

    protected override void SpecialAttack()
    {
        base.SpecialAttack();

        Player.Instance.playerShadow.SpawnShadow(
            spawnPosition: Player.Instance.gameObject.transform.globalPosition,
            chargedLevel: currChargingLevel
        );

        Player.Instance.PerformSpecialAttack(false, false);
        specialAttackObject.SetActive(false);
    }
}