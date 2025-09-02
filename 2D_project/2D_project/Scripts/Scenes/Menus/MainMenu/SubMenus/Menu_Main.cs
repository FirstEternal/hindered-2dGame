using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class Menu_Main : Menu
{
    private static Button StoryButton;
    private static Button ChallengesButton;
    private static Button ReturnButton;

    private GameObject_TextField progressTextField1;

    GameObject elementPanel;
    GameObject[] elementVisualObjects;

    public Menu_Main(Panel parentPanel) : base()
    {
        parentPanel.gameObject.AddChild(this, isOverlay: true);
        CreateMenu();
    }
    protected override void CreateMenu()
    {
        base.CreateMenu();
        int fullWidth = (int)(GameWindow.Instance.windowWidth * 0.8);
        int fullHeight = (int)(GameWindow.Instance.windowHeight * 0.7);
        GameObject background_panelObject = CreateBackgroundPanel(fullWidth, fullHeight);

        GameObject elementProgressPanel = PrefabObjectSliderWithLabels.PanelObject(
            width: 440, //300,
            height: 360, //260, 
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Salamandra-MainMenuPose"),
            panelColor: Color.White
        );
        //elementProgressPanel.GetComponent<Panel>().resizeType = IResizableVisualComponent.ResizeType.None;
        background_panelObject.AddChild(elementProgressPanel, isOverlay: true);

        PivotCentering.UpdatePivot(
            background_panelObject.GetComponent<Panel>(),
            elementProgressPanel.GetComponent<Panel>(),
            elementProgressPanel.transform,
            pivotPosition: PivotCentering.Enum_Pivot.Center,
            offSet: new Vector2(0, -10)
        );
        // this one has element hover logic

        // BUTTONS
        int buttonCount = 1; //2;
        int buttonWidth = GameConstantsAndValues.MENU_BUTTON_WIDTH;
        int buttonHeight = GameConstantsAndValues.BUTTON_HEIGHT;

        GameObject buttonPanel = PrefabObjectSliderWithLabels.PanelObject(
            width: buttonWidth + 10,
            height: buttonCount * (buttonHeight + 10),
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            sliceBorderSize: 12
        );

        background_panelObject.AddChild(buttonPanel, isOverlay: true);
        PivotCentering.UpdatePivot(
            background_panelObject.GetComponent<Panel>(),
            buttonPanel.GetComponent<Panel>(),
            buttonPanel.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopLeft,
            offSet: new Vector2(10, 10)
        );

        Color buttonColor = GameConstantsAndValues.PanelColor_lightBlue;

        // story button
        int y_offset = 10;
        int curr_x_offset = 0;
        int curr_y_offset = 5;
        StoryButton = ButtonGameObject(buttonText: "STAGES", buttonWidth, buttonHeight, buttonColor: buttonColor, curr_x_offset, curr_y_offset,
            buttonPanel.GetComponent<Panel>(), buttonPivot: PivotCentering.Enum_Pivot.TopCenter,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: StoryButton,
            action: (parameters) =>
            {
                FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.LevelSelectionMenu);
            },
            parameters: null
        );
        /*
        // challenges button
        curr_x_offset = 0;
        curr_y_offset = curr_y_offset + buttonHeight + y_offset;
        ChallengesButton = ButtonGameObject(buttonText: "CHALLENGES", buttonWidth, buttonHeight, buttonColor: buttonColor, curr_x_offset, curr_y_offset,
            buttonPanel.GetComponent<Panel>(), buttonPivot: PivotCentering.Enum_Pivot.TopCenter,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        Button_HoverColorChange.AddSoundEffectAndOnClickAction(
            button: ChallengesButton,
            action: (parameters) =>
            {
                FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.LevelSelectionMenu);
            },
            parameters: null
        );*/

        // return button
        ReturnButton = ButtonGameObject(buttonText: "RETURN", buttonWidth,
            buttonHeight: GameConstantsAndValues.BUTTON_HEIGHT, buttonColor: GameConstantsAndValues.PanelColor_GrayFull,
            10, -10, background_panelObject.GetComponent<Panel>(),
            PivotCentering.Enum_Pivot.BottomLeft, textColor: Color.Red, centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle
        ).GetComponent<Button>();

        ReturnButton.AssignOnClickAction((parameters) => { FullMenu.Instance.OpenMenu(currMenu: this, newMenu: FullMenu.Instance.sessionMenu); }, null);


        // Progress Panel
        int titleWidth = GameConstantsAndValues.MENU_BUTTON_WIDTH;
        int titleHeight = GameConstantsAndValues.BUTTON_HEIGHT;

        int progressWidth = titleWidth + 20;
        GameObject progressPanel = PrefabObjectSliderWithLabels.PanelObject(
            width: progressWidth,
            height: fullHeight - 20,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("backgroundPanel"),
            panelColor: Color.White,
            sliceBorderSize: 12
        );

        PivotCentering.UpdatePivot(
            background_panelObject.GetComponent<Panel>(),
            progressPanel.GetComponent<Panel>(),
            progressPanel.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopRight,
            offSet: new Vector2(-10, 10)
        );

        background_panelObject.AddChild(progressPanel, isOverlay: true);

        GameObject titleBackgroundObject = PrefabObjectSliderWithLabels.PanelObject(
            width: titleWidth,
            height: titleHeight,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_GrayFull
        );

        progressPanel.AddChild(titleBackgroundObject, isOverlay: true);
        PivotCentering.UpdatePivot(
            progressPanel.GetComponent<Panel>(),
            titleBackgroundObject.GetComponent<Panel>(),
            titleBackgroundObject.transform,
            pivotPosition: PivotCentering.Enum_Pivot.TopLeft,
            offSet: new Vector2(10, 10)
        );

        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(width: titleWidth, height: titleHeight, JSON_Manager.customBitmapFont,
            "PROGRESS", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_H, spacingX: 5, color: Color.Silver,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );

        GameObject_TextField titleTextField = new GameObject_TextField(spriteTextComponent);
        titleBackgroundObject.AddChild(titleTextField, isOverlay: true);

        Color progColor = Color.White;
        SpriteTextComponent progressText1 = new SpriteTextComponent(width: progressWidth,
            height: titleHeight, JSON_Manager.customBitmapFont,
           "", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
           textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
           textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
           fontSize: GameConstantsAndValues.FONT_SIZE_HM, spacingX: 5, color: progColor,
           graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
       );

        progressTextField1 = new GameObject_TextField(progressText1);
        progressPanel.AddChild(progressTextField1, isOverlay: true);

        elementPanel = new GameObject();
        elementPanel.CreateTransform();
        elementPanel.transform.localScale = new Vector2(0.7f, 0.7f);
        progressPanel.AddChild(elementPanel, isOverlay: true);


        string[] elementNames = { "Burner", "Drowner", "Boulderer", "Froster", "Grasser", "Shader", "Thunderer", "Empty" };
        elementVisualObjects = new GameObject[elementNames.Length];

        for (int i = elementNames.Length - 1; i >= 0; i--)
        {
            string name = elementNames[i];

            // Create GameObject
            GameObject element = new GameObject();
            element.CreateTransform();

            // Create Sprite
            Sprite elementSprite = new Sprite(texture2D: JSON_Manager.uiSpriteSheet, colorTint: Color.White);

            // Add sprite to element
            element.AddComponent(elementSprite);
            if (name != "Empty") element.SetActiveWithParentEnabled = false;

            // Add element to panel
            elementPanel.AddChild(element, isOverlay: true);

            elementSprite.sourceRectangle = JSON_Manager.GetUITile($"Base_{name}_0");
            elementSprite.origin = JSON_Manager.GetUIOrigins($"Base_{name}", 1, element.transform.globalScale)[0];

            // Store reference
            elementVisualObjects[i] = element;
        }
    }

    protected override void UpdateMenu()
    {
        PlayerSession playerSession = ExisitingPlayerSessions.GetActiveSession();
        if (playerSession is null) return;

        int elementsAcquiered = 0;
        foreach (bool acquieredElement in playerSession.AcquiredElements)
        {
            if (acquieredElement) elementsAcquiered++;
        }

        int challengesStarsAcquiered = 0;
        foreach (ChallengeData challengeData in playerSession.Challenges)
        {
            foreach (bool starAchieved in challengeData.starAchievedArray)
            {
                if (starAchieved) challengesStarsAcquiered++;
            }
        }

        SpriteTextComponent spriteTextComponent1 = progressTextField1.spriteTextComponent;
        spriteTextComponent1.text = $"""
            STORY; </yellow>{elementsAcquiered}∕8
            </white>   - TIME; </yellow>{"12:25 s"}
            </white>CHALLENGES; </yellow>{challengesStarsAcquiered}∕8
            </white>   - STARS; </yellow>{challengesStarsAcquiered}∕24
            </white>ELEMENTS; </yellow>{elementsAcquiered}∕7
            """;

        var (width, height) = spriteTextComponent1.MeasureText();
        progressTextField1.transform.localPosition = new Vector2(20, -170 + height / 2);

        elementPanel.transform.localPosition = new Vector2(0, height / 2 + 10);

        for (int i = 0; i < elementVisualObjects.Length - 1; i++)
        {
            elementVisualObjects[i].SetActive(playerSession.AcquiredElements[i]);
        }
    }
}
