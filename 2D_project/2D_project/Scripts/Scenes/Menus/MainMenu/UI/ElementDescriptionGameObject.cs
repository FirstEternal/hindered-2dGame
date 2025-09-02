using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class ElementDescriptionGameObject : GameObject
{
    Weapon.ImbuedElement elementType;

    SpriteTextComponent elementText;
    SpriteTextComponent specialEffect;
    SpriteTextComponent chargingBladeAbility;
    SpriteTextComponent chargingBowAbility;

    Sprite elementSprite;
    Sprite bladeTooltipSprite;
    Sprite bowTooltipSprite;

    private static readonly Dictionary<Weapon.ImbuedElement, Color> ElementColorsDict = new()
{
    { Weapon.ImbuedElement.Burner, Color.Red },                  // Fire
    { Weapon.ImbuedElement.Boulderer, new Color(218, 165, 32) }, // GoldenRod (brownish yellow)
    { Weapon.ImbuedElement.Grasser, new Color(50, 205, 50) },    // LimeGreen (grass-like)
    { Weapon.ImbuedElement.Shader, new Color(105, 105, 105) },   // DimGray (shade/dark)
    { Weapon.ImbuedElement.Thunderer, new Color(218, 112, 214) },// Orchid (pinkish purple)
    { Weapon.ImbuedElement.Froster, new Color(224, 255, 255) },  // LightCyan (icy frost)
    { Weapon.ImbuedElement.Drowner, new Color(30, 144, 255) },                // Water/down
};

    private Dictionary<Weapon.ImbuedElement, string[]> descriptionDict = new Dictionary<Weapon.ImbuedElement, string[]>();
    public ElementDescriptionGameObject(Panel parentPanel)
    {
        CreateVisuals(parentPanel);

        descriptionDict[Weapon.ImbuedElement.Burner] = new string[3]
        {
            "Hitting an Enemy, inflicts </red>burn</black> for 3 seconds",
            "Hold to perform a Dash",
            "Hold to increase projectile size and damage"
        };

        descriptionDict[Weapon.ImbuedElement.Boulderer] = new string[3]
        {
            "TODO",
            "Hold to regenerate shield",
            "Hold to increase projectile size, speed, damage"
        };
        descriptionDict[Weapon.ImbuedElement.Drowner] = new string[3]
        {
            "TODO",
            "Hold to perform a whip attack - TODO(mybe make it into a grappling hook)",
            "Hold to increase projectile count, size and  damage"
        };

        descriptionDict[Weapon.ImbuedElement.Froster] = new string[3]
        {
            "TODO",
            "Hold to perform a defensive block - TODO",
            "Hold to increase projectile count, size and  damage"
        };
        descriptionDict[Weapon.ImbuedElement.Grasser] = new string[3]
        {
            "projectiles heal 3 on hit",
            "Hold to perform a healing slash",
            "Hold to increase projectile count, size, damage"
        };

        descriptionDict[Weapon.ImbuedElement.Shader] = new string[3]
        {
            "projectiles ignore terrain",
            "Hold to create a </purple>shadow clone</black>\nunlock ability\n R; swap position",
            "Hold to increase projectile size, speed, damage"
        };
        descriptionDict[Weapon.ImbuedElement.Thunderer] = new string[3]
        {
            "TODO",
            "Hold to perform a Flying strike",
            "Hold to increase projectile count, size, damage"
        };
    }

    public void AssignValues(Weapon.ImbuedElement elementType)
    {
        this.elementType = elementType;

        string[] descriptionStrings = descriptionDict[elementType];

        elementText.text = this.elementType.ToString().ToUpper();
        elementText.textColor = ElementColorsDict[elementType];
        specialEffect.text = descriptionStrings[0].ToUpper();
        chargingBladeAbility.text = descriptionStrings[1].ToUpper();
        chargingBowAbility.text = descriptionStrings[2].ToUpper();

        elementSprite.sourceRectangle = JSON_Manager.GetUITile($"Element_{elementType}");
        bladeTooltipSprite.sourceRectangle = JSON_Manager.GetUITile($"Tooltip_Blade_{elementType.ToString()}");
        bowTooltipSprite.sourceRectangle = JSON_Manager.GetUITile($"Tooltip_Bow_{elementType.ToString()}");
    }

    public void CreateVisuals(Panel parentPanel)
    {
        CreateTransform();

        elementType = Weapon.ImbuedElement.Boulderer;

        int backgroundWidth = (int)(GameWindow.Instance.windowHeight * 0.9);
        int backgroundHeight = (int)(GameWindow.Instance.windowHeight * 0.79);
        GameObject backgroundPanel = PrefabObjectSliderWithLabels.PanelObject(
            width: backgroundWidth,
            height: backgroundHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            sliceBorderSize: 12
        );
        AddChild(backgroundPanel, isOverlay: true);

        PivotCentering.UpdatePivot(
            parentSprite: parentPanel,
            child: backgroundPanel.GetComponent<Panel>(),
            childTransform: backgroundPanel.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopLeft,
            offSet: new Vector2(10, 10)
        );

        // element visuals
        int elementWidth = 60;
        int elementHeight = 60;
        GameObject elementBackgroundPanel1 = PrefabObjectSliderWithLabels.PanelObject(
            width: elementHeight + 20,
            height: elementHeight + 20,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_GrayFull,
            sliceBorderSize : 12
        );

        backgroundPanel.AddChild(elementBackgroundPanel1, isOverlay: true);
        PivotCentering.UpdatePivot(
            backgroundPanel.GetComponent<Panel>(),
            elementBackgroundPanel1.GetComponent<Panel>(),
            elementBackgroundPanel1.transform,
            PivotCentering.Enum_Pivot.TopLeft,
            offSet: new Vector2(10, 10)
        );

        GameObject elementPanel = PrefabObjectSliderWithLabels.PanelObject(
            width: elementWidth,
            height: elementHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile($"Element_{elementType}"),
            panelColor: Color.White
        );

        elementSprite = elementPanel.GetComponent<Sprite>();

        elementBackgroundPanel1.AddChild(elementPanel, isOverlay: true);

        // Title
        int titleWidth = 140;
        int titleHeight = 30;
        Color descriptionPanelColor = GameConstantsAndValues.PanelColor_lightBlue1;
        Color descriptionPanelTextColor = Color.Black;
        GameObject titleBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: titleWidth,
            height: titleHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull
        );

        elementPanel.AddChild(titleBackgroundObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            elementPanel.GetComponent<Panel>(),
            titleBackgroundObject.GetComponent<Panel>(),
            titleBackgroundObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopRight,
            offSet: new Vector2(titleWidth + 20, -10)
        );

        elementText = new SpriteTextComponent(width: titleWidth, height: titleHeight, JSON_Manager.customBitmapFont,
            $"ELEMENT - {elementType.ToString().ToUpper()}", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, color: Color.Yellow,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );

        GameObject_TextField titleTextField = new GameObject_TextField(elementText);
        titleBackgroundObject.AddChild(titleTextField, isOverlay: true);

        GameObject specialEffectDescritpionObject = Menu.LabelGameObject(
            labelText: "",
            labelWidth: 540,
            labelHeight: elementHeight + 20 - titleHeight,
            labelColor: descriptionPanelColor,
            fontSize: GameConstantsAndValues.FONT_SIZE_S,
            parentPanel: elementPanel.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.BottomLeft,
            curr_x_offset: elementWidth + 20,
            curr_y_offset: 10,
            textColor: descriptionPanelTextColor,
            sliceBorderSize: 12,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );

        specialEffect = specialEffectDescritpionObject.GetComponent<Label>().textField.spriteTextComponent;
        specialEffect.cutWordOnly = true;
        specialEffectDescritpionObject.GetComponent<Label>().sourceRectangle = JSON_Manager.GetUITile("Button3");

        int imgWidth = 375;
        int imgHeight = 225;
        int descriptionWidth = 250;
        int descriptionHeight= imgHeight;
        // description visuals
        GameObject bladeImageObject = PrefabObjectSliderWithLabels.PanelObject(
            width: imgWidth,
            height: imgHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile($"Tooltip_Blade_{elementType.ToString()}"),
            panelColor: Color.White
        );

        bladeTooltipSprite = bladeImageObject.GetComponent<Sprite>();
        backgroundPanel.AddChild(bladeImageObject, isOverlay: true);

        GameObject bladeDescritpionObject = Menu.LabelGameObject(
            labelText: "",
            labelWidth: descriptionWidth,
            labelHeight: descriptionHeight,
            labelColor: descriptionPanelColor,
            fontSize: GameConstantsAndValues.FONT_SIZE_M,
            parentPanel: backgroundPanel.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.BottomRight,
            curr_x_offset: -10,
            curr_y_offset: -10 - imgHeight - 10,
            textColor: descriptionPanelTextColor,
            sliceBorderSize: 12,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );

        chargingBladeAbility = bladeDescritpionObject.GetComponent<Label>().textField.spriteTextComponent;
        chargingBladeAbility.cutWordOnly = true;
        bladeDescritpionObject.GetComponent<Label>().sourceRectangle = JSON_Manager.GetUITile("Button3");
        
        PivotCentering.UpdatePivot(
               backgroundPanel.GetComponent<Panel>(),
               bladeImageObject.GetComponent<Panel>(),
               bladeImageObject.transform,
               PivotCentering.Enum_Pivot.BottomLeft,
               offSet: new Vector2(10, -10 - imgHeight - 10)
        );
        
        // bow section
        GameObject bowImageObject = PrefabObjectSliderWithLabels.PanelObject(
            width: imgWidth,
            height: imgHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile($"Tooltip_Bow_{elementType.ToString()}"),
            panelColor: Color.White
        );

        bowTooltipSprite = bowImageObject.GetComponent<Sprite>();
        backgroundPanel.AddChild(bowImageObject, isOverlay: true);

        GameObject bowDescritpionObject = Menu.LabelGameObject(
            labelText: "",
            labelWidth: descriptionWidth,
            labelHeight: descriptionHeight,
            labelColor: descriptionPanelColor,
            fontSize: GameConstantsAndValues.FONT_SIZE_M,
            parentPanel: backgroundPanel.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.BottomRight,
            curr_x_offset: -10,
            curr_y_offset: -10,
            textColor: descriptionPanelTextColor,
            sliceBorderSize: 12,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            centerY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle
        );

        chargingBowAbility = bowDescritpionObject.GetComponent<Label>().textField.spriteTextComponent;
        chargingBowAbility.cutWordOnly = true;
        bowDescritpionObject.GetComponent<Label>().sourceRectangle = JSON_Manager.GetUITile("Button3");

        PivotCentering.UpdatePivot(
            backgroundPanel.GetComponent<Panel>(),
            bowImageObject.GetComponent<Panel>(),
            bowImageObject.transform,
            PivotCentering.Enum_Pivot.BottomLeft,
            offSet: new Vector2(10, -10)
        );

        // hide this interface
        SetActive(false);
        SetActiveWithParentEnabled = false;
    }
}
