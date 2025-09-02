using Microsoft.Xna.Framework;

internal class AbilityUI_SpecialAbility(Vector2? artImageScale = null, float artImageRotation = 0) : AbilityUI(artImageScale, artImageRotation)
{
    public override void Update(GameTime gameTime)
    {
        // TODO -> cool
        if (!isInitialized) return;
        /*
        float fillAmount = Player.Instance.currWeaponSwapCooldownTimer / Player.Instance.maxWeaponSwapCooldownTimer;
        //Debug.WriteLine(fillAmount);
        UpdateOuterImage(fillAmount);*/
    }

    protected override Rectangle GetTexture2DSourceRectangle()
    {
        return JSON_Manager.GetUITile("background");
        //return JSON_Manager.GetUITile("SpecialIcon");
    }

    protected override void SubscribeToEvent()
    {
        /*
        Player.Instance.OnWeaponSwapped -= OnEventUpdateAbilityUI;
        Player.Instance.OnWeaponSwapped += OnEventUpdateAbilityUI;

        Player.Instance.OnElementSwapped -= OnEventUpdateAbilityUI;
        Player.Instance.OnElementSwapped += OnEventUpdateAbilityUI;
        */
    }
}
