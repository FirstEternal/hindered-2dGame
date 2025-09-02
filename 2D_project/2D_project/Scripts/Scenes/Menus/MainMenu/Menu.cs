using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class Menu : GameObject
{
    public Menu()
    {
        CreateTransform();

        ExisitingPlayerSessions.OnSaveLoadDelete -= OnSessionUpdate;
        ExisitingPlayerSessions.OnSaveLoadDelete += OnSessionUpdate;

        Menu_Session.OnSessionLoad -= OnSessionUpdate;
        Menu_Session.OnSessionLoad += OnSessionUpdate;
    }

    protected virtual void CreateMenu() { }
    protected virtual void UpdateMenu() { }
    private void OnSessionUpdate(object sender, EventArgs e)
    {
        if (FullMenu.Instance.sessionMenu is null || !FullMenu.Instance.sessionMenu.hasLoaded) return;
        UpdateMenu();
    }
    protected virtual GameObject CreateBackgroundPanel(int background_panelWidth, int background_panelHeight, bool isPanelInvisible = false)
    {
        // background -> black border
        GameObject background_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: background_panelWidth,
            height: background_panelHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("backgroundPanel"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );
        AddChild(background_panelObject, isOverlay: true);
        return background_panelObject;
    }

    public static GameObject_Label ButtonGameObject(string buttonText, int buttonWidth, int buttonHeight, Color buttonColor, int curr_x_offset, int curr_y_offset, Panel parentPanel, PivotCentering.Enum_Pivot buttonPivot, Color? textColor = null, int fontSize = 30,
        BitmapFont_equalHeight_dynamicWidth.CenterX centerX = BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
        BitmapFont_equalHeight_dynamicWidth.CenterY centerY = BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
        int sliceBorderSize = 12
    )
    {
        return LabelObject(true, buttonText, buttonWidth, buttonHeight, buttonColor, curr_x_offset, curr_y_offset, parentPanel, buttonPivot, fontSize, centerX, centerY, textColor: textColor, sliceBorderSize);
    }

    public static GameObject_Label LabelGameObject(string labelText, int labelWidth, int labelHeight, Color labelColor, int curr_x_offset, int curr_y_offset, Panel parentPanel, PivotCentering.Enum_Pivot buttonPivot, Color? textColor = null, int fontSize = 30,
        BitmapFont_equalHeight_dynamicWidth.CenterX centerX = BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
        BitmapFont_equalHeight_dynamicWidth.CenterY centerY = BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
        int sliceBorderSize = 0
    )
    {
        return LabelObject(false, labelText, labelWidth, labelHeight, labelColor, curr_x_offset, curr_y_offset, parentPanel, buttonPivot, fontSize, centerX, centerY, textColor: textColor, sliceBorderSize);
    }

    public static GameObject_Label LabelObject(bool isButton, string labelText, int labelWidth, int labelHeight, Color labelColor, int curr_x_offset, int curr_y_offset, Panel parentPanel, PivotCentering.Enum_Pivot buttonPivot, int fontSize = 30,
        BitmapFont_equalHeight_dynamicWidth.CenterX centerX = BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
        BitmapFont_equalHeight_dynamicWidth.CenterY centerY = BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
        Color? textColor = null, int sliceBorderSize = 0
    )
    {
        GameObject_Label LabelGameObject = new GameObject_Label(
            Game2DPlatformer.Instance.GraphicsDevice,
            isButton: isButton,
            texture2D: JSON_Manager.uiSpriteSheet,
            //sourceRectangle: JSON_Manager.GetUITile(sourceRectName),
            sourceRectangle: JSON_Manager.GetUITile(isButton ? "Button3" : "background"),
            buttonColor: labelColor,
            buttonWidth: labelWidth,
            buttonHeight: labelHeight,
            textColor: textColor ?? Color.White,
            //textColor: !isButton ? GameConstantsAndValues.PanelColor_DarkBlueFull : Color.White,
            text: labelText,
            font: JSON_Manager.customBitmapFont,
            fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            fontSize: fontSize,
            centerX: centerX,
            centerY: centerY
        );

        if (parentPanel != null)
        {
            parentPanel.gameObject.AddChild(LabelGameObject, isOverlay: true);
        }

        LabelGameObject.InitializeLabel();
        Type controlType = isButton ? typeof(Button) : typeof(Label);

        if (parentPanel != null)
        {
            if (isButton)
            {
                PivotCentering.UpdatePivot(parentPanel, LabelGameObject.GetComponent<Button>(),
                    LabelGameObject.transform, buttonPivot, new Vector2(curr_x_offset, curr_y_offset));
            }
            else
            {
                PivotCentering.UpdatePivot(parentPanel, LabelGameObject.GetComponent<Label>(),
                    LabelGameObject.transform, buttonPivot, new Vector2(curr_x_offset, curr_y_offset));
            }
        }

        if (isButton) Button_HoverColorChange.AddColorChangeAndSoundEffectOnHover(LabelGameObject.GetComponent<Button>(), labelColor);

        if (sliceBorderSize > 0)
        {
            Panel panel = isButton ? LabelGameObject.GetComponent<Button>() : LabelGameObject.GetComponent<Label>();
            // enable slicing
            panel.EnableNineSliceDraw(sliceBorderSize);

        }
        return LabelGameObject;
    }
}
