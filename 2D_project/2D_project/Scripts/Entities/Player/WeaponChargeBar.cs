using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class WeaponChargeBar : ObjectComponent
{
    //Vector2 localPosition;
    public WeaponChargeBar(Player player, GameObject parent, Vector2 localPosition)
    {
        //this.localPosition = localPosition;
        GameObject gameObject = new GameObject();
        gameObject.CreateTransform(localPosition: localPosition, localScale: new Vector2(1.3f, 1.3f));
        gameObject.AddComponent(this);
        parent.AddChild(gameObject, isOverlay: false);

        SpriteAnimated spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangles: [
                JSON_Manager.GetUITile("ChargeBar_0"),
                JSON_Manager.GetUITile("ChargeBar_1"),
                JSON_Manager.GetUITile("ChargeBar_2"),
                ],
            frameTimers: [int.MaxValue, int.MaxValue, int.MaxValue]
        );

        gameObject.AddComponent(spriteAnimated);

        player.OnWeaponChargeChanged -= ShowChargeBar;
        player.OnWeaponChargeChanged += ShowChargeBar;

        player.OnWeaponAttacked -= HideChargeBar;
        player.OnWeaponAttacked += HideChargeBar;

        gameObject.SetActive(false);
    }
    /*
    public override void Update(GameTime gameTime)
    {
        bool isFlipped = Player.Instance.IsSpriteFlipped();
        SpriteAnimated spriteAnimated = gameObject.GetComponent<SpriteAnimated>();
        gameObject.transform.localPosition = new Vector2(isFlipped ? -localPosition.X : localPosition.X, localPosition.Y);
        spriteAnimated.spriteEffects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
    }*/

    private void HideChargeBar(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void ShowChargeBar(object sender, EventArgs e)
    {
        gameObject.SetActive(true);

        SpriteAnimated spriteAnimated = gameObject.GetComponent<SpriteAnimated>();

        switch (Player.Instance.equipedWeapon.currChargingLevel)
        {
            case Weapon.CHARGED_LEVEL1:
                spriteAnimated.SetFrame(0);
                return;
            case Weapon.CHARGED_LEVEL2:
                spriteAnimated.SetFrame(1);
                return;
            case Weapon.CHARGED_LEVEL3:
                spriteAnimated.SetFrame(2);
                return;
        }
    }
}