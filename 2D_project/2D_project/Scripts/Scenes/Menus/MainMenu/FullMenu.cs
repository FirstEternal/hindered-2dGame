using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class FullMenu : Menu
{
    public static FullMenu Instance { get; private set; }

    public Menu_Session sessionMenu;
    public GameObject mainMenu;
    public GameObject LevelSelectionMenu;

    public GameObject menu_level1;
    public GameObject menu_level2;
    public GameObject menu_level3;

    public GameObject menu_challenge;

    public GameObject activeMenu;
    public FullMenu(Scene scene) : base()
    {
        if (Instance is null) Instance = this;
        scene.AddGameObjectToScene(this, isOverlay: true);
        CreateMenu();
    }

    public static IOnClick.OnClickAction ExitGame()
    {
        return (parameters) =>
        {
            // save sessions
            ExisitingPlayerSessions.SavePlayerSessions();
            // exit the game
            Game2DPlatformer.Instance.Exit();
        };
    }

    protected override void CreateMenu()
    {
        CreateTransform();
        transform.globalPosition = new Vector2(GameWindow.Instance.windowWidth / 2, GameWindow.Instance.windowHeight / 2);
        // 1.) BACKGROUND
        // background -> black border
        int background_panelWidth = (int)(GameWindow.Instance.windowWidth * 1f);
        int background_panelHeight = (int)(GameWindow.Instance.windowHeight * 1f);

        GameObject background_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: background_panelWidth,
            height: background_panelHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("background1"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );


        AddChild(background_panelObject, isOverlay: true);

        // background -> blue inside border
        int background_inner_panelWidth = background_panelWidth - 10;
        int background_inner_panelHeight = background_panelHeight - 10;
        GameObject background_inner_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: background_inner_panelWidth,
            height: background_inner_panelHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("MainMenu_background_with_art"),
            panelColor: Color.White
        );
        background_panelObject.AddChild(background_inner_panelObject, isOverlay: true);

        int titleWidth = 470; // 517;
        int titleHeight = 60;//105;
        GameObject titleBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: titleWidth,
            height: titleHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull
        );

        background_inner_panelObject.AddChild(titleBackgroundObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            background_inner_panelObject.parent.GetComponent<Panel>(),
            titleBackgroundObject.GetComponent<Panel>(),
            titleBackgroundObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopCenter,
            offSet: new Vector2(0, 20)
        );

        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(width: titleWidth, height: titleHeight, JSON_Manager.customBitmapFont,
            "TRIAL OF 𝔰ALAMANADRA", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: 40, spacingX: 5, color: Color.OrangeRed,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );

        GameObject_TextField titleTextField = new GameObject_TextField(spriteTextComponent);
        titleBackgroundObject.AddChild(titleTextField, isOverlay: true);

        CreateSubMenus(background_inner_panelObject.GetComponent<Panel>());

        // create exit tool bar
        GameObject exitToolbar = SettingsToolBar.Instance.CreateMenuSettingsPanelObject(
            parent: this,
            position: Vector2.Zero
        );
    }

    private void CreateSubMenus(Panel parentPanel)
    {
        // 1.) login menu
        sessionMenu = new Menu_Session(parentPanel);
        mainMenu = new Menu_Main(parentPanel);
        LevelSelectionMenu = new Menu_LevelSelection(parentPanel);

        menu_level1 = new Menu_Level1(parentPanel, startingChallengeIndex: 0,bossButtonCount: 2, availablePoints: 5, hasStoryStages: true);
        menu_level2 = new Menu_Level2(parentPanel, startingChallengeIndex: 2, bossButtonCount: 1, availablePoints: 10, hasStoryStages: true);
        menu_level3 = new Menu_Level3(parentPanel, startingChallengeIndex: 4, bossButtonCount: 1, availablePoints: 15, hasStoryStages: false);

        menu_challenge = new Menu_Challenge(parentPanel);

        // start with main menu
        mainMenu.SetActive(false);
        LevelSelectionMenu.SetActive(false);
        menu_level1.SetActive(false);
        menu_level2.SetActive(false);
        menu_level3.SetActive(false);
        menu_challenge.SetActive(false);

        activeMenu = sessionMenu;
        activeMenu.SetActive(true);
    }

    public void OpenMenu(GameObject currMenu, GameObject newMenu)
    {
        currMenu?.SetActive(false);
        activeMenu = newMenu;
        activeMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        LevelSelectionMenu.SetActive(false);
        menu_level1.SetActive(false);
        menu_level2.SetActive(false);
        menu_level3.SetActive(false);
        menu_challenge.SetActive(false);

        activeMenu = mainMenu;
        mainMenu.SetActive(true);

        // TODO
        // show progress
    }
}