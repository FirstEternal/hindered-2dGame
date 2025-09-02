using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class Menu_LevelSelection : Menu
{
    /*
    private static Button LoadStage1;
    private static Button LoadStage2;
    private static Button LoadStage3;
    private static Button LoadStage4;
    */
    private static Button ReturnToSessionMenu;

    public Menu_LevelSelection(Panel parentPanel) : base()
    {
        parentPanel.gameObject.AddChild(this, isOverlay: true);
        CreateMenu();
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
        bool session0Exists = ExisitingPlayerSessions.GetPlayerSession(ExisitingPlayerSessions.SessionIndex.SESSION_0) is not null;
        bool session1Exists = ExisitingPlayerSessions.GetPlayerSession(ExisitingPlayerSessions.SessionIndex.SESSION_1) is not null;
        bool session2Exists = ExisitingPlayerSessions.GetPlayerSession(ExisitingPlayerSessions.SessionIndex.SESSION_2) is not null;

        int buttonWidth = GameConstantsAndValues.MENU_BUTTON_WIDTH;
        CreateStageButtons(background_inner_panelObject.GetComponent<Panel>(), buttonWidth: buttonWidth);

        // Upgrade Options
        new UpgradeOptions(
            parentPanel: background_inner_panelObject.GetComponent<Panel>(),
            width: 352,
            height: 440,
            availabePoints: 50
        );

        //new ElementLoadoutOption(parentPanel: background_inner_panelObject.GetComponent<Panel>(), menu_Level: this);

        // return button
        ReturnToSessionMenu = ButtonGameObject(buttonText: "RETURN", buttonWidth,
            buttonHeight: GameConstantsAndValues.BUTTON_HEIGHT, buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
            10, -10, background_inner_panelObject.GetComponent<Panel>(),
            PivotCentering.Enum_Pivot.BottomLeft, textColor: Color.Red, centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        ReturnToSessionMenu.AssignOnClickAction((parameters) => { FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.mainMenu); }, null);
        //ReturnToSessionMenu.AssignOnClickAction((parameters) => { SetActive(false); FullMenu.Instance.mainMenu.SetActive(true); }, null);
    }

    void CreateStageButtons(Panel backgroundPanel, int buttonWidth)
    {
        int buttonHeight = GameConstantsAndValues.BUTTON_HEIGHT;
        Color buttonColor = GameConstantsAndValues.PanelColor_lightBlue;

        string[] stageNames = { "STAGE-1", "STAGE-2", "STAGE-3"};
        Action[] onClickActions = {
            () => { FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.menu_level1); },
            () => { FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.menu_level2); },
            () => { FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.menu_level3); }
        };

        int y = 0;
        int index = 0;
        for (int i = 0; i < stageNames.Length; i++)
        {
            int stageIndex = i;
            if (i % 7 == 0 && i != 0)
            { 
                y++;
            }

            Button button = ButtonGameObject(
                buttonText: stageNames[i],
                buttonWidth: buttonWidth,
                buttonHeight: buttonHeight,
                buttonColor: buttonColor,
                curr_x_offset: 10 + y * (buttonWidth + 10),
                curr_y_offset: 10 + index * (buttonHeight + 10),
                backgroundPanel,
                buttonPivot: PivotCentering.Enum_Pivot.TopLeft,
                centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
            ).GetComponent<Button>();

            Button_HoverColorChange.AddSoundEffectAndOnClickAction(button, (parameters) => onClickActions[stageIndex](), null);
            /*
            // Store in static field
            switch (i)
            {
                case 0: LoadStage1 = button; break;
                case 1: LoadStage2 = button; break;
                case 2: LoadStage3 = button; break;
                case 3: LoadStage4 = button; break;
            }*/

            index++;
            if (index == 7) index = 0; 
        }
    }
    /*
    protected override void ShowMenu()
    {
        if (!isActive) return;

        PlayerSession activeSession = ExisitingPlayerSessions.GetActiveSession();

        bool[] stageUnlocked = [
            activeSession.Stages[0] == true,
            activeSession.Stages[0] == true,
            activeSession.Stages[0] == true,
            activeSession.Stages[0] == true];

        LoadStage1.isDisabled = stageUnlocked[0];
        LoadStage1.isHoverEnable = LoadStage1.canPress;
        LoadStage1.colorTint = LoadStage1.canPress ? GameConstantsAndValues.PanelColor_lightBlue : Color.Gray;

        LoadStage2.isDisabled = stageUnlocked[0];
        LoadStage2.isHoverEnable = LoadStage2.canPress;
        LoadStage2.colorTint = LoadStage2.canPress ? GameConstantsAndValues.PanelColor_lightBlue : Color.Gray;

        LoadStage3.isDisabled = stageUnlocked[0];
        LoadStage3.isHoverEnable = LoadStage3.canPress;
        LoadStage3.colorTint = LoadStage3.canPress ? GameConstantsAndValues.PanelColor_lightBlue : Color.Gray;

        LoadStage4.isDisabled = stageUnlocked[0];
        LoadStage4.isHoverEnable = LoadStage4.canPress;
        LoadStage4.colorTint = LoadStage4.canPress ? GameConstantsAndValues.PanelColor_lightBlue : Color.Gray;
    } */
}

