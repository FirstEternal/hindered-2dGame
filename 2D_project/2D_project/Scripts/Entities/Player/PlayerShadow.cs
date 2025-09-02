using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class PlayerShadow : PhysicsComponent
{
    private const float minDeathTimer = 4f;
    private float maxDeathTimer;
    private float currDeathTimer;

    private Sprite deathTimerSprite;
    private Rectangle originalDeathTimerSpriteRectangle;

    private float lastStep;
    private const float colorChangeTimer = minDeathTimer * 0.5f;
    private int currColorTintIndex;
    private Color[] colorTints = [Color.White, Color.Yellow, Color.Red];

    public PlayerShadow(Vector2 localScale, Scene scene) : base(mass: 10000, isGravity: false)
    {
        GameObject gameObject = new GameObject();
        gameObject.CreateTransform();
        gameObject.transform.localScale = localScale;

        scene.AddGameObjectToScene(gameObject, isOverlay: false);

        gameObject.AddComponent(this);
        /*
        Sprite sprite = new Sprite(
            texture2D: JSON_Manager.playerSpriteSheet,
            colorTint: Color.White           
        );*/

        SpriteAnimated spriteAnimated = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("Player_Shadow", 2),
            frameTimers: [0.3f, int.MaxValue],
            origins: JSON_Manager.GetPlayerOrigin("Player_Shadow", 2, gameObject.transform.globalScale),
            layerDepths: [0.9f, 0.9f]
        );

        gameObject.AddComponent(spriteAnimated);
        /*
        sprite.sourceRectangle = JSON_Manager.GetPlayerSourceRectangle("Player_Shadow", 1)[0];
        sprite.origin = JSON_Manager.GetPlayerOrigin("Player_Shadow", 1, gameObject.transform.globalScale)[0];
        sprite.layerDepth = 0.9f;
*/

        // TODO -> create duration bar
        // 2.) Create Duration Object        
        // create outer border
        GameObject borderObject = new GameObject();
        borderObject.CreateTransform(localPosition: new Vector2(0, -150), localScale: new Vector2(0.8f, 0.8f));
        gameObject.AddChild(borderObject);

        Sprite borderSprite = new Sprite(
            texture2D: JSON_Manager.uiSpriteSheet,
            colorTint: Color.Black
        );
        borderSprite.sourceRectangle = JSON_Manager.GetUITile("HealthBar_Outer");
        borderSprite.origin = JSON_Manager.GetUIOrigin("HealthBar_Outer", borderObject.transform.globalScale);
        borderSprite.layerDepth = 0.75f;
        borderObject.AddComponent(borderSprite);
        // create inner health
        GameObject deathTimerObject = new GameObject();
        deathTimerObject.CreateTransform(localPosition: new Vector2(0, -150), localScale: new Vector2(0.8f, 0.8f));
        gameObject.AddChild(deathTimerObject);

        deathTimerSprite = new Sprite(
            texture2D: JSON_Manager.uiSpriteSheet,
            colorTint: Color.White
        );
        deathTimerSprite.layerDepth = 0.76f;

        deathTimerSprite.sourceRectangle = JSON_Manager.GetUITile("HealthBar_Inner");
        deathTimerSprite.origin = JSON_Manager.GetUIOrigin("HealthBar_Inner", deathTimerObject.transform.globalScale);
        deathTimerSprite.layerDepth = 0f;
        originalDeathTimerSpriteRectangle = deathTimerSprite.sourceRectangle;
        deathTimerObject.AddComponent(deathTimerSprite);

        gameObject.SetActive(false);
    }

    public void SpawnShadow(Vector2 spawnPosition, float chargedLevel)
    {
        gameObject.transform.globalPosition = spawnPosition;
        gameObject.SetActive(true);

        maxDeathTimer = chargedLevel * minDeathTimer;
        currDeathTimer = maxDeathTimer;

        currColorTintIndex = Math.Min((int)(chargedLevel / 0.5f) - 1, colorTints.Length - 1);
        deathTimerSprite.colorTint = colorTints[currColorTintIndex];

        lastStep = currDeathTimer - colorChangeTimer;

        gameObject.GetComponent<SpriteAnimated>().SetFrame(0);
    }

    public void TeleportPlayer()
    {
        Player.Instance.gameObject.transform.globalPosition = gameObject.transform.globalPosition;
        currDeathTimer = 0; // kill object on player teleport
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds; ;

        // color change
        if (currColorTintIndex > 0 && currDeathTimer <= lastStep)
        {
            currColorTintIndex--;

            lastStep -= colorChangeTimer;
            deathTimerSprite.colorTint = colorTints[currColorTintIndex];
        }

        // death timer
        currDeathTimer -= elapsedTime;
        if (currDeathTimer < 0)
        {
            gameObject.SetActive(false);
        }
        float fillAmount = currDeathTimer / maxDeathTimer;
        deathTimerSprite.sourceRectangle = new Rectangle(originalDeathTimerSpriteRectangle.X, originalDeathTimerSpriteRectangle.Y, (int)(fillAmount * originalDeathTimerSpriteRectangle.Width), originalDeathTimerSpriteRectangle.Height);
    }
}
