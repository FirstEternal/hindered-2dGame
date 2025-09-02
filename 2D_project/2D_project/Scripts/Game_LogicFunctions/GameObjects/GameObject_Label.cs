using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class GameObject_Label : GameObject
{
    SpriteTextComponent spriteText;
    int buttonWidth;
    int buttonHeight;

    bool isButton;
    Color buttonColor;
    Rectangle sourceRectangle;
    Texture2D texture2D;

    private int sliceBorderSize;
    public GameObject_Label(GraphicsDevice graphicsDevice, bool isButton, Texture2D texture2D, Rectangle sourceRectangle, Color buttonColor, int buttonWidth, int buttonHeight, Color textColor, BitmapFont_equalHeight_dynamicWidth font, BitmapFont_equalHeight_dynamicWidth.FontStyle fontStyle, string text, int fontSize, BitmapFont_equalHeight_dynamicWidth.CenterX centerX = BitmapFont_equalHeight_dynamicWidth.CenterX.Left, BitmapFont_equalHeight_dynamicWidth.CenterY centerY = BitmapFont_equalHeight_dynamicWidth.CenterY.Middle, int sliceBorderSize = 0, int id = -1, string tag = "") : base(id, tag)
    {
        CreateTransform();
        this.isButton = isButton;
        this.buttonWidth = buttonWidth;
        this.buttonHeight = buttonHeight;

        int labelWidth = buttonWidth - 5;
        int labelHeight = buttonHeight - 5;

        this.buttonColor = buttonColor;
        this.sourceRectangle = sourceRectangle;
        this.texture2D = texture2D;

        //this.spriteText = new SpriteTextComponent(labelWidth, labelHeight, font, text, 0, fontSize: fontSize, color: textColor);
        this.spriteText = new SpriteTextComponent(
            labelWidth,
            labelHeight,
            font,
            text,
            fontStyle,
            spacingX: 5,
            fontSize: fontSize,
            //fontSize: labelHeight - 6,  
            color: textColor,
            textCenterX: centerX,
            textCenterY: centerY,
            graphicsDevice: graphicsDevice
        );

        this.sliceBorderSize = sliceBorderSize;
    }

    public void InitializeLabel()
    {
        GameObject_TextField textField = new GameObject_TextField(spriteText);
        AddChild(textField, isOverlay: true);

        if (!isButton)
        {
            Label label = new Label(
                width: buttonWidth,
                height: buttonHeight,
                texture2D: texture2D,
                buttonColor: buttonColor,
                IResizableVisualComponent.ResizeType.Fill,
                textField,
                PivotCentering.Enum_Pivot.Center
             );

            label.sourceRectangle = sourceRectangle;
            label.origin = new Vector2(label.sourceRectangle.Width / 2, label.sourceRectangle.Height / 2);
            AddComponent(label);

            if (sliceBorderSize > 0) label.EnableNineSliceDraw(sliceBorderSize);
        }
        else
        {
            Button button = new Button(
                ButtonResponseSystem.Instance,
                width: buttonWidth,
                height: buttonHeight,
                texture2D: JSON_Manager.uiSpriteSheet,
                buttonColor: buttonColor,
                IResizableVisualComponent.ResizeType.Fill,
                textField,
                PivotCentering.Enum_Pivot.Center
            );

            button.sourceRectangle = sourceRectangle;
            button.origin = new Vector2(button.sourceRectangle.Width / 2, button.sourceRectangle.Height / 2);
            AddComponent(button);

            if (sliceBorderSize > 0) button.EnableNineSliceDraw(sliceBorderSize);
        }
    }
}
