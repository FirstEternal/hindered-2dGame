using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

internal class Projectile_AfterDrowner : Projectile
{

    public override void Initialize()
    {
        base.Initialize();
        element = Weapon.ImbuedElement.Drowner;
        // to do assign attributes
        base.Initialize();
        // to do assign attributes
        baseLinearSpeed = 500;
        knockBackForce = 0.5f;

        //gameObject.transform.globalScale = new Vector2(0.1f, 0.1f);

        baseAngularSpeed = 10;
    }
    public override void LoadContent()
    {
        Sprite sprite = new Sprite(
            texture2D: JSON_Manager.weaponBowSpriteSheet,
            colorTint: Color.White
        );

        sprite.sourceRectangle = JSON_Manager.GetWeaponBowSourceRectangle("DrownProjectile");
        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);

        // add sprite 
        gameObject.AddComponent(sprite);

        originalRadius = sprite.sourceRectangle.Width / 2;
        ParticleCollider particleCollider = new ParticleCollider(radius: 10, isAftermath: true, isRelaxPosition: false);
        gameObject.AddComponent(particleCollider);

        base.LoadContent();
    }

    float originalRadius;
    protected override void UpdateCollider()
    {
        ParticleCollider particleCollider = (ParticleCollider)gameObject.GetComponent<Collider>();
        particleCollider.radius = originalRadius * gameObject.transform.localScale.X;
    }
}
