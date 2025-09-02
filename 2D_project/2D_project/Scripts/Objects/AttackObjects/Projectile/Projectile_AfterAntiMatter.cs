using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

internal class Projectile_AfterAntiMatter : Projectile
{
    public override void Initialize()
    {
        base.Initialize();

        element = Weapon.ImbuedElement.Boulderer;


        // to do assign attributes
        baseLinearSpeed = 500;
        knockBackForce = 0.5f;

        //gameObject.transform.localScale = new Vector2(0.5f, 0.5f);

        baseAngularSpeed = 10;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void LoadContent()
    {
        Sprite sprite = new Sprite(
            texture2D: JSON_Manager.weaponBowSpriteSheet,
            colorTint: Color.White
        );

        sprite.sourceRectangle = JSON_Manager.GetWeaponBowSourceRectangle("AntiMatterProjectile");
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
