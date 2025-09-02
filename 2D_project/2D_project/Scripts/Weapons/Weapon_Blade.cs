using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class Weapon_Blade : Weapon
{
    protected Vector2 specialAttackLocalPosition = Vector2.Zero;
    protected GameObject specialAttackObject;
    protected bool isPerformingSpecialAttack;

    protected bool isAttackBeingAnimated;
    protected float attackAnimationTimer = 0.11f;
    protected float currAttackAnimationTimer = 0;

    protected int currAttackPose;
    Vector2[] attackingPositions = [
        //41, -80
        new Vector2(-40, -125), // idle
        new Vector2(70, -155), // attack pose 1
        new Vector2(130, 55), // attack pose 2
        /*
        new Vector2(-60, -40), // idle
        new Vector2(50, -70), // attack pose 1
        new Vector2(110, 140), // attack pose 2*/
    ];
    Vector2 chargePose = new Vector2(22, -170);
    float chargeRotation = (float)-Math.PI / 2;

    Vector2[] flippedAttackingPositions;
    float[] attackingRotations = [
        3.85f, // 
        5.11f, // 75° 
        6.90f, // 175° 
    ];
    float[] flippedAttackingRotations;

    Sprite WeaponSprite;

    internal Weapon_Blade(float chargeTime = 1f) { }
    internal Weapon_Blade(
        Vector2 localPosition,
        ImbuedElement imbuedElement,        // Element imbued in the weapon 

        // Combat stats
        int damage,                         // Base damage of the weapon
        float critRate,                     // Chance of a critical hit
        float critMultiplier,               // Multiplier for critical damage

        // Projectile properties
        Vector2? projectileBaseScale = null,
        int projectileCount = 1,                // Number of projectiles fired
        float projectileSaparatorDistance = 0,  // Distance between projectiles

        // Reloading properties
        float reloadTimer = 0                // reload time
    )
    {
        this.imbuedElement = imbuedElement;
        this.damage = damage;
        this.critRate = critRate;
        this.weaponType = WeaponType.Blade;
        this.critMultiplier = critMultiplier;
        this.projectileBaseScale = projectileBaseScale ?? Vector2.One;
        this.projectileCount = projectileCount;
        this.projectileSaparatorDistance = projectileSaparatorDistance;
        this.reloadTimer = reloadTimer;
        this.localPosition = localPosition;
        this.chargeTime = 1f;
    }

    // Characteristics
    protected Vector2 projectileBaseScale;
    protected int projectileCount;
    protected float projectileSaparatorDistance;

    private List<Projectile> projectiles = new List<Projectile>();

    public override void SpawnGameObject(string[] weaponSpriteFrameNames, Texture2D weaponSprite)
    {
        base.SpawnGameObject(weaponSpriteFrameNames, weaponSprite);
        gameObject.SetActiveWithParentEnabled = false;

        // calculates sourceRectangles , weaponSpriteFrameNames
        Rectangle[] sourceRectangles = new Rectangle[weaponSpriteFrameNames.Length];

        for (int i = 0; i < weaponSpriteFrameNames.Length; i++)
        {
            Rectangle sourceRectangle = JSON_Manager.GetWeaponBladeSourceRectangle(weaponSpriteFrameNames[i]);

            sourceRectangles[i] = sourceRectangle;
        }

        // first frame lasts 0.5f on all weapon, last frame lasts indefenitely
        float[] frameTimers = [0.4f * reloadTimer, 0.4f * reloadTimer, int.MaxValue];

        // TODO assign arrays
        //SpriteAnimated animatedSprite = new SpriteAnimated(texture2D: weaponSprite, sourceRectangles: sourceRectangles, frameTimers: frameTimers, isAnimationDisabled: true);
        WeaponSprite = new Sprite(weaponSprite, colorTint: Color.White);
        WeaponSprite.sourceRectangle = JSON_Manager.GetWeaponBladeSourceRectangle(weaponSpriteFrameNames[0]);
        WeaponSprite.origin = new Vector2(WeaponSprite.sourceRectangle.Width / 2, WeaponSprite.sourceRectangle.Height / 2);
        WeaponSprite.layerDepth = 0.76f;
        WeaponSprite.spriteEffects = SpriteEffects.FlipHorizontally;

        //gameObject.AddComponent(animatedSprite);
        gameObject.AddComponent(WeaponSprite);

        // add under a player object
        Player.Instance.rightHandSprite.gameObject.AddChild(gameObject);

        currAttackPose = 0;
        gameObject.transform.localRotationAngle = attackingRotations[currAttackPose];
        gameObject.transform.localPosition = attackingPositions[currAttackPose];
        AddCollider();

        flippedAttackingPositions = new Vector2[attackingPositions.Length];
        flippedAttackingRotations = new float[attackingPositions.Length];

        for (int i = 0; i < flippedAttackingPositions.Length; i++)
        {
            flippedAttackingPositions[i].X = -attackingPositions[i].X;
            flippedAttackingPositions[i].Y = attackingPositions[i].Y;

            flippedAttackingRotations[i] = -attackingRotations[i];
        }

        isAttackBeingAnimated = false;

        bool isFlipped = Player.Instance.IsSpriteFlipped();
        WeaponSprite.spriteEffects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        gameObject.transform.localPosition = isFlipped ? flippedAttackingPositions[currAttackPose] : attackingPositions[currAttackPose];
        gameObject.transform.localRotationAngle = isFlipped ? flippedAttackingRotations[currAttackPose] : attackingRotations[currAttackPose];


        CreateSpecialAttackAnimatedSprite();
    }

    protected virtual void CreateSpecialAttackAnimatedSprite()
    {
        specialAttackObject = new GameObject(tag: GameConstantsAndValues.Tags.PlayerSpawned.ToString());
        specialAttackObject.CreateTransform(localPosition: specialAttackLocalPosition);
        specialAttackObject.SetActiveWithParentEnabled = false;

        Player.Instance.SpecialAttackGameObject.AddChild(specialAttackObject);
        specialAttackObject.SetActive(false);
    }

    protected virtual void AddCollider() { }

    public override void OnEnable()
    {
        base.OnEnable();
        isAttackBeingAnimated = false;
        currAttackPose = 0;
        currAttackAnimationTimer = 0;
        currChargingLevel = 0;
    }

    public override void Attack()
    {
        if (currChargingLevel == 0)
        {
            // Normal attack
            isAttackBeingAnimated = true;
            currAttackAnimationTimer = 0;
            currAttackPose = 0;
            //WeaponSprite.layerDepth = 0.49f;
        }
        else
        {
            Debug.WriteLine("Special Attack");
            SpecialAttack();
        }

        currChargingValue = 0;
        currChargingLevel = 0; // reset charging level

        WeaponSprite.layerDepth = 0.76f;
    }
    protected virtual void SpecialAttack()
    {
        isPerformingSpecialAttack = true;
        Player.Instance.PerformSpecialAttack(true, false);
    }

    public virtual void CancelSpecialAttack()
    {
        isPerformingSpecialAttack = false;
        Player.Instance.PerformSpecialAttack(false, true);
    }

    public override void UpdateWeaponChargeValue(GameTime gameTime, Vector2 destinationVector)
    {
        float prevChargingLevel = currChargingLevel;
        base.UpdateWeaponChargeValue(gameTime, destinationVector);
        if (prevChargingLevel != currChargingLevel)
        {
            OnChargeValueChange();
        }
    }

    protected virtual void OnChargeValueChange()
    {
        WeaponSprite.layerDepth = (currChargingLevel > 0) ? 0.25f : WeaponSprite.layerDepth = 0.76f;

        Player.Instance.leftHandSprite.animationEnabled = false;
        Player.Instance.leftHandSprite.SetFrame(1);

        Player.Instance.rightHandSprite.animationEnabled = false;
        Player.Instance.rightHandSprite.SetFrame(2);
    }

    // update weapon direction
    public override void Update(GameTime gameTime)
    {
        bool isFlipped = Player.Instance.IsSpriteFlipped();
        WeaponSprite.spriteEffects = isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        if (isAttackBeingAnimated)
        {
            currAttackAnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currAttackAnimationTimer >= attackAnimationTimer)
            {
                currAttackAnimationTimer = 0;
                if (++currAttackPose >= attackingPositions.Length)
                {
                    currAttackPose = 0;
                    isAttackBeingAnimated = false;
                }
            }
        }

        if (currChargingLevel == 0)
        {
            gameObject.transform.localPosition = isFlipped ? flippedAttackingPositions[currAttackPose] : attackingPositions[currAttackPose];
            gameObject.transform.localRotationAngle = isFlipped ? flippedAttackingRotations[currAttackPose] : attackingRotations[currAttackPose];
        }
        else
        {
            gameObject.transform.localPosition = isFlipped ? new Vector2(-chargePose.X, chargePose.Y) : chargePose;
            gameObject.transform.localRotationAngle = isFlipped ? -chargeRotation : chargeRotation;
        }
        /*
        if (isPerformingSpecialAttack)
        {
            switch (currChargingLevel)
            {
                case CHARGED_LEVEL1:
                    if (spriteAnimated.currFrameIndex == 0) isPerformingSpecialAttack = false;
                    return;
                case CHARGED_LEVEL2:
                    if (spriteAnimated.currFrameIndex == 1) isPerformingSpecialAttack = false;
                    return;
                case CHARGED_LEVEL3:
                    if (spriteAnimated.currFrameIndex == 2) isPerformingSpecialAttack = false;
                    return;
            }
            
            if(isPerformingSpecialAttack == false)
            {
                Player.Instance.PerformSpecialAttack(false);
                //specialAttackObject.SetActive(false);
                //WeaponSprite.layerDepth = 0.76f;
            }
            return;
        }*/
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);
    }
}