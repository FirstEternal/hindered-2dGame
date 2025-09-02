using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

internal class HUD_PauseMenu
{
    public Keys openMenuKey;
    public Button CloseMenuButton;
    public GameObject PauseMenu;

    public HUD_PauseMenu(Scene_Stage scene)
    {
        // settings button
        //int buttonSize = 50;
        PauseMenu = new GameObject();
        PauseMenu.CreateTransform();
        PauseMenu.transform.localPosition = new Vector2(GameWindow.Instance.windowWidth / 2, GameWindow.Instance.windowHeight / 2);
        scene.AddGameObjectToScene(PauseMenu, isOverlay: true);

        int buttonWidth = GameConstantsAndValues.MENU_BUTTON_WIDTH;
        int buttonHeight = GameConstantsAndValues.BUTTON_HEIGHT;

        GameObject settingsBar = SettingsToolBar.Instance.CreateGameSettingsPanelObject(
            parent: PauseMenu,
            position: Vector2.Zero,
            menuName: "PAUSE MENU"
        );

        GameObject bottomToolbar = PrefabObjectSliderWithLabels.PanelObject(
            width: (int)(GameWindow.Instance.windowWidth * 0.55f) - 20,
            height: buttonHeight * 2 + 20,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            layerDepth: 0,
            sliceBorderSize: 12
        );
        settingsBar.AddChild(bottomToolbar, isOverlay: true);

        PivotCentering.UpdatePivot(
            parentSprite: settingsBar.GetComponent<Panel>(),
            bottomToolbar.GetComponent<Panel>(),
            bottomToolbar.transform,
            pivotPosition: PivotCentering.Enum_Pivot.BottomCenter,
            offSet: new Vector2(0, -5)
        );

        // return button
        Button ReturnToMenuButton = Menu.ButtonGameObject(buttonText: "MAIN MENU", buttonWidth,
            buttonHeight: buttonHeight, buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
            0, -10, parentPanel: bottomToolbar.GetComponent<Panel>(),
            PivotCentering.Enum_Pivot.BottomCenter, textColor: Color.Red, centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: ReturnToMenuButton,
            action: (parameters) =>
            {
                // close pause menu
                scene.isPaused = false;
                PauseMenu.SetActive(false);

                // load menu scene
                FullMenu.Instance.BackToMenu();
                SceneManager.Instance.LoadScene("Scene_MenuScene");
            },
            parameters: null
        );

        Button restartLevelButton = Menu.ButtonGameObject(
            buttonText: "RESTART",
            buttonWidth: buttonWidth,
            buttonHeight: buttonHeight,
            buttonColor: GameConstantsAndValues.PanelColor_lightBlue,
            curr_x_offset: 0,
            curr_y_offset: 5,
            parentPanel: bottomToolbar.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.TopCenter,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: restartLevelButton,
            action: (parameters) =>
            {
                // close pause menu
                scene.isPaused = false;
                PauseMenu.SetActive(false);

                // restart stage
                scene.RestartStage(resetRespawn: true);
            },
            parameters: null
        );

        // exit button
        Button exitPauseMenuButton = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: 50,
            buttonHeight: 50,
            buttonColor: Color.White,
            curr_x_offset: -10,
            curr_y_offset: 10,
            parentPanel: settingsBar.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.TopRight
        ).GetComponent<Button>();

        exitPauseMenuButton.sourceRectangle = JSON_Manager.GetUITile("SettingsButton");
        GameObject exitSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 36,
            height: 36,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("XButtonIcon"),
            panelColor: Color.White
        );
        exitPauseMenuButton.gameObject.AddChild(exitSpriteObject, isOverlay: true);

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: exitPauseMenuButton,
            action: (parameters) =>
            {
                // close pause menu
                scene.isPaused = false;
                PauseMenu.SetActive(false);
            },
            parameters: null
        );
    }
}
