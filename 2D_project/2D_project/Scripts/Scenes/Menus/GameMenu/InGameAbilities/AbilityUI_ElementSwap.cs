using Microsoft.Xna.Framework;
using System;

internal class AbilityUI_ElementSwap(IndexedElement indexedElement, Vector2? artImageScale = null, float artImageRotation = 0) : AbilityUI(artImageScale, artImageRotation)
{
    private IndexedElement indexedElement = indexedElement;

    public override void Update(GameTime gameTime)
    {
        if (!isInitialized) return;

        float fillAmount = Player.Instance.currElementSwapCooldownTimer / Player.Instance.maxElementSwapCooldownTimer;
        //Debug.WriteLine(fillAmount);
        UpdateOuterImage(fillAmount);
    }

    private Weapon.ImbuedElement GetElement(Player player)
    {
        switch (indexedElement)
        {
            case IndexedElement.Prev:
                return player.equipedWeapon.imbuedElement;
            case IndexedElement.Next:
                return ElementDataBase.GetLoadoutElement(player.currElementLoadoutIndex + 1);
            default:
                throw new NullReferenceException();
        }
    }

    protected override Rectangle GetTexture2DSourceRectangle()
    {
        return ElementDataBase.elementSpriteDictionary[GetElement(player: Player.Instance)];
    }
    protected override void SubscribeToEvent()
    {
        Player.Instance.OnElementSwapped -= OnEventUpdateAbilityUI;
        Player.Instance.OnElementSwapped += OnEventUpdateAbilityUI;
    }
}

public enum IndexedElement
{
    Prev,
    Next,
}
