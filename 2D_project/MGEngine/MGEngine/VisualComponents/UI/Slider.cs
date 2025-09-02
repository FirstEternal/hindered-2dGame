using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class Slider : Panel, IOnValueChange
{
    public Button button { get; private set; }
    Panel handlePanel;

    public float slideValue { get; private set; }
    public void ForcedValueUpdate(float value)
    {
        slideValue = Math.Clamp(value, 0f, 1f);
        UpdateSlidePosition(slideValue);
    }

    float minSlidePosX;
    float maxSlidePosX;
    bool isSliding = false;

    public IOnValueChange.OnValueChangeAction? onValueChangeAction { get; set; }
    public object[]? onValueChangeParameters { get; set; }
    public EventHandler? OnValueChange { get; set; }

    public void AssignOnValueChangeAction(IOnValueChange.OnValueChangeAction onValueChange, object[] parameters)
    {
        this.onValueChangeAction = onValueChange;
        onValueChangeParameters = parameters;
    }

    public Slider(Button button, int width, int height, Texture2D texture2D, Rectangle sourceRectangle, Color handlePanelColor, Color backgroundColor, float initialSlideValue = 0) : base(width, height, texture2D, handlePanelColor)
    {
        base.width = width;
        base.height = height;
        this.slideValue = Math.Clamp(initialSlideValue, 0f, 1f);

        this.button = button;

        colorTint = new Color(0, 0, 0, 0); // backgroundColor;
        this.sourceRectangle = sourceRectangle;
        origin = new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2);

        int handlePanelWidth = (int)(width * ((width - button.width) / width));

        handlePanel = new Panel(handlePanelWidth, (int)button.height / 2, texture2D, handlePanelColor);
        handlePanel.sourceRectangle = sourceRectangle;
        handlePanel.origin = new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2);
    }
    public override void Initialize()
    {
        // create button
        GameObject buttonObject = new GameObject();
        buttonObject.CreateTransform();
        buttonObject.AddComponent(button);

        button.AssignOnClickAction((parameters) => { isSliding = !isSliding; }, null);
        //Button_HoverColorChange.AddSoundEffectAndOnClickAction(button, (parameters) => { isSliding = !isSliding; }, null);

        // create slide panel
        GameObject slidePanelObject = new GameObject();
        slidePanelObject.CreateTransform();
        slidePanelObject.AddComponent(handlePanel);

        gameObject.AddChild(slidePanelObject, isOverlay: true);
        slidePanelObject.AddChild(buttonObject, isOverlay: true);

        int edgeOffset = 5;
        float left = gameObject.transform.globalPosition.X - width / 2 + edgeOffset;
        minSlidePosX = left + button.width / 2;
        maxSlidePosX = minSlidePosX + width - button.width - 2 * edgeOffset;
        UpdateSlidePosition(slideValue);

        base.Initialize();
    }

    private void UpdateSlidePosition(float slideValue)
    {
        this.slideValue = slideValue;
        if (button.gameObject is null) return;
        button.gameObject.transform.globalPosition.X = minSlidePosX + (slideValue * (maxSlidePosX - minSlidePosX));
    }

    private void UpdateSlideValue(float newPosX)
    {
        if (newPosX < minSlidePosX)
        {
            button.gameObject.transform.globalPosition.X = minSlidePosX;
        }
        else if (newPosX > maxSlidePosX)
        {
            button.gameObject.transform.globalPosition.X = maxSlidePosX;
        }
        else
        {
            button.gameObject.transform.globalPosition.X = newPosX;
        }

        slideValue = (button.gameObject.transform.globalPosition.X - minSlidePosX) / (maxSlidePosX - minSlidePosX);

        OnValueChange?.Invoke(this, EventArgs.Empty);
        onValueChangeAction?.Invoke(onValueChangeParameters);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (isSliding)
        {
            Debug.WriteLineIf(MouseGameObject.Singleton is null, "can't perform slide, while mouseObject is null");
            isSliding = MouseGameObject.Singleton?.MouseState.LeftButton == ButtonState.Pressed;
            UpdateSlideValue(MouseGameObject.Singleton?.transform.globalPosition.X ?? 0);
        }
    }
}