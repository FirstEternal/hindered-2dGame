using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

internal class Projectile_AfterThunderer : Projectile
{
    public override void Initialize()
    {
        base.Initialize();
        element = Weapon.ImbuedElement.Thunderer;

        // to do assign attributes
        baseLinearSpeed = 500;
        knockBackForce = 0.5f;
        gameObject.transform.globalScale = new Vector2(0.8f, 0.8f);
    }

    public override void LoadContent()
    {
        Sprite sprite = new Sprite(
            texture2D: JSON_Manager.weaponBowSpriteSheet,
            colorTint: Color.White
        );

        sprite.sourceRectangle = JSON_Manager.GetWeaponBowSourceRectangle("ThunderProjectile");
        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);

        // add sprite 
        gameObject.AddComponent(sprite);

        //ParticleCollider particleCollider = new ParticleCollider(radius: 10, isAftermath: true, isRelaxPosition: false);
        //gameObject.AddComponent(particleCollider);

        originalWidth = sprite.sourceRectangle.Width;
        originalHeight = sprite.sourceRectangle.Height * 0.5f; // cover slightly less than full

        OBBRectangleCollider collider = new OBBRectangleCollider(originalWidth, originalHeight, isAftermath: true, isRelaxPosition: false);
        gameObject.AddComponent(collider);

        base.LoadContent();
    }

    float originalWidth;
    float originalHeight;
    protected override void UpdateCollider()
    {
        OBBRectangleCollider collider = (OBBRectangleCollider)gameObject.GetComponent<Collider>();
        collider.Width = originalWidth * gameObject.transform.localScale.X;
        collider.Height = originalHeight * gameObject.transform.localScale.X;
    }
}
