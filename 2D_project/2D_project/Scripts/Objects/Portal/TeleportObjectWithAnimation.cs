using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class TeleportObjectWithAnimation : TeleportObject
{
    GameObject portalAnimationObject;
    bool isAnimationPlaying = false;

    string animationSpriteName;
    int animationSpriteCount;
    float[] frameTimers;
    Color[] colorTints;

    public TeleportObjectWithAnimation(Vector2 portalLocation, string portalSpriteName, string animationSpriteName, int animationSpriteCount) : base(portalLocation, portalSpriteName)
    {
        this.animationSpriteName = animationSpriteName;
        this.animationSpriteCount = animationSpriteCount;

        frameTimers = new float[animationSpriteCount];
        colorTints = new Color[animationSpriteCount];

        for (int i = 0; i < animationSpriteCount; i++)
        {
            frameTimers[i] = 0.2f;
            colorTints[i] = Color.White;
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        portalAnimationObject = new GameObject();
        portalAnimationObject.CreateTransform();
        SpriteAnimated portalAnimationSprite = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle(tileName: animationSpriteName, animationSpriteCount),
            frameTimers: frameTimers,
            colorTints: colorTints
        );
        portalAnimationSprite.PauseAnimation();

        gameObject.AddChild(portalAnimationObject);
        portalAnimationObject.AddComponent(portalAnimationSprite);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    private void PlayAnimation()
    {
        if (isAnimationPlaying && portalAnimationObject.GetComponent<SpriteAnimated>().isAnimationPaused)
        {
            isAnimationPlaying = false;

            // teleport player to location
            Player.Instance.gameObject.transform.globalPosition = portalLocation;
        }
    }

    public override void OnCollisionEnter(Collider collider)
    {
        if (collider.gameObject.tag == GameConstantsAndValues.Tags.Player.ToString())
        {
            isAnimationPlaying = true;

            portalAnimationObject.GetComponent<SpriteAnimated>().ResumeAnimation();
        }
    }
}
