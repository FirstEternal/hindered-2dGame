using Microsoft.Xna.Framework;

internal class AbilityUI_WeaponSwap(Vector2? artImageScale = null, float artImageRotation = 0) : AbilityUI(artImageScale, artImageRotation)
{
    public override void Update(GameTime gameTime)
    {
        if (!isInitialized) return;
        float fillAmount = Player.Instance.currWeaponSwapCooldownTimer / Player.Instance.maxWeaponSwapCooldownTimer;
        //Debug.WriteLine(fillAmount);
        UpdateOuterImage(fillAmount);
    }

    protected override Rectangle GetTexture2DSourceRectangle()
    {
        return (Player.Instance.equipedWeapon?.linkedWeapon is not null)
            ? WeaponDataBase.WeaponIconDictionary[Player.Instance.equipedWeapon.linkedWeapon.weaponType]
            : JSON_Manager.GetUITile("BowIcon"); ;
    }

    protected override void SubscribeToEvent()
    {
        Player.Instance.OnWeaponSwapped -= OnEventUpdateAbilityUI;
        Player.Instance.OnWeaponSwapped += OnEventUpdateAbilityUI;

        Player.Instance.OnElementSwapped -= OnEventUpdateAbilityUI;
        Player.Instance.OnElementSwapped += OnEventUpdateAbilityUI;
    }
}
