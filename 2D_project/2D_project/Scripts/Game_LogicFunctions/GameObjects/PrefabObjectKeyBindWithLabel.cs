using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

internal class PrefabObjectKeyBindWithLabel : GameObject
{
    InputBinding binding;
    Vector2 originalLocalPosition;
    private bool isLocked;
    float maxButtonWidth;
    float labelWidth;
    Panel childObjectPanel;
    Label keybindLabel;
    int originalFontSize;
    GameAction action;

    private Button _button;
    public Button button => !isLocked ? _button : null;

    public PrefabObjectKeyBindWithLabel(string inputBindingKeyName, GameAction inputAction, InputBinding initialKeyBinding, GameObject parent, Vector2 localPosition, int labelWidth, int maxButtonWidth, int totalHeight, int spacing, bool isLocked = false, int id = -1, string tag = "") : base(id, tag)
    {
        this.isLocked = isLocked;
        Init(inputBindingKeyName, inputAction, initialKeyBinding, parent, localPosition, labelWidth, maxButtonWidth, totalHeight, spacing, isLocked);
    }
    public PrefabObjectKeyBindWithLabel(GameAction inputAction, InputBinding initialKeyBinding, GameObject parent, Vector2 localPosition, int labelWidth, int maxButtonWidth, int totalHeight, int spacing, bool isLocked = false, int id = -1, string tag = "") : base(id, tag)
    {
        this.isLocked = isLocked;
        Init(inputBindingKeyName: initialKeyBinding.ToString(), inputAction, initialKeyBinding, parent, localPosition, labelWidth, maxButtonWidth, totalHeight, spacing, isLocked);
    }

    private void Init(string inputBindingKeyName, GameAction inputAction, InputBinding initialKeyBinding, GameObject parent,
    Vector2 localPosition, int labelWidth, int maxButtonWidth, int totalHeight, int spacing, bool isLocked = false)
    {
        action = inputAction;
        if (!isLocked)
        {
            KeyBindManager.Instance.OnRebind -= UpdateText;
            KeyBindManager.Instance.OnRebind += UpdateText;
        }

        CreateTransform();

        binding = initialKeyBinding;
        parent.AddChild(this, isOverlay: true);

        int offSet = 2;
        this.maxButtonWidth = maxButtonWidth;
        this.labelWidth = labelWidth;
        int fontSize = GameConstantsAndValues.FONT_SIZE_S;
        originalFontSize = fontSize;
        int totalWidth = labelWidth + maxButtonWidth + 2 * offSet; // button size might change

        GameObject child_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: totalWidth,
            height: totalHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_lightBlue
        );
        AddChild(child_panelObject, isOverlay: true);

        childObjectPanel = child_panelObject.GetComponent<Panel>();

        transform.localPosition = localPosition;
        originalLocalPosition = new Vector2(-totalWidth / 2, 0);
        child_panelObject.transform.localPosition = originalLocalPosition;


        Color textColor = Color.White;

        int labelHeight = (int)(totalHeight - 2 * offSet);

        // LABEL
        GameObject_Label keybindLabelObject = new GameObject_Label(
            Game2DPlatformer.Instance.GraphicsDevice,
            isButton: false,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            buttonColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            buttonWidth: labelWidth,
            buttonHeight: labelHeight,
            textColor: textColor,
            font: JSON_Manager.customBitmapFont,
            fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            text: inputAction.ToString().Replace("_", " "), // MOVE_LEFT -> MOVE LEFT
            fontSize: fontSize,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );
        keybindLabelObject.InitializeLabel();
        keybindLabel = keybindLabelObject.GetComponent<Label>();
        child_panelObject.AddChild(keybindLabelObject, isOverlay: true);

        // BUTTON
        GameObject_Label keybindButtonObject = new GameObject_Label(
            Game2DPlatformer.Instance.GraphicsDevice,
            isButton: true,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("KeyBindButton"),
            buttonColor: !isLocked ? Color.White : new Color(150, 150, 150, 255),
            buttonWidth: (int)(totalWidth - 2 * offSet),
            buttonHeight: labelHeight,
            textColor: Color.Black,
            font: JSON_Manager.customBitmapFont,
            fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            text: inputBindingKeyName,
            fontSize: GameConstantsAndValues.FONT_SIZE_M,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            sliceBorderSize: 10
        );
        keybindButtonObject.InitializeLabel();
        child_panelObject.AddChild(keybindButtonObject, isOverlay: true);

        _button = keybindButtonObject.GetComponent<Button>();

        UpdateText(null);

        KeyBindRebindingComponent keyBindRebindingComponent = new KeyBindRebindingComponent();
        AddComponent(keyBindRebindingComponent);

        if (!isLocked)
        {
            Button_HoverColorChange.AddSoundEffectAndOnClickAction(
                button: _button,
                action: (parameters) =>
                {
                    UpdateText("WAITING");
                    keyBindRebindingComponent.StartRebind(action: inputAction, component: this);
                },
                parameters: []
            );
        }
    }

    /// <summary>
    /// Handles resizing text, button, panel, and re-centering pivots.
    /// </summary>
    public void UpdateText(string text)
    {
        SpriteTextComponent stc = _button.textField.spriteTextComponent;

        if (text == null)
        {
            binding = KeyBindManager.Instance.GetBinding(action);
            if (!isLocked) stc.text = binding.ToString();

            if (binding.Type == InputType.Keyboard && binding.Key == Keys.None)
            {
                _button.textField.spriteTextComponent.text = "UNBOUND";
                _button.textField.spriteTextComponent.textColor = Color.Red;
            }
            else
            {
                // Reset to normal colors for assigned keys
                _button.textField.spriteTextComponent.textColor = Color.Black;
            }
        }
        else
        {
            stc.text = text;
        }

        stc.fontSize = originalFontSize;
        stc.width = int.MaxValue;
        var (textWidth, textHeight) = stc.MeasureText();

        int offSet = 2;

        textWidth = Math.Min(maxButtonWidth, textWidth);
        float newWidth = labelWidth + textWidth + 15 + 3 * offSet;

        // Panel + button update
        childObjectPanel.width = newWidth;
        _button.width = textWidth + 15;
        stc.width = _button.width;

        // Recenter panel relative to parent
        childObjectPanel.gameObject.transform.localPosition =
            new Vector2(originalLocalPosition.X + newWidth / 2, 0);

        // Pivots
        PivotCentering.UpdatePivot(
            childObjectPanel,
            child: _button,
            childTransform: _button.gameObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.CenterRight,
            offSet: new Vector2(-offSet, 0)
        );

        PivotCentering.UpdatePivot(
            childObjectPanel,
            child: keybindLabel,
            childTransform: keybindLabel.gameObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.CenterLeft,
            offSet: new Vector2(offSet, 0)
        );
    }
}
