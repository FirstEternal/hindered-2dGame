using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

internal class Menu_Level : Menu
{
    protected List<Button> buttons = new List<Button>();
    private int bossChallangeCount;
    private int startingChallengeIndex;

    private static Button ReturnToLevelSelection;
    protected int[] challengeIndices;

    protected int availablePoints;
    protected UpgradeOptions upgradeOptions;

    private bool hasStoryStage;
    public ElementDescriptionGameObject elementDescriptionGameObject;

    public Menu_Level(Panel parentPanel, int startingChallengeIndex, int bossChallangeCount, int availablePoints, bool hasStoryStage = true) : base()
    {
        this.startingChallengeIndex = startingChallengeIndex;
        this.hasStoryStage = hasStoryStage;
        this.availablePoints = availablePoints;
        parentPanel.gameObject.AddChild(this, isOverlay: true);
        this.bossChallangeCount = bossChallangeCount;
        CreateMenu();

        elementDescriptionGameObject = new ElementDescriptionGameObject(parentPanel);

        AddChild(elementDescriptionGameObject, isOverlay: true);
    }

    protected override void CreateMenu()
    {
        base.CreateMenu();
        // 1.) BACKGROUND
        // background -> black border
        int panelWidth = (int)(GameWindow.Instance.windowWidth * 0.8);
        int panelHeight = (int)(GameWindow.Instance.windowHeight * 0.7);
        GameObject background_inner_panelObject = CreateBackgroundPanel(panelWidth, panelHeight);


        // STAGE BUTTONS       
        int buttonWidth = GameConstantsAndValues.MENU_BUTTON_WIDTH;
        int buttonHeight = GameConstantsAndValues.BUTTON_HEIGHT;
        int curr_x_offset = 10;
        int curr_y_offset = 10;
        Color unlockedButtonColor = GameConstantsAndValues.PanelColor_lightBlue;

        // add story button
        int y_offset = 10;
        int startIndexOffset = hasStoryStage ? 1 : 0;

        if (hasStoryStage)
        {
            buttons.Add(ButtonGameObject(
                buttonText: "STORY",
                buttonWidth, buttonHeight,
                buttonColor: unlockedButtonColor,
                curr_x_offset, curr_y_offset,
                background_inner_panelObject.GetComponent<Panel>(),
                PivotCentering.Enum_Pivot.TopLeft,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
            ).GetComponent<Button>());
        }

        // add challenge buttons
        for (int i = 0; i < bossChallangeCount; i++)
        {
            curr_y_offset = (i + 1 + startIndexOffset) * y_offset + (i + startIndexOffset) * buttonHeight;
            buttons.Add(ButtonGameObject(
                buttonText: $"CHALLENGE-{startingChallengeIndex + i + 1}",
                buttonWidth, buttonHeight,
                buttonColor: unlockedButtonColor,
                curr_x_offset, curr_y_offset,
                background_inner_panelObject.GetComponent<Panel>(),
                PivotCentering.Enum_Pivot.TopLeft,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
            ).GetComponent<Button>());
        }

        upgradeOptions = new UpgradeOptions(
           parentPanel: background_inner_panelObject.GetComponent<Panel>(),
           width: 352,
           height: 440,
           availabePoints: 50
        );

        new ElementLoadoutOption(parentPanel: background_inner_panelObject.GetComponent<Panel>(), menu_Level: this);

        // return button
        ReturnToLevelSelection = ButtonGameObject(buttonText: "RETURN", buttonWidth,
            buttonHeight: GameConstantsAndValues.BUTTON_HEIGHT, buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
            10, -10, background_inner_panelObject.GetComponent<Panel>(),
            PivotCentering.Enum_Pivot.BottomLeft, textColor: Color.Red, centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(ReturnToLevelSelection, (parameters) => { FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.mainMenu); }, null);
    }

    protected void LoadStageViaString(object[] parameters)
    {
        upgradeOptions.SetPlayerStats(); // set upgrade options under player
        string sceneName = (string)parameters[0];
        SceneManager.Instance.LoadScene(sceneName);

    }

    protected void AssignButtonFunctions(List<IOnClick.OnClickAction> buttonActions, List<object[]> actionParameters)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            //buttons[i].AssignOnClickAction(buttonActions[i], actionParameters[i]);
            Button_HoverColorChange.AddSoundEffectAndOnClickAction(buttons[i], buttonActions[i], actionParameters[i]);

        }
    }

    protected void AssignLevel(string bossName, int bossIndex)
    {
        Menu_Challenge menuChallenge = (Menu_Challenge)FullMenu.Instance.menu_challenge;
        Rectangle sourceRect = JSON_Manager.GetEnemiesSourceRectangles($"Dragonic {bossName}_P1", 1)[0];
        menuChallenge.UpdateValues(parentMenu: this, challengeDataIndex: bossIndex, challengeSceneName: $"Boss_Fight_{bossName}", $"{bossName.ToUpper()}", sourceRect);
        FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.menu_challenge);
    }
}