using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

internal class SettingsToolBar
{
    public static SettingsToolBar Instance;

    private PrefabObjectStringDropdown resolutionDropdown;

    List<PrefabObjectSliderWithLabels[]> SoundSliders = new List<PrefabObjectSliderWithLabels[]>();

    public SettingsToolBar()
    {
        if (Instance is not null) return;
        Instance = this;
    }

    public GameObject CreateMenuSettingsPanelObject(GameObject parent, Vector2 position)
    {
        GameObject settingsObject = new GameObject();
        settingsObject.CreateTransform(position);

        parent.AddChild(settingsObject, isOverlay: true);

        int toolbarWidth = 60;
        int toolbarHeight = 130;
        Vector2 toolbarPos = new Vector2(GameWindow.Instance.windowWidth - toolbarWidth / 2 - 10, toolbarHeight / 2 + 10);

        GameObject exitToolbar = PrefabObjectSliderWithLabels.PanelObject(
           width: toolbarWidth,
           height: toolbarHeight,
           texture2D: JSON_Manager.uiSpriteSheet,
           sourceRectangle: JSON_Manager.GetUITile("Button3"),
           panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
           layerDepth: 0,
           sliceBorderSize: 12
        );
        settingsObject.AddChild(exitToolbar, isOverlay: true);

        int buttonSize = 50;
        // exit button
        Button exitButtonObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: 0,
            curr_y_offset: 10,
            exitToolbar.GetComponent<Panel>(), PivotCentering.Enum_Pivot.TopCenter
        ).GetComponent<Button>();

        exitButtonObject.sourceRectangle = JSON_Manager.GetUITile("SettingsButton");
        GameObject exitSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 36,
            height: 36,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("XButtonIcon"),
            panelColor: Color.White
        );
        exitButtonObject.gameObject.AddChild(exitSpriteObject, isOverlay: true);


        // settings button
        Button settingsButtonObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: 0,
            curr_y_offset: -10,
            exitToolbar.GetComponent<Panel>(), PivotCentering.Enum_Pivot.BottomCenter
        ).GetComponent<Button>();

        settingsButtonObject.sourceRectangle = JSON_Manager.GetUITile("SettingsButton");
        GameObject settingsSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 36,
            height: 36,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("SettingsButtonIcon"),
            panelColor: Color.White
        );
        settingsButtonObject.gameObject.AddChild(settingsSpriteObject, isOverlay: true);


        Vector2 locPos = toolbarPos - exitToolbar.transform.globalPosition;
        exitToolbar.transform.globalPosition = Vector2.Zero;
        exitToolbar.transform.localPosition = locPos;


        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: exitButtonObject,
            action: FullMenu.ExitGame(),
            parameters: []
        );


        // Settings panel with resolution, sound, keybinds
        GameObject SettingPanel = CreateSettingsSubObject(exitToolbar, position: Vector2.Zero, isMainMenuSettings: true, menuName: "SETTINGS");

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: settingsButtonObject,
            action: (parameters) =>
            {
                SettingPanel.SetActive(!SettingPanel.isActive);
                FullMenu.Instance.activeMenu.SetActive(!SettingPanel.isActive);
            },
            parameters: []
        );

        return exitToolbar;
    }

    public GameObject CreateGameSettingsPanelObject(GameObject parent, Vector2 position, string menuName)
    {

        // Settings panel with resolution, sound, keybinds
        GameObject SettingPanel = CreateSettingsSubObject(parent, position: Vector2.Zero, menuName: menuName, isMainMenuSettings: false);
        parent.AddChild(SettingPanel, isOverlay: true);

        return SettingPanel;
    }
    //
    private GameObject CreateResolutionPanelObject(GameObject parent, Vector2 position)
    {
        GameObject resolutionPanel = new GameObject();
        resolutionPanel.CreateTransform();
        parent.AddChild(resolutionPanel, isOverlay: true);

        float posX = -140;

        resolutionDropdown = ResolutionDropdown(resolutionPanel, posX, -250);

        //resolutionPanel.SetActiveWithParentEnabled = false; // this one should be first to be active
        return resolutionPanel;
    }

    private void UpdateSoundSliders()
    {
        foreach (PrefabObjectSliderWithLabels[] soundSliderPrefabs in SoundSliders)
        {
            soundSliderPrefabs[0].ForcedValueUpdate(value: SoundController.instance.volume_master);
            soundSliderPrefabs[1].ForcedValueUpdate(value: SoundController.instance.volume_soundEffects);
            soundSliderPrefabs[2].ForcedValueUpdate(value: SoundController.instance.volume_music);
        }
    }

    private GameObject CreateSoundPanelObject(GameObject parent, Vector2 position, bool isMainMenuSettings)
    {
        GameObject soundPanel = new GameObject();
        soundPanel.CreateTransform();
        parent.AddChild(soundPanel, isOverlay: true);

        float posX = -140;

        PrefabObjectSliderWithLabels masterVolumeSliderObject = SoundSlider(volumeName: "MASTER", initialValue: SoundController.instance.volume_master, soundPanel, posX, -250);
        Slider masterVolumeSlider = masterVolumeSliderObject.slider;

        masterVolumeSlider.AssignOnValueChangeAction((parameters) =>
        {
            SoundController.instance.volume_master = masterVolumeSlider.slideValue;

        }, parameters: null);

        PrefabObjectSliderWithLabels vfxVolumeSliderObject = SoundSlider(volumeName: "VFX", initialValue: SoundController.instance.volume_soundEffects, soundPanel, posX, masterVolumeSliderObject.transform.localPosition.Y);
        Slider vfxVolumeSlider = vfxVolumeSliderObject.slider;

        vfxVolumeSlider.AssignOnValueChangeAction((parameters) =>
        {
            SoundController.instance.volume_soundEffects = vfxVolumeSlider.slideValue;
        }, parameters: null);

        PrefabObjectSliderWithLabels musicVolumeSliderObject = SoundSlider(volumeName: "MUSIC", initialValue: SoundController.instance.volume_music, soundPanel, posX, vfxVolumeSliderObject.transform.localPosition.Y);
        Slider musicVolumeSlider = musicVolumeSliderObject.slider;

        musicVolumeSlider.AssignOnValueChangeAction((parameters) =>
        {
            SoundController.instance.volume_music = musicVolumeSlider.slideValue;
        }, parameters: null);

        soundPanel.SetActiveWithParentEnabled = false;

        PrefabObjectSliderWithLabels[] sliders = new PrefabObjectSliderWithLabels[] { masterVolumeSliderObject, vfxVolumeSliderObject, musicVolumeSliderObject };

        SoundSliders.Add(sliders);

        SoundController.instance.OnVolumeChanged -= UpdateSoundSliders;
        SoundController.instance.OnVolumeChanged += UpdateSoundSliders;
        return soundPanel;

    }

    private GameObject CreateKeybindPanelObject(GameObject parent, Vector2 position)
    {

        GameObject keyBindPanel = new GameObject();
        keyBindPanel.CreateTransform();
        parent.AddChild(keyBindPanel, isOverlay: true);

        float posX = -140;

        int width = 150;
        int height = 30;

        // Movement section
        GameObject textBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            sliceBorderSize: 12
        );

        keyBindPanel.AddChild(textBackgroundObject, isOverlay: true);

        textBackgroundObject.transform.localPosition = new Vector2(-135, -180);

        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(width: width, height: height, JSON_Manager.customBitmapFont,
            text: "MOVEMENT", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        GameObject_TextField textField = new GameObject_TextField(spriteTextComponent);
        textBackgroundObject.AddChild(textField, isOverlay: true);

        // keyboard
        int labelWidth = 70;
        float localPositionX = 40;
        PrefabObjectKeyBindWithLabel keyBind_MoveLeft = new PrefabObjectKeyBindWithLabel(
            GameAction.LEFT,
            KeyBindManager.Instance.GetBinding(GameAction.LEFT),
            localPosition: new Vector2(localPositionX, 50), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        PrefabObjectKeyBindWithLabel keyBind_MoveRight = new PrefabObjectKeyBindWithLabel(
            GameAction.RIGHT,
            KeyBindManager.Instance.GetBinding(GameAction.RIGHT),
            localPosition: new Vector2(localPositionX, 90), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        PrefabObjectKeyBindWithLabel keyBind_Jump = new PrefabObjectKeyBindWithLabel(
            GameAction.JUMP,
            KeyBindManager.Instance.GetBinding(GameAction.JUMP),
            localPosition: new Vector2(localPositionX, 130), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        // Ability section
        textBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
              width: width,
              height: height,
              texture2D: JSON_Manager.uiSpriteSheet,
              sourceRectangle: JSON_Manager.GetUITile("Button3"),
              panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
              sliceBorderSize: 12
          );

        keyBindPanel.AddChild(textBackgroundObject, isOverlay: true);

        textBackgroundObject.transform.localPosition = new Vector2(-135, 0);

        spriteTextComponent = new SpriteTextComponent(width: width, height: height, JSON_Manager.customBitmapFont,
            text: "ABILITIES", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        textField = new GameObject_TextField(spriteTextComponent);
        textBackgroundObject.AddChild(textField, isOverlay: true);

        labelWidth = 190;
        localPositionX = 100;
        PrefabObjectKeyBindWithLabel keyBind_SwapWeapon = new PrefabObjectKeyBindWithLabel(
            GameAction.SWAP_WEAPON,
            KeyBindManager.Instance.GetBinding(GameAction.SWAP_WEAPON),
            localPosition: new Vector2(localPositionX, 50), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        PrefabObjectKeyBindWithLabel keyBind_ChangeElement = new PrefabObjectKeyBindWithLabel(
            GameAction.CHANGE_ELEMENT,
            KeyBindManager.Instance.GetBinding(GameAction.CHANGE_ELEMENT),
            localPosition: new Vector2(localPositionX, 90), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        PrefabObjectKeyBindWithLabel keyBind_SpecialAbility = new PrefabObjectKeyBindWithLabel(
            GameAction.SPECIAL_ABILITY,
            KeyBindManager.Instance.GetBinding(GameAction.SPECIAL_ABILITY),
            localPosition: new Vector2(localPositionX, 130), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        // Attacking section
        textBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
              width: width,
              height: height,
              texture2D: JSON_Manager.uiSpriteSheet,
              sourceRectangle: JSON_Manager.GetUITile("Button3"),
              panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
              sliceBorderSize: 12
        );

        keyBindPanel.AddChild(textBackgroundObject, isOverlay: true);

        textBackgroundObject.transform.localPosition = new Vector2(135, -180);

        spriteTextComponent = new SpriteTextComponent(width: width, height: height, JSON_Manager.customBitmapFont,
            text: "ATTACKING", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        textField = new GameObject_TextField(spriteTextComponent);
        textBackgroundObject.AddChild(textField, isOverlay: true);

        // mouse
        labelWidth = 110;
        localPositionX = 60;
        PrefabObjectKeyBindWithLabel keyBind_Attack = new PrefabObjectKeyBindWithLabel(
            GameAction.ATTACK,
            KeyBindManager.Instance.GetBinding(GameAction.ATTACK),
            localPosition: new Vector2(localPositionX, 50), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2
        );

        PrefabObjectKeyBindWithLabel keyBind_ChargeWeapon = new PrefabObjectKeyBindWithLabel(
            inputBindingKeyName: "HOLD ATTACK",
            GameAction.CHARGING,
            KeyBindManager.Instance.GetBinding(GameAction.CHARGING),
            localPosition: new Vector2(localPositionX, 90), parent: textBackgroundObject, labelWidth: labelWidth, maxButtonWidth: 120, totalHeight: 40, spacing: 2,
            isLocked: true
        );

        keyBindPanel.SetActiveWithParentEnabled = false;
        return keyBindPanel;
    }

    private PrefabObjectStringDropdown ResolutionDropdown(GameObject resolutionPanel, float positionX, float positionY)
    {
        Dictionary<string, GameWindow.ScreenResolution> resDict = new Dictionary<string, GameWindow.ScreenResolution>
        {
            { "1280 × 720",    GameWindow.ScreenResolution.RES_1280x720 },
            { "1366 × 768",    GameWindow.ScreenResolution.RES_1366x768 },
            { "1440 × 900",    GameWindow.ScreenResolution.RES_1440x900 },
            { "1600 × 900",    GameWindow.ScreenResolution.RES_1600x900 },
            { "1920 × 1080",   GameWindow.ScreenResolution.RES_1920x1080 },
            { "2560 × 1440",   GameWindow.ScreenResolution.RES_2560x1440 },
            { "3840 × 2160",   GameWindow.ScreenResolution.RES_3840x2160 },
        };

        // stage_1
        PrefabObjectStringDropdown resolutionDropdown = new PrefabObjectStringDropdown(
            items: [
                "1280 × 720",  // seems good
                "1366 × 768",  // seems good
                "1440 × 900",  // we shall see about this one
                "1600 × 900",  // seems good
                "1920 × 1080"  // we shall see about this one
            ],
            initialItemIndex: 0,
            resolutionPanel,
            totalWidth: 280,
            totalHeight: 40
        );

        resolutionDropdown.onDropdownSelect += () =>
        {
            // update game window
            string selectedOption = resolutionDropdown.items[resolutionDropdown.selectedItemIndex];
            GameWindow.Instance.SetResolution(resDict[selectedOption]);
        };

        resolutionDropdown.transform.localPosition = new Vector2(
            positionX + 140,
            positionY + 110
        );

        int width = 150;
        int height = 30;

        GameObject textBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            sliceBorderSize: 12
        );
        resolutionDropdown.AddChild(textBackgroundObject, isOverlay: true);
        textBackgroundObject.transform.localPosition = new Vector2(0, -40);

        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(width: width, height: height, JSON_Manager.customBitmapFont,
            text: "RESOLUTION", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        GameObject_TextField textField = new GameObject_TextField(spriteTextComponent);
        textBackgroundObject.AddChild(textField, isOverlay: true);

        return resolutionDropdown;
    }

    private PrefabObjectSliderWithLabels SoundSlider(string volumeName, float initialValue, GameObject soundPanel, float positionX, float positionY)
    {
        // stage_1
        PrefabObjectSliderWithLabels masterVolumeSlider = new PrefabObjectSliderWithLabels(
            initialSlideValue: initialValue,
            parent: soundPanel,
            totalWidth: 400,
            totalHeight: 66,
            labelWidth: 66,
            leftLabelText: "",
            rightLabelText: "100", // initial value
            id: 500,
            tag: ""
        );

        masterVolumeSlider.transform.localPosition = new Vector2(
            positionX + 140,
            positionY + 100
        );

        int width = 100;
        int height = 30;

        GameObject textBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            sliceBorderSize: 12
        );
        masterVolumeSlider.AddChild(textBackgroundObject, isOverlay: true);
        textBackgroundObject.transform.localPosition = new Vector2(-130, -30);

        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(width: width, height: height, JSON_Manager.customBitmapFont,
            text: volumeName, fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        GameObject_TextField textField = new GameObject_TextField(spriteTextComponent);
        textBackgroundObject.AddChild(textField, isOverlay: true);

        return masterVolumeSlider;
    }

    private GameObject CreateSettingsSubObject(GameObject parent, Vector2 position, string menuName, bool isMainMenuSettings = false)
    {
        GameObject SettingPanel = PrefabObjectSliderWithLabels.PanelObject(
            width: (int)(GameWindow.Instance.windowWidth * 0.55f),
            height: (int)(GameWindow.Instance.windowHeight * 0.80f),
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("backgroundPanel"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );

        if (isMainMenuSettings) SettingPanel.transform.localPosition = -parent.transform.localPosition;
        parent.AddChild(SettingPanel, isOverlay: true);

        // Title
        int titleWidth = 376;
        int titleHeight = 48;
        GameObject titleBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: titleWidth,
            height: titleHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            sliceBorderSize: 12
        );

        SettingPanel.AddChild(titleBackgroundObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            SettingPanel.GetComponent<Panel>(),
            titleBackgroundObject.GetComponent<Panel>(),
            titleBackgroundObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopCenter,
            offSet: new Vector2(0, 10)
        );

        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(width: titleWidth, height: titleHeight, JSON_Manager.customBitmapFont,
            menuName, fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_H, spacingX: 5, color: Color.Silver,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );

        GameObject_TextField titleTextField = new GameObject_TextField(spriteTextComponent);
        titleBackgroundObject.AddChild(titleTextField, isOverlay: true);


        int buttonSize = 50;
        int buttonCount = 3;
        int toolbarWidth = 60;
        int toolbarHeight = 10 + buttonCount * (buttonSize + 10);

        GameObject subToolbar = PrefabObjectSliderWithLabels.PanelObject(
           width: toolbarWidth,
           height: toolbarHeight,
           texture2D: JSON_Manager.uiSpriteSheet,
           sourceRectangle: JSON_Manager.GetUITile("Button3"),
           panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
           layerDepth: 0,
           sliceBorderSize: 12
        );
        SettingPanel.AddChild(subToolbar, isOverlay: true);
        subToolbar.transform.localPosition = new Vector2((-SettingPanel.GetComponent<Panel>().width + toolbarWidth) / 2 + 10, -20);

        PivotCentering.UpdatePivot(
            SettingPanel.GetComponent<Panel>(),
            subToolbar.GetComponent<Panel>(),
            subToolbar.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopLeft,
            offSet: new Vector2(10, 10 + titleHeight)
        );


        // resolution button
        Button resolutionButtonObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: 0,
            curr_y_offset: 10,
            subToolbar.GetComponent<Panel>(), PivotCentering.Enum_Pivot.TopCenter
        ).GetComponent<Button>();

        resolutionButtonObject.sourceRectangle = JSON_Manager.GetUITile("SettingsButton");
        GameObject resolutionSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 36,
            height: 36,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("ResolutionButtonIcon"),
            panelColor: Color.White
        );
        resolutionButtonObject.gameObject.AddChild(resolutionSpriteObject, isOverlay: true);

        // settings button
        Button soundButtonObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: 0,
            curr_y_offset: 0,
            subToolbar.GetComponent<Panel>(), PivotCentering.Enum_Pivot.Center
        ).GetComponent<Button>();

        soundButtonObject.sourceRectangle = JSON_Manager.GetUITile("SettingsButton");
        GameObject soundSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 36,
            height: 36,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("SoundButtonIcon"),
            panelColor: Color.White
        );
        soundButtonObject.gameObject.AddChild(soundSpriteObject, isOverlay: true);

        // settings button
        Button keybindsObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: 0,
            curr_y_offset: -10,
            subToolbar.GetComponent<Panel>(), PivotCentering.Enum_Pivot.BottomCenter
        ).GetComponent<Button>();

        keybindsObject.sourceRectangle = JSON_Manager.GetUITile("SettingsButton");
        GameObject keybindsSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 36,
            height: 36,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("KeyBindsButtonIcon"),
            panelColor: Color.White
        );
        keybindsObject.gameObject.AddChild(keybindsSpriteObject, isOverlay: true);


        GameObject resolutionPanel = CreateResolutionPanelObject(SettingPanel, Vector2.Zero);
        resolutionPanel.SetActive(false);

        GameObject soundPanel = CreateSoundPanelObject(SettingPanel, Vector2.Zero, isMainMenuSettings);
        soundPanel.SetActive(false);

        GameObject keybindPanel = CreateKeybindPanelObject(SettingPanel, Vector2.Zero);
        keybindPanel.SetActive(false);

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: resolutionButtonObject,
            action: (parameters) =>
            {
                if (resolutionPanel.isActive) return;
                resolutionPanel.SetActive(true);

                soundPanel.SetActive(false);
                keybindPanel.SetActive(false);
            },
            parameters: []
        );

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: soundButtonObject,
            action: (parameters) =>
            {
                if (soundPanel.isActive) return;
                soundPanel.SetActive(true);

                resolutionPanel.SetActive(false);
                keybindPanel.SetActive(false);
            },
            parameters: []
        );

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: keybindsObject,
            action: (parameters) =>
            {
                if (keybindPanel.isActive) return;
                keybindPanel.SetActive(true);

                resolutionPanel.SetActive(false);
                soundPanel.SetActive(false);
            },
            parameters: []
        );


        SettingPanel.SetActive(false);
        return SettingPanel;
    }
}
