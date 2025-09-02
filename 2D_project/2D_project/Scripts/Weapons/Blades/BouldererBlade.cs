internal class BouldererBlade : Weapon_Blade
{
    public BouldererBlade(float chargeTime = 1f) : base(chargeTime)
    {
        // weapon stats
        weaponType = WeaponType.Blade;
        imbuedElement = ImbuedElement.Boulderer;
        damage = 60;
        critRate = 0f;
        critMultiplier = 0f;
        reloadTimer = 0.5f;

        this.chargeTime = chargeTime;
    }

    protected override void OnChargeValueChange()
    {
        base.OnChargeValueChange();
        // charging level changed -> update shield value
        StatChangeFunctions.GainShieldAndHealth(Player.Instance, healthGain: 0, shieldGain: Player.Instance.healthBar.maxShield / 3);
        base.SpecialAttack();
    }
}