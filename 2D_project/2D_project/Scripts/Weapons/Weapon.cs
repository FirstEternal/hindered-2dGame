using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
internal class Weapon : ObjectComponent
{
    protected Vector2 localPosition;

    public WeaponType weaponType;
    public ImbuedElement imbuedElement;
    public Weapon linkedWeapon;

    // Damage
    public int damage;
    public float critRate;
    public float critMultiplier;

    public float reloadTimer;
    public float chargeTime; // 0-100% to initiate stronger hit

    protected float currChargingValue;
    public float currChargingLevel { get; protected set; }
    public const float CHARGED_LEVEL1 = 0.5f;
    public const float CHARGED_LEVEL2 = 1f;
    public const float CHARGED_LEVEL3 = 1.5f;

    protected SpriteAnimated spriteAnimated;

    public virtual void SpawnGameObject(string[] weaponSpriteFrameNames, Texture2D weaponSprite)
    {

        GameObject weaponObject = new GameObject(100);
        weaponObject.AddComponent(this);
        weaponObject.CreateTransform(localPosition: localPosition);
        gameObject.tag = GameConstantsAndValues.Tags.PlayerSpawned.ToString();
    }

    public enum ImbuedElement
    {
        None,
        Boulderer,
        Burner,
        Drowner,
        Froster,
        Grasser,
        Shader,
        Thunderer,
    }
    public enum WeaponType
    {
        Blade,
        Bow,
    }

    public virtual void BeginCharging(Vector2 destinationVector)
    {
        currChargingLevel = 0;
    }

    public virtual void UpdateWeaponChargeValue(GameTime gameTime, Vector2 destinationVector) { UpdateChargeValue(gameTime); }
    protected void UpdateChargeValue(GameTime gameTime)
    {
        if (currChargingValue == CHARGED_LEVEL3) return;

        float valueChange = (float)gameTime.ElapsedGameTime.TotalSeconds / chargeTime;
        currChargingValue += valueChange;

        // update scale when reaching 0.5f, 1.0f, 1.5f
        if (currChargingLevel != CHARGED_LEVEL1 && currChargingValue < CHARGED_LEVEL1 && currChargingValue + valueChange >= CHARGED_LEVEL1)
        {
            currChargingLevel = CHARGED_LEVEL1;
        }
        // update scale when reaching 0.5f, 1.0f, 1.5f
        else if (currChargingLevel != CHARGED_LEVEL2 && currChargingValue < CHARGED_LEVEL2 && currChargingValue + valueChange >= CHARGED_LEVEL2)
        {
            currChargingLevel = CHARGED_LEVEL2;
            if (weaponType == WeaponType.Bow) spriteAnimated?.DisplayNextFrame();
        }
        else if (currChargingLevel != CHARGED_LEVEL3 && currChargingValue >= CHARGED_LEVEL3)
        {
            currChargingLevel = CHARGED_LEVEL3;
            if (weaponType == WeaponType.Bow) spriteAnimated?.DisplayNextFrame();
            if (weaponType == WeaponType.Bow) spriteAnimated?.PauseAnimation();
        }
    }
    public virtual void Attack() { }
}