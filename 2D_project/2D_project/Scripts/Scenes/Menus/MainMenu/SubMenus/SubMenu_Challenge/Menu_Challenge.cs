using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
internal class Menu_Challenge : Menu
{
    private Menu_Level parentMenu;
    private int challengeDataIndex;

    private GameObject_TextField challangeTimeScore_gameObject_TextField;

    private Button PlayChallangeButton;
    private Button ReturnButton;


    private GameObject_ChallangeScoreUI challangeScoreUI;
    GameObject_ChallangeBossInformation bossInfoUI;
    /*
    protected override void ShowMenu()
    {
        base.ShowMenu();

        PlayerSession session = ExisitingPlayerSessions.GetActiveSession();
        if (session is null) return;
        challangeScoreUI.UpdateValues(ExisitingPlayerSessions.GetActiveSession().Challenges[challengeDataIndex]);
    }
    */
    public Menu_Challenge(Panel parentPanel) : base()
    {
        parentPanel.gameObject.AddChild(this, isOverlay: true);
        CreateMenu();
    }
    protected override void CreateMenu()
    {
        base.CreateMenu();
        GameObject background_inner_panelObject = CreateBackgroundPanel(
            (int)(GameWindow.Instance.windowWidth * 0.8),
            (int)(GameWindow.Instance.windowHeight * 0.6));

        // BUTTONS
        int y_offset = 20;
        int buttonWidth = 420;
        int buttonHeight = GameConstantsAndValues.BUTTON_HEIGHT;
        int curr_x_offset = 0;
        int curr_y_offset = y_offset;
        Color buttonColor = GameConstantsAndValues.PanelColor_lightBlue;

        // Challange text
        challangeScoreUI = new GameObject_ChallangeScoreUI();
        background_inner_panelObject.AddChild(challangeScoreUI, isOverlay: true);
        challangeScoreUI.CreateUI();
        PivotCentering.UpdatePivot(background_inner_panelObject.GetComponent<Panel>(), challangeScoreUI.GetChild(0).GetComponent<Panel>(),
            challangeScoreUI.transform, PivotCentering.Enum_Pivot.TopLeft, new Vector2(20, 40));
        challangeScoreUI.transform.globalPosition = challangeScoreUI.GetChild(0).transform.globalPosition;

        // img panel
        bossInfoUI = new GameObject_ChallangeBossInformation(
            panelWidth: (int)(background_inner_panelObject.GetComponent<Panel>().width / 2 - 30),
            panelHeight: (int)(background_inner_panelObject.GetComponent<Panel>().height - 30 - buttonHeight)
        );
        AddChild(bossInfoUI, isOverlay: true);
        bossInfoUI.CreateUI();
        PivotCentering.UpdatePivot(background_inner_panelObject.GetComponent<Panel>(), bossInfoUI.GetComponent<Panel>(), bossInfoUI.transform,
            PivotCentering.Enum_Pivot.TopRight, new Vector2(-10, 10));

        curr_x_offset = 20;
        curr_y_offset = -10;
        // buttons
        PlayChallangeButton = ButtonGameObject(buttonText: "PLAY", buttonWidth, buttonHeight, buttonColor: buttonColor, curr_x_offset, curr_y_offset,
            background_inner_panelObject.GetComponent<Panel>(), PivotCentering.Enum_Pivot.BottomLeft).GetComponent<Button>();

        curr_x_offset = -20;
        curr_y_offset = -10;
        ReturnButton = ButtonGameObject(buttonText: "RETURN", buttonWidth, buttonHeight, buttonColor: buttonColor, curr_x_offset, curr_y_offset,
            background_inner_panelObject.GetComponent<Panel>(), PivotCentering.Enum_Pivot.BottomRight).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(ReturnButton, (parameters) =>
        {
            // return to parent menu
            parentMenu.SetActive(true);
            this.SetActive(false);
        }, []);

    }

    public void UpdateValues(Menu_Level parentMenu, int challengeDataIndex, string challengeSceneName, string bossName, Rectangle bossArtSourceRectangle)
    {
        // round time to 2 decimals
        //challangeTimeScore_gameObject_TextField.spriteTextComponent.text = challangeData.challangeTimeScore.ToString();

        this.parentMenu = parentMenu;
        //PlayChallangeButton.AssignOnClickAction((parameters) => { SceneManager.Instance.LoadScene(challengeSceneName); }, null);

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(PlayChallangeButton, (parameters) => { SceneManager.Instance.LoadScene(challengeSceneName); }, null);

        bossInfoUI.UpdateValues(bossName, bossArtSourceRectangle);
        challangeScoreUI.UpdateValues(ExisitingPlayerSessions.GetActiveSession().Challenges[challengeDataIndex]);
    }

    public override void SetActive(bool value)
    {
        base.SetActive(value);
    }
}
