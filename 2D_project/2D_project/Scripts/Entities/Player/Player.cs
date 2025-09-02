using GamePlatformer;
using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

internal class Player : Entity
{
    public static Player Instance { get; private set; }
    public Player(float mass, Vector2? velocity = null, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true) : base(mass, velocity, acceleration, isGravity, isMovable)
    {
        this.Mass = mass;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Velocity = velocity ?? Vector2.Zero;
        this.Acceleration = acceleration ?? Vector2.Zero; // enakomerni pospešek
        this.isGravity = isGravity;
        this.isMovable = isMovable;
    }

    private bool cheatEnabled = false;

    public Collider playerCollider;
    public PlayerShadow playerShadow;
    Scene scene;

    private bool visualsDisabled;
    private float deathByGravityHeight = 500;

    public bool grabbed;

    public PlayerLoadout loadout = new PlayerLoadout();

    // for testing
    public void EnableCheatMode()
    {
        moveSpeed = 1000;
        //moveSpeed = 5000;
        isGravity = false;
        cheatEnabled = true;

        healthBar.maxHealth = int.MaxValue;
        healthBar.currHealth = int.MaxValue;
    }

    public enum PlayerState
    {
        Idle,
        Moving,
    }

    public bool isAttacking;
    public bool isReloading;
    public bool isMoving;

    //private PlayerState playerState;

    public Weapon equipedWeapon { get; private set; }

    private bool isChargingAttack;

    public int currElementLoadoutIndex { get; private set; }

    private bool isInput;
    private float untilIdleTime = 5;
    private bool isIdle;

    public float currRealoadTimer { get; private set; } = 0;
    public float currElementSwapCooldownTimer { get; private set; } = 0;
    public readonly float maxElementSwapCooldownTimer = 1;

    public float currWeaponSwapCooldownTimer { get; private set; } = 0;
    public readonly float maxWeaponSwapCooldownTimer = 0.4f;

    public EventHandler OnWeaponAttacked;
    public EventHandler OnWeaponChargeChanged;
    public EventHandler OnWeaponSwapped;
    public EventHandler OnElementSwapped;


    private int jumpCounter = 0;
    private readonly int maxJumpCounter = 2;
    private float timeSinceJump = 0;
    private float jumpSpeed = 200;

    private float _plaftormJumpVelocityY;
    public float plaftormJumpVelocityY
    {
        get
        {
            return _plaftormJumpVelocityY;
        }
        set
        {
            // can only jump up
            _plaftormJumpVelocityY = value < 0 ? value : 0;
        }
    }


    int movingSpriteCount = 3;
    SpriteAnimated movingSprite;
    SpriteAnimated jumpSprite;
    public SpriteAnimated leftHandSprite;
    //Vector2 leftHandLocalPosition;
    public SpriteAnimated rightHandSprite;
    //Vector2 rightHandLocalPosition;
    public GameObject SpecialAttackGameObject { get; private set; }
    bool isPerformingSpecialAttack;
    //Dictionary<Weapon_Blade, SpriteAnimated> specialAttackObjectDictionary = new Dictionary<Weapon_Blade, SpriteAnimated>();



    private Vector2 baseKnockBackVelocity = new Vector2(500, 500);
    private Vector2 knockBackVelocity;
    public bool IsKnockedBack;
    private float knockBackDuration;
    private float currKnockBackImunityTimer = 0;
    private float knockBackImunityTimer = 0.2f;


    public bool enteredTeleport;
    private float currTeleportDurationTimer = 0;
    private float teleportDurationTimer = 0.5f;
    private float currTeleportImunityTimer = 0;

    public override void Initialize()
    {
        if (Instance is not null) return;

        Instance = this;

        base.Initialize();
    }

    private void SetupColliders()
    {
        //isInitialized = true;
        // assign different sprite sheets
        Game2DPlatformer game2DPlatformer = Game2DPlatformer.Instance;

        // CREATE ANIMATED SPRITES
        int currID = gameObject.id;

        // 1.) moving sprite
        GameObject movingSpriteObject = new GameObject(id: ++currID);
        movingSpriteObject.CreateTransform();

        Rectangle[] sourceRectangles = JSON_Manager.GetPlayerSourceRectangle("Pose", movingSpriteCount);

        // create sprite component
        movingSprite = new SpriteAnimated(
                texture2D: JSON_Manager.playerSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: [.1f, .1f, .1f],
                colorTints: [Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetPlayerOrigin("Pose", movingSpriteCount, gameObject.transform.globalScale),
                layerDepths: [0.6f, 0.6f, 0.6f]
        );

        gameObject.AddChild(movingSpriteObject);

        movingSpriteObject.AddComponent(movingSprite);

        // 2.) jump sprite
        GameObject jumpSpriteObject = new GameObject(id: ++currID);
        jumpSpriteObject.CreateTransform();

        Rectangle[] sourceRectangles1 = JSON_Manager.GetPlayerSourceRectangle("JumpPose", movingSpriteCount);

        // create sprite component
        jumpSprite = new SpriteAnimated(
                texture2D: JSON_Manager.playerSpriteSheet,
                sourceRectangles: sourceRectangles1,
                frameTimers: [.1f, .1f, int.MaxValue],
                colorTints: [Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetPlayerOrigin("JumpPose", movingSpriteCount, gameObject.transform.globalScale),
                layerDepths: [0.6f, 0.6f, 0.6f]
        );

        // add sprite 
        gameObject.AddChild(jumpSpriteObject);
        jumpSpriteObject.AddComponent(jumpSprite);
        jumpSprite.gameObject.SetActive(false);


        // 3.) hand sprites
        // left hand
        GameObject leftHandObject = new GameObject(id: ++currID);
        //leftHandLocalPosition = new Vector2(-135 * gameObject.transform.globalScale.X, -420 * gameObject.transform.globalScale.Y);
        //leftHandObject.CreateTransform(localPosition: leftHandLocalPosition);
        leftHandObject.CreateTransform();

        gameObject.AddChild(leftHandObject);
        Rectangle[] sourceRectangles2 = JSON_Manager.GetPlayerSourceRectangle("LeftHand_Pose", 3);

        leftHandSprite = new SpriteAnimated(
                texture2D: JSON_Manager.playerSpriteSheet,
                sourceRectangles: sourceRectangles2,
                frameTimers: [0f, 0.25f, int.MaxValue],
                colorTints: [Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetPlayerOrigin("LeftHand_Pose", 3, leftHandObject.transform.globalScale),
                layerDepths: [0.2f, 0.2f, 0.2f]
        );

        leftHandObject.AddComponent(leftHandSprite);

        // 4.) right hand
        GameObject rightHandObject = new GameObject(id: ++currID);
        //rightHandLocalPosition = new Vector2(90 * gameObject.transform.globalScale.X, -450 * gameObject.transform.globalScale.Y);
        //rightHandObject.CreateTransform(localPosition: rightHandLocalPosition);
        rightHandObject.CreateTransform();
        gameObject.AddChild(rightHandObject);
        Rectangle[] sourceRectangles3 = JSON_Manager.GetPlayerSourceRectangle("RightHand_Pose", 4);

        rightHandSprite = new SpriteAnimated(
                texture2D: JSON_Manager.playerSpriteSheet,
                sourceRectangles: sourceRectangles3,
                frameTimers: [0f, 0.125f, 0.125f, int.MaxValue],
                colorTints: [Color.White, Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetPlayerOrigin("RightHand_Pose", 4, rightHandObject.transform.globalScale),
                layerDepths: [0.8f, 0.8f, 0.8f, 0.8f]
        );
        //rightHandSprite.layerDepth = 0.7f;

        rightHandObject.AddComponent(rightHandSprite);

        playerCollider = new OBBRectangleCollider(
            GameConstantsAndValues.SQUARE_TILE_WIDTH * 0.9f,
            movingSprite.sourceRectangle.Height * gameObject.transform.globalScale.Y,
            isAftermath: true,
            isRelaxPosition: true);
        gameObject.AddComponent(playerCollider);

        gameObject.tag = GameConstantsAndValues.Tags.Player.ToString();

        // Create normal visual GameObject
        // add sprites to normal visual object

        // Create SpecialAttack visual GameObject
        SpecialAttackGameObject = new GameObject();
        SpecialAttackGameObject.CreateTransform();
        gameObject.AddChild(SpecialAttackGameObject);

        WeaponDataBase weaponDataBase = new WeaponDataBase();
        weaponDataBase.CreateWeaponDictionary();

        boundsSprite = movingSprite;
    }

    public bool IsSpriteFlipped()
    {
        return leftHandSprite.spriteEffects == SpriteEffects.FlipHorizontally;
    }

    public void MakePlayerObjectChangeScene(Scene scene)
    {
        this.scene = scene;
        scene.RemoveGameObjectFromScene(gameObject, isOverlay: false); // add to scene
        scene.AddGameObjectToScene(gameObject, isOverlay: false); // add to scene
    }

    public void ResetPlayer(Vector2 globalPosition)
    {
        gameObject.tag = GameConstantsAndValues.Tags.Player.ToString();
        if (healthBar is null)
        {
            CreateHealthBar(maxHealth: 100, maxShield: 100);
            healthBar.gameObject.transform.localPosition = new Vector2(0, -200);

            // create chargeBar
            WeaponChargeBar chargeBar = new WeaponChargeBar(this, parent: gameObject, localPosition: new Vector2(-135, -150));

            moveSpeed = 300;
            gameObject.transform.localScale = new Vector2(0.2f, 0.2f);

            playerShadow = new PlayerShadow(gameObject.transform.globalScale, scene: scene);

            SetupColliders();
        }

        if (!cheatEnabled) moveSpeed = 300;
        IsKnockedBack = false;
        healthBar.currHealth = healthBar.maxHealth;
        //healthBar.currShield = healthBar.maxShield;
        healthBar.currShield = 0;
        dmgReduction = 0;

        //if (!isInitialized) LoadContent();

        //SceneManager.Instance.activeScene.mainCamera.Zoom = 1.75f;

        Velocity = Vector2.Zero;
        cumulatedVelocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        //currCollisionEnemyDamageCooldown = 0;
        currElementSwapCooldownTimer = 0;
        currRealoadTimer = 0;
        currWeaponSwapCooldownTimer = 0;
        timeSinceJump = 0;
        isChargingAttack = false;

        gameObject.transform.globalPosition = globalPosition;
        knockBackVelocity = Vector2.Zero;
        currKnockBackImunityTimer = 0;
        isPerformingSpecialAttack = false;

        // reset weapon show
        foreach (Weapon[] elementWeapons in WeaponDataBase.WeaponDictionary.Values)
        {
            elementWeapons[0].gameObject.SetActive(false);
            elementWeapons[1].gameObject.SetActive(false);
        }

        int weaponTypeIndex = WeaponDataBase.BLADE_INDEX;

        currElementLoadoutIndex = 0;
        equipedWeapon = WeaponDataBase.WeaponDictionary[ElementDataBase.GetLoadoutElement(currElementLoadoutIndex)][weaponTypeIndex];
        equipedWeapon.gameObject.SetActive(true);

        visualsDisabled = false;
        leftHandSprite.gameObject.SetActive(true);
        rightHandSprite.gameObject.SetActive(true);
        movingSprite.gameObject.SetActive(true);
        jumpSprite.gameObject.SetActive(false);
        SpecialAttackGameObject.SetActive(false);

        grabbed = false;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (grabbed) return;

        float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        AttackLogic(gameTime);
        // make teleport imunity
        if (enteredTeleport)
        {
            currTeleportImunityTimer -= elapsedTime;
            currTeleportDurationTimer -= elapsedTime;

            if (currTeleportDurationTimer <= 0)
            {
                isMovable = true;
            }

            if (currTeleportImunityTimer <= 0)
            {
                enteredTeleport = false;
                return;
            }
        }

        UpdateCollider();

        isInput = false;

        // Update timers
        timeSinceJump += elapsedTime;
        currElementSwapCooldownTimer -= elapsedTime; // reset will happen when element swap is called
        currWeaponSwapCooldownTimer -= elapsedTime; // reset will happen when weapon swap is called

        currRealoadTimer -= elapsedTime; // ZZZZZ
        if (currRealoadTimer < 0)
        {
            isReloading = false;
            isAttacking = false;
        }

        // Get movement input from the player
        //KeyboardState keyboardState = Keyboard.GetState();

        //Velocity.X = 0;
        Velocity = new Vector2(0, Velocity.Y);
        if (jumpCounter == 0) Velocity = new Vector2(Velocity.X, 0);  //Velocity.Y = 0; 

        isMoving = false;

        // player movement
        //if (InputController.Instance.IsKeyHeld(Keys.Left) || InputController.Instance.IsKeyHeld(Keys.A))
        if (KeyBindManager.Instance.IsActionHeld(GameAction.LEFT))
        {
            isMoving = true;
            //Velocity.X = -moveSpeed;
            Velocity = new Vector2(-moveSpeed, Velocity.Y);

            leftHandSprite.spriteEffects = SpriteEffects.FlipHorizontally;
            //leftHandSprite.gameObject.transform.localPosition = new Vector2(-leftHandLocalPosition.X, leftHandLocalPosition.Y);
            rightHandSprite.spriteEffects = SpriteEffects.FlipHorizontally;
            //rightHandSprite.gameObject.transform.localPosition = new Vector2(-rightHandLocalPosition.X, rightHandLocalPosition.Y);
            movingSprite.spriteEffects = SpriteEffects.FlipHorizontally;
            jumpSprite.spriteEffects = SpriteEffects.FlipHorizontally;

            //SpecialAttackGameObject.GetComponent<SpriteAnimated>().spriteEffects = SpriteEffects.FlipHorizontally;
        }
        //else if (IsKeyHeld(Keys.Right) || IsKeyHeld(Keys.D))
        else if (KeyBindManager.Instance.IsActionHeld(GameAction.RIGHT))
        {
            isMoving = true;
            //Velocity.X = moveSpeed;
            Velocity = new Vector2(moveSpeed, Velocity.Y); //Velocity.Y = 0;

            leftHandSprite.spriteEffects = SpriteEffects.None;
            //leftHandSprite.gameObject.transform.localPosition = new Vector2(leftHandLocalPosition.X, leftHandLocalPosition.Y);
            rightHandSprite.spriteEffects = SpriteEffects.None;
            //rightHandSprite.gameObject.transform.localPosition = new Vector2(rightHandLocalPosition.X, rightHandLocalPosition.Y);
            movingSprite.spriteEffects = SpriteEffects.None;
            jumpSprite.spriteEffects = SpriteEffects.None;
            //SpecialAttackGameObject.GetComponent<SpriteAnimated>().spriteEffects = SpriteEffects.None;
        }

        //if(playerShadow.gameObject.isActive && InputController.Instance.IsKeyPressed(key: Keys.R)){
        if (playerShadow.gameObject.isActive && KeyBindManager.Instance.IsActionPressed(GameAction.SPECIAL_ABILITY))
        {
            playerShadow.TeleportPlayer();
        }

        // TESTING ONLY
        if (cheatEnabled)
        {
            if (InputController.Instance.IsKeyHeld(Keys.Up) || InputController.Instance.IsKeyHeld(Keys.W))
            {
                isMoving = true;
                //Velocity.Y = -moveSpeed;
                Velocity = new Vector2(Velocity.X, -moveSpeed); //Velocity.Y = 0;
            }
            else if (InputController.Instance.IsKeyHeld(Keys.Down) || InputController.Instance.IsKeyHeld(Keys.S))
            {
                isMoving = true;
                //Velocity.Y = moveSpeed;
                Velocity = new Vector2(Velocity.X, moveSpeed); //Velocity.Y = 0;

            }
        }
        //if (InputController.Instance.IsKeyPressed(Keys.Space))
        if (KeyBindManager.Instance.IsActionPressed(GameAction.JUMP))
        {
            isInput = true;
            Jump();
        }

        // translation
        if (Velocity.X != 0) isInput = true;

        //Movement.MoveInLine(gameTime, translationVector, moveSpeed, this.gameObject.transform);
        // swap weapons: Bow -> Blade -> Bow
        //if (InputController.Instance.IsKeyPressed(Keys.Q))
        if (KeyBindManager.Instance.IsActionPressed(GameAction.SWAP_WEAPON))
        {
            SwapWeaponType();
        }

        // swap elements -> next element in dictionary
        //if (InputController.Instance.IsKeyPressed(Keys.E))
        if (KeyBindManager.Instance.IsActionPressed(GameAction.CHANGE_ELEMENT))
        {
            SwapWeaponElements();
        }

        if (!isInput)
        {
            untilIdleTime -= elapsedTime;
            if (untilIdleTime <= 0)
            {
                isIdle = true;
            }
        }
        else
        {
            ResetIdleTimer();
        }

        if (isIdle)
        {
            //PerformAnimation("Idle");
        }


        // move camera
        SceneManager.Instance.activeScene.mainCamera.FollowSmooth(gameObject, lerpFactor: 0.3f);

        //movingSprite.gameObject.SetActive(true);

        // update movement sprite
        //movingSpriteCount

        if (!visualsDisabled)
        {
            movingSprite.gameObject.SetActive(jumpCounter == 0);
            jumpSprite.gameObject.SetActive(jumpCounter > 0);
            if (movingSprite.gameObject.isActive)
            {
                if (isMoving || movingSprite.currFrameIndex > 0)
                {
                    movingSprite.ResumeAnimation();
                }
                else
                {
                    movingSprite.PauseAnimation();
                }
            }
        }

        ApplyKnockBack(elapsedTime);

        if (gameObject.transform.globalPosition.Y > deathByGravityHeight)
        {
            ApplyDeath();
        }
    }

    public void PerformSpecialAttack(bool isPerforming, bool shouldDisableVisuals)
    {
        if (isPerformingSpecialAttack == isPerforming) return;

        isPerformingSpecialAttack = isPerforming;

        if (shouldDisableVisuals)
        {
            SetVisuals(!isPerforming);
        }
    }

    public void SetVisuals(bool enable)
    {
        Debug.WriteLine("setting visuals");
        visualsDisabled = !enable;
        // right hand is only enabled if it is a blade weapon

        rightHandSprite.gameObject.SetActive(enable && equipedWeapon.weaponType == Weapon.WeaponType.Blade);
        leftHandSprite.gameObject.SetActive(enable);
        movingSprite.gameObject.SetActive(enable);
        jumpSprite.gameObject.SetActive(enable);

        equipedWeapon.gameObject.SetActive(enable);
    }

    private void AttackLogic(GameTime gameTime)
    {
        bool attackHeld = KeyBindManager.Instance.IsActionHeld(GameAction.ATTACK);
        bool attackPressed = KeyBindManager.Instance.IsActionPressed(GameAction.ATTACK);

        // Check if attack is held
        if (attackHeld)
        {
            isInput = true;

            float currWeaponCharge = equipedWeapon.currChargingLevel;

            if (isChargingAttack)
            {
                // update projectiles size, trajectory...
                Vector2 mousePos = GetAttackTargetPosition();
                equipedWeapon.UpdateWeaponChargeValue(gameTime, destinationVector: mousePos);
                if (currWeaponCharge != equipedWeapon.currChargingLevel)
                {
                    OnWeaponChargeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (!isReloading)
            {
                isChargingAttack = true;
                Vector2 mousePos = GetAttackTargetPosition();
                equipedWeapon.BeginCharging(destinationVector: mousePos);
            }
        }

        // Check if attack is released
        if (isChargingAttack && !attackHeld)
        {
            currRealoadTimer = equipedWeapon.reloadTimer;
            isChargingAttack = false;

            isAttacking = true;
            isReloading = true;

            leftHandSprite.animationEnabled = true;
            if (equipedWeapon.weaponType == Weapon.WeaponType.Blade) rightHandSprite.animationEnabled = true;

            equipedWeapon.Attack();
            OnWeaponAttacked?.Invoke(this, EventArgs.Empty);
        }
    }

    // Helper function to get the target position for attacks
    private Vector2 GetAttackTargetPosition()
    {
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        mousePos = SceneManager.Instance.activeScene.mainCamera.ScreenToWorld(mousePos, Game2DPlatformer.Instance.GraphicsDevice);

        return mousePos;
    }

    public void SwapWeaponType()
    {
        // if linked weapon does not exist or if weapon swap is on cooldown -> swap can't be performed
        if (equipedWeapon.linkedWeapon == null || currWeaponSwapCooldownTimer > 0) return;

        SpecialAttackGameObject.SetActive(false); // hide special attack if it was active


        SwapWeapon(prevWeapon: equipedWeapon, newWeapon: equipedWeapon.linkedWeapon);

        OnWeaponSwapped?.Invoke(this, EventArgs.Empty);
        currWeaponSwapCooldownTimer = maxWeaponSwapCooldownTimer;

        // if current timer is bigger than equiped weapons reload timer just, adjust reload timer
        // TODO find better solution
        currRealoadTimer = 0;
        //if (currRealoadTimer > equipedWeapon.reloadTimer) currRealoadTimer = equipedWeapon.reloadTimer;

        rightHandSprite.gameObject.SetActive(equipedWeapon.weaponType == Weapon.WeaponType.Blade);
    }

    public void SwapWeaponElements()
    {
        // if element swap is on cooldown -> swap can't be performed
        if (currElementSwapCooldownTimer > 0) return;

        // increment imbuedElementIndex
        if (++currElementLoadoutIndex == loadout.elements.Length) currElementLoadoutIndex = 0;
        // get new eLement
        Weapon.ImbuedElement element = ElementDataBase.GetLoadoutElement(currElementLoadoutIndex);
        int equipedWeaponTypeIndex = (equipedWeapon.weaponType == Weapon.WeaponType.Blade) ? WeaponDataBase.BLADE_INDEX : WeaponDataBase.BOW_INDEX;
        SwapWeapon(prevWeapon: equipedWeapon, newWeapon: WeaponDataBase.WeaponDictionary[element][equipedWeaponTypeIndex]);

        OnElementSwapped?.Invoke(this, EventArgs.Empty);
        currElementSwapCooldownTimer = maxElementSwapCooldownTimer;
        // if current timer is bigger than equiped weapons reload timer just, adjust reload timer
        // TODO find better solution
        currRealoadTimer = 0;
    }

    private void SwapWeapon(Weapon prevWeapon, Weapon newWeapon)
    {
        if (prevWeapon is null || newWeapon is null) return;

        Weapon_Blade blade = prevWeapon as Weapon_Blade;
        blade?.CancelSpecialAttack();

        prevWeapon.gameObject.SetActive(false); // prevent drawing
        equipedWeapon = newWeapon;
        newWeapon.gameObject.SetActive(true); // enable drawing new weapon
        SpriteAnimated spriteAnimated = equipedWeapon.gameObject.GetComponent<SpriteAnimated>();
        if (spriteAnimated is not null) spriteAnimated.animationEnabled = false; // disable animation, it is enabled on attack
    }

    private void ResetIdleTimer()
    {
        untilIdleTime = 5;
        isIdle = false;
    }

    private void Jump()
    {
        if (++jumpCounter > 2) return;
        // reset cumulative velocity
        cumulatedVelocity.Y = 0;

        isMoving = true;
        isInput = true;

        if (jumpCounter == 1)
        {
            Velocity.Y = -jumpSpeed + plaftormJumpVelocityY; // - is positive from screen
            //Velocity = new Vector2(Velocity.X, -jumpSpeed + plaftormJumpVelocityY); //Velocity.Y = 0;
        }
        if (jumpCounter == 2)
        {
            float minJumpStrength = .5f;
            float maxJumpStrength = 1.8f;
            float assumedJumpStrength = 1f / timeSinceJump; // based on how fast double jump is pressed
            float actualJumpStength = MathF.Max(minJumpStrength, MathF.Min(maxJumpStrength, assumedJumpStrength));

            Velocity.Y = actualJumpStength * -jumpSpeed + plaftormJumpVelocityY; // - is positive from screen
            //Velocity = new Vector2(Velocity.X, actualJumpStength * -jumpSpeed + plaftormJumpVelocityY); //Velocity.Y = 0;
        }

        timeSinceJump = 0;
        jumpSprite.animationEnabled = true;

        plaftormJumpVelocityY = 0;
        SoundController.instance.PlaySoundEffect("jump", pitch: 1);
    }

    public override void OnCollisionEnter(Collider collider)
    {
        base.OnCollisionEnter(collider);

        Projectile projectile = collider.gameObject.GetComponent<Projectile>();
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (projectile != null)
        {
            if (!projectile.isPlayerSpawned)
            {
                if (projectile is Projectile_AfterAntiMatterHand || projectile is Projectile_AfterAntiVerseHand) return;
                //if (gameObject.tag != GameConstantsAndValues.Tags.EnemyMeleeSpawned.ToString()) return;

                StatChangeFunctions.PlayerDamageCalculation(enemy: (Enemy)projectile.projectileOwner);
                StatChangeFunctions.ApplyElementalEffect(
                    attacker: (Enemy)projectile.projectileOwner,
                    target: this,
                    effect: projectile.element);
            }
        }
        else if (enemy != null)
        {
            StatChangeFunctions.PlayerDamageCalculation(enemy);
        }
    }

    protected override void TerrainCollision(Collider collider)
    {
        base.TerrainCollision(collider);

        /*
        float halfWidth = movingSprite.sourceRectangle.Width / 2 * gameObject.transform.globalScale.X;
        float halfHeight = movingSprite.sourceRectangle.Height / 2 * gameObject.transform.globalScale.Y;
        Terrain_Tile terrainTile = collider.gameObject?.parent?.GetComponent<PhysicsComponent>() as Terrain_Tile;
        if(terrainTile is not null)
        {
            terrainTile.SnapToPosition(this.gameObject.transform, halfWidth: halfWidth, halfHeight: halfHeight);
        }

        Velocity.Y = 0;
        cumulatedVelocity.Y = 0;*/
        jumpCounter = 0;

        if (!movingSprite.animationEnabled) movingSprite.animationEnabled = true;
    }

    /// <summary>
    /// base knock back velocity with push power 1 = 500
    /// </summary>
    public void BeginKnockBack(float pushPower, Vector2 direction, float knockDuration = 0.3f)
    {
        if (currKnockBackImunityTimer > 0) return; // currently immune to knockBack
        currKnockBackImunityTimer = knockBackImunityTimer;

        knockBackVelocity = direction * pushPower * baseKnockBackVelocity;

        knockBackDuration = knockDuration;

        IsKnockedBack = true;
    }

    private void ApplyKnockBack(float elapsedTime)
    {
        if (IsKnockedBack)
        {
            // Apply knockback force to the player
            gameObject.transform.globalPosition += knockBackVelocity * elapsedTime;

            // Decrease the knockback duration over time
            knockBackDuration -= elapsedTime;

            // Decrease knockback imunity timer
            currKnockBackImunityTimer -= elapsedTime;

            if (knockBackDuration <= 0)
            {
                IsKnockedBack = false;
                knockBackVelocity = Vector2.Zero;
            }
        }
    }

    public void Teleport(Vector2 teleportLocation)
    {
        gameObject.transform.globalPosition = teleportLocation;

        currTeleportDurationTimer = teleportDurationTimer;
        currTeleportImunityTimer = 3 * teleportDurationTimer;
        enteredTeleport = true;
        isMovable = false;
    }

    public override void ApplyDeath()
    {
        Weapon_Bow bow = equipedWeapon as Weapon_Bow;
        bow?.ReleaseAttackOnDeath();

        base.ApplyDeath();
    }
}
