using GamePlatformer;
using MGEngine.Collision.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
internal class PressureButton : ObjectComponent
{
    public event EventHandler OnPressureButtonPressed;

    private SpriteAnimated buttonSprite;
    private bool isPressed = false;
    private List<string> pressureTargetTags = new List<string>();
    int resetInXSeconds;
    public PressureButton(string[] pressureTargetTags, int resetInXSeconds = int.MaxValue)
    {
        this.pressureTargetTags = pressureTargetTags.ToList<string>();
        propagatedCollisionEnabled = false;

        this.resetInXSeconds = resetInXSeconds;
    }

    public void Reset()
    {
        buttonSprite.PauseAnimation();
        buttonSprite.SetFrame(0);
        isPressed = false;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Reset();
    }

    public override void Initialize()
    {
        base.Initialize();

        buttonSprite = new SpriteAnimated(
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangles: [
                JSON_Manager.GetUITile("Button_0"),
                JSON_Manager.GetUITile("Button_1"),
                JSON_Manager.GetUITile("Button_2"),
            ],
            frameTimers: [0.05f, 0.1f, int.MaxValue],
            layerDepths: [0.1f, 0.1f, 0.1f],
            origins: JSON_Manager.GetUIOrigins("Button", 3, gameObject.transform.globalScale)
        );
        buttonSprite.loopEnabled = false;

        OBBRectangleCollider oBBRectangleCollider = new OBBRectangleCollider(
            width: buttonSprite.sourceRectangle.Width * gameObject.transform.globalScale.X * 1.8f,
            height: buttonSprite.sourceRectangle.Height * gameObject.transform.globalScale.Y * 0.8f,
            isAftermath: false
        );

        gameObject.tag = GameConstantsAndValues.Tags.Button.ToString();
        gameObject.AddComponent(buttonSprite);
        gameObject.AddComponent(oBBRectangleCollider);

        gameObject.id = 7777;

        Reset();
    }

    public override void OnCollisionEnter(Collider collider)
    {
        //if (isPressed) return;
        if (isPressed || !pressureTargetTags.Contains(collider.gameObject.tag)) return;

        base.OnCollisionEnter(collider);
        buttonSprite.ResumeAnimation();
        collider.gameObject.GetComponent<Projectile>()?.gameObject.SetActive(false);

        isPressed = true;
        OnPressureButtonPressed?.Invoke(this, EventArgs.Empty);

        // reset button
        ResetButtonInSeconds(resetInXSeconds);
    }

    public override void OnDetectionRange(Collider collider)
    {
        //if (isPressed) return;
        if (isPressed || !pressureTargetTags.Contains(collider.gameObject.tag)) return;

        base.OnDetectionRange(collider);
        buttonSprite.ResumeAnimation();
        collider.gameObject.GetComponent<Projectile>()?.gameObject.SetActive(false);
        isPressed = true;
        OnPressureButtonPressed?.Invoke(this, EventArgs.Empty);

        // reset button
        ResetButtonInSeconds(resetInXSeconds);
    }

    private void ResetButtonInSeconds(float delay)
    {
        if (delay == int.MaxValue) return; // never resets

        Timer timer = new Timer(Game2DPlatformer.Instance, delay);

        // Define the callback -> reset after x amount of seconds
        Action<Timer> callback = null!;
        callback = (Timer t) =>
        {
            t.OnCountdownEnd -= callback; // Clean up

            Reset();
            Game2DPlatformer.Instance.Components.Remove(t); // Optional, if not in Timer internally
            t.Dispose(); // Safe here
        };

        timer.OnCountdownEnd += callback;
        timer.BeginTimer();
    }
}