using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

internal class PrefabObjectSpriteDropdown : GameObject
{
    public readonly Texture2D texture;
    public readonly Rectangle[] items;
    public int selectedItemIndex { get; private set; }
    private Sprite selectedItemSprite;

    private GameObject SubItemsObject;
    private SpriteTextComponent expandCollapseSpriteText;

    public Action onDropdownSelect;
    public Action<int> onItemHoverEnter;
    internal Action<int> onItemHoverExit;

    public Action<PrefabObjectSpriteDropdown> onExpand;

    public void ForcedCollapse()
    {
        Collapse();
    }

    public PrefabObjectSpriteDropdown(Texture2D texture, Rectangle[] items, int initialItemIndex, GameObject parent, int totalWidth, int totalHeight, int id = -1, string tag = "", int direction = 1) : base(id, tag)
    {
        if (items == null || items.Length == 0) throw new NullReferenceException("dropdown's items cannot null nor empty");

        CreateTransform();

        parent.AddChild(this, isOverlay: true);

        this.items = items;
        selectedItemIndex = initialItemIndex;

        this.texture = texture;
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
               text: "",
               fontSize: fontSize,
               centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
               centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
               sliceBorderSize: 12
        );

        drowpdownObject.InitializeLabel();
        AddChild(drowpdownObject, isOverlay: true);

        GameObject imageObject = PrefabObjectSliderWithLabels.PanelObject(
            width: totalWidth - 10,
            height: totalHeight - 10,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: items[initialItemIndex],
            panelColor: Color.White
        );

        selectedItemSprite = imageObject.GetComponent<Sprite>();

        drowpdownObject.AddChild(imageObject, isOverlay: true);

        GameObject collapseGameObject = Menu.LabelGameObject(
            labelText: "▼",
            labelWidth: 20,
            labelHeight: 20,
            fontSize: 20,
            labelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            curr_x_offset: -5,
            curr_y_offset: -6,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Top,
            parentPanel: imageObject.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.TopLeft
        );

        collapseGameObject.GetComponent<Label>().textField.transform.localPosition = new Vector2(-1, -4);
        expandCollapseSpriteText = collapseGameObject.GetComponent<Label>().textField.spriteTextComponent;
        drowpdownObject.AddChild(collapseGameObject, isOverlay: true);


        int subItemHeight = (int)(totalHeight * 0.7f);
        int fontsize = (int)(fontSize * 0.6f);
        int seperatingDistance = 2;

        int fullSubPanelHeight = (subItemHeight + seperatingDistance) * items.Length + seperatingDistance;
        SubItemsObject = PrefabObjectSliderWithLabels.PanelObject(
            width: totalWidth,
            height: fullSubPanelHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("background"),
            panelColor: GameConstantsAndValues.PanelColor_GrayFull
        );

        SubItemsObject.transform.localPosition = new Vector2(0, direction * (fullSubPanelHeight / 2 + subItemHeight - 5 - 2));

        SubItemsObject.SetActiveWithParentEnabled = false;
        drowpdownObject.AddChild(SubItemsObject, isOverlay: true);

        for (int i = 0; i < items.Length; i++)
        {
            SubItemObject(seperatingDistance: seperatingDistance, itemIndex: i,
                parent: SubItemsObject,
                selectedSprite: selectedItemSprite,
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
                    onExpand?.Invoke(this);
                }
                else Collapse();

            },
            parameters: [SubItemsObject]
        );
    }

    private GameObject SubItemObject(int seperatingDistance, int itemIndex, GameObject parent, Sprite selectedSprite, int width, int height, int fontSize, int topPosY)
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
            text: "", //items[itemIndex],
            fontSize: fontSize,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );

        parent.AddChild(buttonObject, isOverlay: true);
        buttonObject.transform.localPosition = new Vector2(0, -topPosY - height / 2 + (itemIndex + 1) * (height + seperatingDistance));

        // TODO ADD TEXTURE
        /*
        GameObject imageObject = new GameObject();
        imageObject.CreateTransform();
        Sprite sprite = new Sprite(JSON_Manager.uiSpriteSheet, colorTint: Color.White);
        sprite.sourceRectangle = items[itemIndex];
        sprite.origin = new Vector2(sprite.sourceRectangle.Width / 2, sprite.sourceRectangle.Height / 2);
        imageObject.AddComponent(sprite);
        buttonObject.AddChild(imageObject, isOverlay: true);
        */
        GameObject imageObject = PrefabObjectSliderWithLabels.PanelObject(
            width: height - 10,
            height: height - 10,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: items[itemIndex],
            panelColor: Color.White
        );

        buttonObject.AddChild(imageObject, isOverlay: true);

        buttonObject.InitializeLabel();
        buttonObject.GetComponent<Button>().AssignOnHoverEnterAction(
            onHoverEnterAction: (parameters) => 
            { 
                onItemHoverEnter?.Invoke(itemIndex);  
            }, 
            parameters: []
        );

        buttonObject.GetComponent<Button>().AssignOnHoverExitAction(
            onHoverExitAction: (parameters) =>
            {
                onItemHoverExit?.Invoke(itemIndex);
            },
            parameters: []
        );

        buttonObject.GetComponent<Button>().AssignOnClickAction(
            onClickAction: (parameters) =>
            {
                // change to sprite
                // -> also update all other options
                selectedItemIndex = itemIndex;
                selectedSprite.sourceRectangle = items[itemIndex];
                selectedSprite.origin = new Vector2(selectedSprite.sourceRectangle.Width / 2, selectedSprite.sourceRectangle.Height / 2);
                Collapse();

                onDropdownSelect?.Invoke();
            },
            parameters: []
        );
        return null;
    }

    public void ManualUpdate(int itemIndex)
    {
        selectedItemIndex = itemIndex;
        selectedItemSprite.sourceRectangle = items[itemIndex];
        selectedItemSprite.origin = new Vector2(selectedItemSprite.sourceRectangle.Width / 2, selectedItemSprite.sourceRectangle.Height / 2);
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
