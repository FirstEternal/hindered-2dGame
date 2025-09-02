using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal abstract class AbilityUI(Vector2? artImageScale = null, float artImageRotation = 0) : ObjectComponent
{
    protected Sprite abilityArtSprite;
    protected Sprite abilityCooldownSprite;
    protected Rectangle abilityCooldownRectangle;

    protected Vector2 artImageScale = artImageScale ?? Vector2.One;
    protected float artImageRotation = artImageRotation;

    private Color cooldownSpriteColor = new Color(0, 0, 0, 150);

    protected bool isInitialized;
    public void InitializeUI()
    {
        // create art layer - > image
        GameObject imageObject = new GameObject();
        imageObject.CreateTransform(localScale: artImageScale, localRotationAngle: artImageRotation);
        Sprite sprite = new Sprite(JSON_Manager.uiSpriteSheet, colorTint: Color.White);
        sprite.sourceRectangle = GetTexture2DSourceRectangle();
        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);
        sprite.layerDepth = 0.02f;
        imageObject.AddComponent(sprite);


        gameObject.AddChild(imageObject, isOverlay: true);
        abilityArtSprite = sprite; // assign as ability art sprite

        // create inner layer -> image
        imageObject = new GameObject();
        imageObject.CreateTransform(localScale: new Vector2(artImageScale.X * 1.4f, artImageScale.Y * 1.4f), localRotationAngle: (float)Math.PI);
        sprite = new Sprite(JSON_Manager.uiSpriteSheet, colorTint: cooldownSpriteColor);
        sprite.sourceRectangle = JSON_Manager.GetUITile("InventoryReloadTile");
        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);
        imageObject.AddComponent(sprite);
        gameObject.AddChild(imageObject, isOverlay: true);
        abilityCooldownSprite = sprite; // assign as ability cooldown sprite
        abilityCooldownRectangle = sprite.sourceRectangle;
        sprite.layerDepth = 0.01f;

        // TODO -> optionally: add text to show the cooldown

        // subscribe to player events
        SubscribeToEvent();
        isInitialized = true;
    }

    protected void UpdateOuterImage(float fillAmount)
    {
        Player player = Player.Instance;

        // show ability cooldown sprite only if ability can not be used
        abilityCooldownSprite.gameObject.SetActive(fillAmount > 0);
        if (fillAmount == 0) return;

        // update source rectangle based on fill value
        abilityCooldownSprite.sourceRectangle = new Rectangle(abilityCooldownSprite.sourceRectangle.X, abilityCooldownSprite.sourceRectangle.Y, abilityCooldownRectangle.Width, (int)(fillAmount * abilityCooldownRectangle.Height));
    }

    protected virtual Rectangle GetTexture2DSourceRectangle()
    {
        return Rectangle.Empty; // should be overriden anway
    }

    protected virtual void SubscribeToEvent()
    {
    }
    protected void OnEventUpdateAbilityUI(object sender, EventArgs e)
    {
        // update art sprite
        abilityArtSprite.sourceRectangle = GetTexture2DSourceRectangle();
    }
}
