using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class HUD_PlayerAbilites
{
    public static void Add_HUD_PlayerAbilites(Scene scene)
    {
        // TESTING OVERLAY RENDERER
        int iconSize = 70;

        int width = iconSize + 10;
        int height = (iconSize + 10) * 3 + 10;

        GameObject abilityToolbar = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            layerDepth: 0,
            sliceBorderSize: 12
        );

        abilityToolbar.transform.localPosition = new Vector2(width / 2 + 10, height / 2 + 10 + 100);

        scene.AddGameObjectToScene(abilityToolbar, isOverlay: true);
        
        GameObject overlayObject = PrefabObjectSliderWithLabels.PanelObject(
            width: iconSize,
            height: iconSize,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("SettingsButton"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );
        AbilityUI_Attack abilityUI_Attack = new AbilityUI_Attack(artImageScale: new Vector2(0.8f, 0.8f));
        overlayObject.AddComponent(abilityUI_Attack);

        abilityToolbar.AddChild(overlayObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            parentSprite: abilityToolbar.GetComponent<Panel>(),
            overlayObject.GetComponent<Panel>(),
            overlayObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopCenter,
            offSet: new Vector2(0, 10)
        );

        abilityUI_Attack.InitializeUI();

        // 2) Ability Weapon Swap
        overlayObject = PrefabObjectSliderWithLabels.PanelObject(
            width: iconSize,
            height: iconSize,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("SettingsButton"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );
        AbilityUI_WeaponSwap abilityUI_WeaponSwap = new AbilityUI_WeaponSwap(artImageScale: new Vector2(0.8f, 0.8f));
        overlayObject.AddComponent(abilityUI_WeaponSwap);

        abilityToolbar.AddChild(overlayObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            parentSprite: abilityToolbar.GetComponent<Panel>(),
            overlayObject.GetComponent<Panel>(),
            overlayObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopCenter,
            offSet: new Vector2(0, 20 + iconSize)
        );

        abilityUI_WeaponSwap.InitializeUI();

        // 4) Ability Next Imbued Element
        overlayObject = PrefabObjectSliderWithLabels.PanelObject(
            width: iconSize,
            height: iconSize,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("SettingsButton"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );
        AbilityUI_ElementSwap abilityUI_currElementSwap = new AbilityUI_ElementSwap(IndexedElement.Next, artImageScale: new Vector2(0.8f, 0.8f));
        overlayObject.AddComponent(abilityUI_currElementSwap);

        abilityToolbar.AddChild(overlayObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            parentSprite: abilityToolbar.GetComponent<Panel>(),
            overlayObject.GetComponent<Panel>(),
            overlayObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopCenter,
            offSet: new Vector2(0, 30 + iconSize * 2)
        );

        abilityUI_currElementSwap.InitializeUI();



        // R ability
        GameObject abilityToolbar1 = PrefabObjectSliderWithLabels.PanelObject(
            width: iconSize + 10,
            height: iconSize + 10,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            layerDepth: 0,
            sliceBorderSize: 12
        );
        abilityToolbar1.transform.localPosition = new Vector2((iconSize + 10) / 2 + 10 + width + 10, (iconSize + 10) / 2 + 10 + 100);
        scene.AddGameObjectToScene(abilityToolbar1, isOverlay: true);

        overlayObject = PrefabObjectSliderWithLabels.PanelObject(
            width: iconSize,
            height: iconSize,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("SettingsButton"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );

        AbilityUI_SpecialAbility abilityUI_SpecialAbility = new AbilityUI_SpecialAbility(artImageScale: new Vector2(0.4f, 0.4f));
        overlayObject.AddComponent(abilityUI_SpecialAbility);

        abilityToolbar1.AddChild(overlayObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            parentSprite: abilityToolbar1.GetComponent<Panel>(),
            overlayObject.GetComponent<Panel>(),
            overlayObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.Center
        );

        abilityUI_SpecialAbility.InitializeUI();        
    }
}
