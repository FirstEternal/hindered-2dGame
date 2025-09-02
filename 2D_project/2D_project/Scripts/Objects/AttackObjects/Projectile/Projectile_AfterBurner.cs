using Microsoft.Xna.Framework;
internal class Projectile_AfterBurner : Projectile
{
    Vector2 originalScale;
    public override void Initialize()
    {
        base.Initialize();
        element = Weapon.ImbuedElement.Burner;
        // to do assign attributes
        baseLinearSpeed = 500;
        knockBackForce = 0.5f;

        originalScale = new Vector2(0.3f, 0.3f);
        gameObject.transform.localScale = originalScale;
    }

    public override void LoadContent()
    {
        Sprite sprite = new Sprite(
            texture2D: JSON_Manager.weaponBowSpriteSheet,
            colorTint: Color.White
        );

        sprite.sourceRectangle = JSON_Manager.GetWeaponBowSourceRectangle("BurnProjectile");
        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);

        // add sprite 
        gameObject.AddComponent(sprite);

        ConvexCollider convexCollider = new ConvexCollider(isScalable: true, bounds: CreateBounds(), isAftermath: true, isRelaxPosition: false);
        gameObject.AddComponent(convexCollider);

        base.LoadContent();
    }

    private ConvexPolygon CreateBounds()
    {
        Sprite sprite = gameObject.GetComponent<Sprite>();
        float width_half = sprite.sourceRectangle.Width / 2;
        float height_half = sprite.sourceRectangle.Height / 2;

        return new ConvexPolygon([
            new Vector2(-width_half, -height_half*0.2f),
            new Vector2(0, -height_half),
            new Vector2(width_half, 0),
            new Vector2(0, height_half),
            new Vector2(0 - width_half, height_half*0.2f),
        ]);
    }
}
