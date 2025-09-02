using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class PrefabObjectStringDropdown : GameObject
{
    public readonly string[] items;
    public int selectedItemIndex { get; private set; }

    private GameObject SubItemsObject;
    private SpriteTextComponent expandCollapseSpriteText;

    public Action onDropdownSelect;
    public PrefabObjectStringDropdown(string[] items, int initialItemIndex, GameObject parent, int totalWidth, int totalHeight, int id = -1, string tag = "") : base(id, tag)
    {
        if (items == null || items.Length == 0) throw new NullReferenceException("dropdown's items cannot null nor empty");

        CreateTransform();

        parent.AddChild(this, isOverlay: true);

        this.items = items;
        selectedItemIndex = initialItemIndex;

        Rectangle selectedItemRectangle = JSON_Manager.GetUITile("Button3");
        Color textColor = Color.White;//Color.Black;
        int fontSize = GameConstantsAndValues.FONT_SIZE_HM;
        GameObject_Label drowpdownObject = new GameObject_Label(
               Game2DPlatformer.Instance.GraphicsDevice,
               isButton: true,
               texture2D: JSON_Manager.uiSpriteSheet,
               sourceRectangle: selectedItemRectangle,
               buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
               buttonWidth: totalWidth,
               buttonHeight: totalHeight,
               textColor: textColor,
               font: JSON_Manager.customBitmapFont,
               fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
               text: items[initialItemIndex],
               fontSize: fontSize,
               centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
               centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );
        drowpdownObject.InitializeLabel();
        AddChild(drowpdownObject, isOverlay: true);

        expandCollapseSpriteText = new SpriteTextComponent(width: totalHeight /*square*/, height: totalHeight, JSON_Manager.customBitmapFont,
          text: "▼", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
          textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Right,
          textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
          fontSize: fontSize, spacingX: 5, color: Color.White,
          graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );

        GameObject_TextField textField = new GameObject_TextField(expandCollapseSpriteText);
        drowpdownObject.AddChild(textField, isOverlay: true);

        PivotCentering.UpdatePivot(
            drowpdownObject.GetComponent<Button>(),
            child: textField.spriteTextComponent,
            childTransform: textField.transform,
            pivotPosition: PivotCentering.Enum_Pivot.CenterRight,
            offSet: new Vector2(-5, -5)
        );

        int subItemHeight = (int)(totalHeight * 0.7f);
        int fontsize = (int)(fontSize * 0.7f);
        int seperatingDistance = 2;

        int fullSubPanelHeight = (subItemHeight + seperatingDistance) * items.Length + seperatingDistance;
        SubItemsObject = PrefabObjectSliderWithLabels.PanelObject(
            width: totalWidth,
            height: fullSubPanelHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("background"),
            panelColor: GameConstantsAndValues.PanelColor_GrayFull
        );

        SubItemsObject.transform.localPosition = new Vector2(0, fullSubPanelHeight / 2 + subItemHeight - 5 - 2);

        SubItemsObject.SetActiveWithParentEnabled = false;
        drowpdownObject.AddChild(SubItemsObject, isOverlay: true);

        for (int i = 0; i < items.Length; i++)
        {
            SubItemObject(seperatingDistance: seperatingDistance, itemIndex: i,
                parent: SubItemsObject,
                selectedText: drowpdownObject.GetComponent<Button>().textField.spriteTextComponent,
                width: totalWidth - 6, subItemHeight, fontsize, topPosY: fullSubPanelHeight / 2
            );
        }

        SubItemsObject.SetActive(false);


        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: drowpdownObject.GetComponent<Button>(),
            action: (parameters) =>
            {
                if (!SubItemsObject.isActive)
                {
                    Expand();
                }
                else Collapse();
            },
            parameters: [SubItemsObject]
        );
    }

    private GameObject SubItemObject(int seperatingDistance, int itemIndex, GameObject parent, SpriteTextComponent selectedText, int width, int height, int fontSize, int topPosY)
    {
        Rectangle itemRectangle = JSON_Manager.GetUITile("Button1");
        Color textColor = Color.White;//Color.Black;

        GameObject_Label buttonObject = new GameObject_Label(
            Game2DPlatformer.Instance.GraphicsDevice,
            isButton: true,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: itemRectangle,
            buttonColor: GameConstantsAndValues.PanelColor_DarkBlue,
            buttonWidth: width,
            buttonHeight: height,
            textColor: textColor,
            font: JSON_Manager.customBitmapFont,
            fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            text: items[itemIndex],
            fontSize: fontSize,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );

        parent.AddChild(buttonObject, isOverlay: true);
        buttonObject.transform.localPosition = new Vector2(0, -topPosY - height / 2 + (itemIndex + 1) * (height + seperatingDistance));

        buttonObject.InitializeLabel();
        buttonObject.GetComponent<Button>().AssignOnClickAction(
            onClickAction: (parameters) =>
            {
                selectedItemIndex = itemIndex;
                selectedText.text = items[itemIndex];
                Collapse();

                onDropdownSelect?.Invoke();
            },
            parameters: []
        );
        return null;
    }

    private void Expand()
    {
        SubItemsObject.SetActive(true);
        expandCollapseSpriteText.text = "▲";
    }

    private void Collapse()
    {
        SubItemsObject.SetActive(false);
        expandCollapseSpriteText.text = "▼";
    }
}
