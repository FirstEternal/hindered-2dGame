using Microsoft.Xna.Framework;
internal class AbilityUI_Attack(Vector2? artImageScale = null, float artImageRotation = 0) : AbilityUI(artImageScale, artImageRotation)
{
    public override void Update(GameTime gameTime)
    {
        if (!isInitialized) return;
        float fillAmount = Player.Instance.currRealoadTimer / (Player.Instance.equipedWeapon?.reloadTimer ?? 1);
        UpdateOuterImage(fillAmount);
    }

    protected override Rectangle GetTexture2DSourceRectangle()
    {
        switch (Player.Instance?.equipedWeapon?.currChargingLevel)
        {
            case Weapon.CHARGED_LEVEL1:
                return JSON_Manager.GetUITile("Basic_1");
            case Weapon.CHARGED_LEVEL2:
                return JSON_Manager.GetUITile("Basic_2");
            case Weapon.CHARGED_LEVEL3:
                return JSON_Manager.GetUITile("Basic_3");

            default:
                return JSON_Manager.GetUITile("Basic_0");
        }
    }

    protected override void SubscribeToEvent()
    {
        Player.Instance.OnWeaponAttacked -= OnEventUpdateAbilityUI;
        Player.Instance.OnWeaponAttacked += OnEventUpdateAbilityUI;
        Player.Instance.OnWeaponChargeChanged -= OnEventUpdateAbilityUI;
        Player.Instance.OnWeaponChargeChanged += OnEventUpdateAbilityUI;
    }
}
