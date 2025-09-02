using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class Menu_Session : Menu
{
    private static Button CreateSessionButton0, CreateSessionButton1, CreateSessionButton2;
    private static Button LoadSessionButton0, LoadSessionButton1, LoadSessionButton2;

    private static Button ReturnToMainMenu;

    public static event EventHandler OnSessionLoad;
    public bool hasLoaded { get; private set; } = false;

    public Menu_Session(Panel parentPanel) : base()
    {
        parentPanel.gameObject.AddChild(this, isOverlay: true);
        CreateMenu();
    }

    public override void SetActive(bool value)
    {
        base.SetActive(value);
        ShowLoadSessionButtons();
    }

    private void ShowLoadSessionButtons()
    {
        if (!isActive) return;
        // LOAD SESSIONS
        ExisitingPlayerSessions.LoadPlayerSessions();

        // SESSION BUTTONS
        bool session0Exists = ExisitingPlayerSessions.GetPlayerSession(ExisitingPlayerSessions.SessionIndex.SESSION_0) is not null;
        bool session1Exists = ExisitingPlayerSessions.GetPlayerSession(ExisitingPlayerSessions.SessionIndex.SESSION_1) is not null;
        bool session2Exists = ExisitingPlayerSessions.GetPlayerSession(ExisitingPlayerSessions.SessionIndex.SESSION_2) is not null;

        CreateSessionButton0.gameObject.SetActive(!session0Exists);
        LoadSessionButton0.gameObject.SetActive(session0Exists);

        CreateSessionButton1.gameObject.SetActive(!session1Exists);
        LoadSessionButton1.gameObject.SetActive(session1Exists);

        CreateSessionButton2.gameObject.SetActive(!session2Exists);
        LoadSessionButton2.gameObject.SetActive(session2Exists);

        hasLoaded = true;
    }
    protected override void CreateMenu()
    {
        base.CreateMenu();
        // 1.) BACKGROUND
        // background -> black border
        GameObject background_inner_panelObject = CreateBackgroundPanel(
            320,
            (int)(GameWindow.Instance.windowHeight * 0.7));

        CreateSessionButtons(background_inner_panelObject.GetComponent<Panel>(), buttonWidth: 300);

        // return button
        ReturnToMainMenu = ButtonGameObject(buttonText: "EXIT GAME", 300,
            buttonHeight: GameConstantsAndValues.BUTTON_HEIGHT, buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
            0, -10, background_inner_panelObject.GetComponent<Panel>(),
            PivotCentering.Enum_Pivot.BottomCenter, textColor: Color.Red, centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: ReturnToMainMenu,
            action: FullMenu.ExitGame(),
            parameters: []
        );

        ShowLoadSessionButtons();
    }

    private void CreateSessionButtons(Panel backgroundpanel, int buttonWidth)
    {
        int y_offset = 10;
        int buttonHeight = GameConstantsAndValues.BUTTON_HEIGHT;
        Color buttonColor = GameConstantsAndValues.PanelColor_lightBlue;

        Button[] createButtons = new Button[3];
        Button[] loadButtons = new Button[3];

        for (int i = 0; i < 3; i++)
        {
            int index = i; // Closure-safe capture
            int curr_y_offset = (i + 1) * y_offset + i * buttonHeight;

            createButtons[index] = ButtonGameObject(
                buttonText: "NEW SESSION",
                buttonWidth: buttonWidth,
                buttonHeight: buttonHeight,
                buttonColor: buttonColor,
                curr_x_offset: 0,
                curr_y_offset: curr_y_offset,
                parentPanel: backgroundpanel,
                buttonPivot: PivotCentering.Enum_Pivot.TopCenter,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
            ).GetComponent<Button>();

            loadButtons[index] = ButtonGameObject(
                buttonText: $"LOAD SESSION-{index + 1}",
                buttonWidth: buttonWidth,
                buttonHeight: buttonHeight,
                buttonColor: buttonColor,
                curr_x_offset: 0,
                curr_y_offset: curr_y_offset,
                parentPanel: backgroundpanel,
                buttonPivot: PivotCentering.Enum_Pivot.TopCenter,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
            ).GetComponent<Button>();

            Button_HoverColorChange.AddSoundEffectAndOnClickAction(
                createButtons[index],
                CreateSession,
                parameters: [this, (ExisitingPlayerSessions.SessionIndex)index, createButtons[index], loadButtons[index]]
            );

            Button_HoverColorChange.AddSoundEffectAndOnClickAction(
                loadButtons[index],
                LoadSession,
                parameters: [this, (ExisitingPlayerSessions.SessionIndex)index]
            );
        }

        // Store in static fields
        CreateSessionButton0 = createButtons[0];
        CreateSessionButton1 = createButtons[1];
        CreateSessionButton2 = createButtons[2];
        LoadSessionButton0 = loadButtons[0];
        LoadSessionButton1 = loadButtons[1];
        LoadSessionButton2 = loadButtons[2];
    }

    private void CreateSession(object[] parameters)
    {
        // parameters
        GameObject loginMenu = (GameObject)parameters[0];
        ExisitingPlayerSessions.SessionIndex sessionIndex = (ExisitingPlayerSessions.SessionIndex)parameters[1];
        Button createSessionButton = (Button)parameters[2];
        Button loadSessionButton = (Button)parameters[3];

        // function
        ExisitingPlayerSessions.CreateSession(sessionIndex);
        createSessionButton.gameObject.SetActive(false);
        loadSessionButton.gameObject.SetActive(true);
    }

    private void LoadSession(object[] parameters)
    {
        // parameters
        GameObject loginMenu = (GameObject)parameters[0];
        ExisitingPlayerSessions.SessionIndex sessionIndex = (ExisitingPlayerSessions.SessionIndex)parameters[1];

        // function
        ExisitingPlayerSessions.MakeSessionActive(sessionIndex);
        OnSessionLoad?.Invoke(this, EventArgs.Empty);

        // disable login menu
        SetActive(false);

        // enable main menu
        FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.mainMenu);
    }
}
