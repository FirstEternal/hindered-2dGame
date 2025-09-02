using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class PrefabObjectSliderWithLabels : GameObject
{
    Label leftLabel;
    int leftValue = 0;
    Label rightLabel;
    int rightValue = 0;
    public Slider slider { get; private set; }

    public void ForcedValueUpdate(float value)
    {
        slider.ForcedValueUpdate(value);
        UpdateText();
    }

    public PrefabObjectSliderWithLabels(float initialSlideValue, GameObject parent, int totalWidth, int totalHeight, int labelWidth, string leftLabelText, string rightLabelText, int id = -1, string tag = "") : base(id, tag)
    {
        CreateTransform();

        parent.AddChild(this, isOverlay: true);

        // example widht = 800, height = 200
        int panelWidth = totalWidth;
        int panelHeight = totalHeight;

        float offSet = 10;

        panelWidth = panelWidth - (int)(2 * offSet);
        panelHeight = panelHeight - (int)(2 * offSet);
        GameObject child_panelObject = PanelObject(
            width: panelWidth,
            height: 16,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: new Rectangle(1, 1, 0, 0),
            panelColor: Color.White
        );
        AddChild(child_panelObject, isOverlay: true);

        int fontSize = 25;
        //int spacing = 0;

        int maxCharCount = Math.Max(leftLabelText.Length, rightLabelText.Length);
        int maxLabelHeight = (int)(totalHeight - 2 * offSet);
        int maxLabelWidth = maxLabelHeight;

        int leftLabelWidth = leftLabelText.Length > 0 ? maxLabelWidth : 0;
        int rightLabelWidth = rightLabelText.Length > 0 ? maxLabelWidth : 0;

        int totalSliderLenght = totalWidth - leftLabelWidth - rightLabelWidth - (int)(2 * offSet);

        Rectangle sourceRectangle = JSON_Manager.GetUITile("Button3");
        Color textColor = Color.White;//Color.Black;
        if (leftLabelWidth > 0)
        {
            // add label to left side
            GameObject_Label buttonObject = new GameObject_Label(
                Game2DPlatformer.Instance.GraphicsDevice,
                isButton: false,
                texture2D: JSON_Manager.uiSpriteSheet,
                sourceRectangle: sourceRectangle,
                buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
                buttonWidth: leftLabelWidth,
                buttonHeight: maxLabelHeight,
                textColor: textColor,
                font: JSON_Manager.customBitmapFont,
                fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
                text: leftLabelText,
                fontSize: fontSize,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
                centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
            );

            child_panelObject.AddChild(buttonObject, isOverlay: true);
            buttonObject.InitializeLabel();

            leftLabel = buttonObject.GetComponent<Label>();

            PivotCentering.UpdatePivot(child_panelObject.GetComponent<Panel>(), buttonObject.GetComponent<Label>(), buttonObject.transform, PivotCentering.Enum_Pivot.CenterLeft, Vector2.Zero);
        }

        if (rightLabelWidth > 0)
        {
            // add label to left side
            GameObject_Label buttonObject = new GameObject_Label(
                Game2DPlatformer.Instance.GraphicsDevice,
                isButton: false,
                texture2D: JSON_Manager.uiSpriteSheet,
                sourceRectangle: sourceRectangle,
                buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
                buttonWidth: rightLabelWidth,
                buttonHeight: maxLabelHeight,
                textColor: textColor,
                font: JSON_Manager.customBitmapFont,
                fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
                text: rightLabelText,
                fontSize: fontSize,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
                centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
            );
            child_panelObject.AddChild(buttonObject, isOverlay: true);
            buttonObject.InitializeLabel();

            rightLabel = buttonObject.GetComponent<Label>();

            PivotCentering.UpdatePivot(child_panelObject.GetComponent<Panel>(), buttonObject.GetComponent<Label>(), buttonObject.transform, PivotCentering.Enum_Pivot.CenterRight, Vector2.Zero);
        }

        int slideButtonLabelWidth = (int)(20 * 0.95);
        int slideButtonLabelHeight = (int)(20 * 0.95);
        SpriteTextComponent spriteText = new SpriteTextComponent(
            slideButtonLabelWidth,
            slideButtonLabelHeight,
            JSON_Manager.customBitmapFont,
            "",
            BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            0,
            fontSize: GameConstantsAndValues.FONT_SIZE_M,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice);

        GameObject_TextField textField = new GameObject_TextField(spriteText);

        Button slideButton = new Button(
            ButtonResponseSystem.Instance,
            width: 24,
            height: 24,
            texture2D: JSON_Manager.uiSpriteSheet,
            buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
            spriteResizeType: IResizableVisualComponent.ResizeType.Fill,
            textField: textField,
            labelPositionPivot: PivotCentering.Enum_Pivot.Center
        );

        slideButton.sourceRectangle = JSON_Manager.GetUITile("UIBox");
        slideButton.origin = new Vector2(slideButton.sourceRectangle.Width / 2, slideButton.sourceRectangle.Height / 2);

        slider = new Slider(slideButton,
            totalSliderLenght,
            maxLabelHeight,
            JSON_Manager.uiSpriteSheet,
            JSON_Manager.GetUITile("background"),
            handlePanelColor: GameConstantsAndValues.PanelColor_lightBlue1,
            backgroundColor: new Color(0, 0, 0, 0),
            initialSlideValue: initialSlideValue
        ); //

        GameObject sliderObject = new GameObject();
        sliderObject.CreateTransform();
        sliderObject.AddComponent(slider);

        child_panelObject.AddChild(sliderObject, isOverlay: true);
        PivotCentering.Enum_Pivot sliderPos = PivotCentering.Enum_Pivot.Center;

        if (leftLabelWidth < 1)
        {
            sliderPos = PivotCentering.Enum_Pivot.CenterLeft;
        }
        else if (rightLabelWidth < 1)
        {
            sliderPos = PivotCentering.Enum_Pivot.CenterRight;
        }
        PivotCentering.UpdatePivot(child_panelObject.GetComponent<Panel>(), slider, sliderObject.transform, sliderPos, Vector2.Zero);

        if (leftLabel is null || rightLabel is null)
        {
            int.TryParse(leftLabelText, out leftValue);
            int.TryParse(rightLabelText, out rightValue);

            slider.OnValueChange += UpdateText;
        }

        // update text
        if (leftLabel is null)
        {
            int val = (int)(rightValue * slider.slideValue);
            rightLabel.textField.spriteTextComponent.text = val.ToString();
        }
        else if (rightLabel is null)
        {
            int val = (int)(leftValue * slider.slideValue);
            leftLabel.textField.spriteTextComponent.text = val.ToString();
        }
    }

    private void UpdateText(object sender, EventArgs e)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (leftLabel is null)
        {
            int val = (int)(rightValue * slider.slideValue);
            rightLabel.textField.spriteTextComponent.text = val.ToString();
        }
        else if (rightLabel is null)
        {
            int val = (int)(leftValue * slider.slideValue);
            leftLabel.textField.spriteTextComponent.text = val.ToString();
        }
    }

    public static GameObject PanelObject(int width, int height, Texture2D texture2D, Rectangle sourceRectangle, Color panelColor, Vector2? localPosition = null, float layerDepth = 0.01f, int sliceBorderSize = 0)
    {
        GameObject panelObject = new GameObject();
        Panel panel = new Panel(width, height, texture2D, panelColor);
        panelObject.CreateTransform(localPosition: localPosition);
        panel.sourceRectangle = sourceRectangle;
        panel.origin = new Vector2(panel.sourceRectangle.Width / 2, panel.sourceRectangle.Height / 2);
        panel.layerDepth = layerDepth;
        panelObject.AddComponent(panel);

        if (sliceBorderSize > 0)
        {
            panel.EnableNineSliceDraw(sliceBorderSize);
        }

        return panelObject;
    }
}
