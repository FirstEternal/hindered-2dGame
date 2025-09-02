using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
internal class UpgradeOptions
{
    float[] playerStats; // TODO: only needed for initial load
    int pointsAvailable = 300; //10; // TODO: calculate based on logic
    int pointsUsed = 0; // TODO: calculate based on logic
    private Dictionary<int, UpgradeOption> playerStatValues = new Dictionary<int, UpgradeOption>();

    static SpriteTextComponent availablePointsSpriteText;

    public void SetPlayerStats()
    {
        Player.Instance.loadout.playerStats = playerStats;
    }

    public UpgradeOptions(Panel parentPanel, int width, int height, int availabePoints)
    {
        pointsAvailable = availabePoints;
        /*
        Menu_Session.OnSessionLoad -= UpdateData;
        Menu_Session.OnSessionLoad += UpdateData;
        */
        //ExisitingPlayerSessions.OnSaveLoadDelete += ipda

        CreateUpgradeOptionObjects(parentPanel, width, height);
    }

    /*
       private void UpdateData(object sender, EventArgs e)
       {
           PlayerSession activeSession = ExisitingPlayerSessions.GetActiveSession();
           playerStats = activeSession.PlayerStats;
           //Menu_Session.OnSessionLoad -= UpdateData; 
           // not ideal
           ExisitingPlayerSessions.OnSaveLoadDelete -= UpdateData;
           ExisitingPlayerSessions.OnSaveLoadDelete += UpdateData;

           //PlayerSession activeSession = ExisitingPlayerSessions.GetActiveSession();
           playerStats = activeSession.PlayerStats;

           for (int i = 0; i < playerStats.Length; i++)
           {
               playerStatValues[i].UpdateValue(playerStats[i]);
           }
       }
   */

    /// <summary>
    /// 0 -> health
    /// 1 -> shield
    /// 2 -> atk dmg
    /// 3 -> crit rate
    /// 4 -> crit damage
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    public void UpdatePlayerStat(float value, int index)
    {
        playerStats[index] = value;

        availablePointsSpriteText.text = $"AVAILABLE POINTS; </yellow>{pointsAvailable - pointsUsed}";
    }

    public void UpdatePlayerStats(float[] newPlayerStats)
    {
        /*
        PlayerSession activeSession = ExisitingPlayerSessions.GetActiveSession();
        activeSession.PlayerStats = newPlayerStats;
        ExisitingPlayerSessions.SaveActiveSession();
        */
    }

    private class UpgradeOption
    {
        readonly SpriteTextComponent spriteTextComponent;
        readonly SpriteTextComponent upgradeSpriteTextComponent;
        readonly int index;
        readonly float minValue;
        readonly float maxValue;
        readonly float valueIncrease;
        readonly Button plusButton;
        readonly Button minusButton;
        readonly string name;

        public UpgradeOption(UpgradeOptions upgradeOptions, GameObject fullGameObject, Button plusButton, Button minusButton, string name, SpriteTextComponent spriteTextComponent, SpriteTextComponent upgradeSpriteTextComponent, int index, float value, float valueIncrease, float minValue, float maxValue)
        {
            this.spriteTextComponent = spriteTextComponent;
            this.upgradeSpriteTextComponent = upgradeSpriteTextComponent;
            this.index = index;
            this.valueIncrease = valueIncrease;
            this.minValue = minValue;
            this.maxValue = maxValue;

            this.name = name;
            this.plusButton = plusButton;
            this.minusButton = minusButton;

            plusButton.isDisabled = value >= maxValue;
            minusButton.isDisabled = value <= minValue;
            plusButton.colorTint = (value >= maxValue) ? Color.Gray : Color.White;
            minusButton.colorTint = (value <= minValue) ? Color.Gray : Color.White;

            // PLUS BUTTON
            Button_HoverColorChange.AddSoundEffectAndOnClickAction(
                button: this.plusButton,
                action: (parameters) =>
                {
                    if (upgradeOptions.pointsAvailable - upgradeOptions.pointsUsed == 0)
                        return;

                    float raw = upgradeOptions.playerStats[index] + valueIncrease;
                    float value = MathF.Round(raw, 4); // Round to 4 decimal places (enough for 0.1/0.2 steps)

                    if (value > maxValue)
                        return;

                    upgradeOptions.pointsUsed++;
                    upgradeOptions.UpdatePlayerStat(value, index);
                    UpdateValue(value);

                    plusButton.colorTint = (value >= maxValue) ? Color.Gray : Color.White;
                },
                parameters: [plusButton]
            );

            // MINUS BUTTON
            Button_HoverColorChange.AddSoundEffectAndOnClickAction(
                button: this.minusButton,
                action: (parameters) =>
                {
                    float raw = upgradeOptions.playerStats[index] - valueIncrease;
                    float value = MathF.Round(raw, 4);

                    if (value < minValue)
                        return;

                    upgradeOptions.UpdatePlayerStat(value, index);
                    UpdateValue(value);
                    upgradeOptions.pointsUsed--;

                    minusButton.colorTint = (value <= minValue) ? Color.Gray : Color.White;
                },
                parameters: [minusButton]
            );

            UpdateValue(value);
        }

        public void UpdateValue(float newValue)
        {
            newValue = MathF.Round(newValue, 2); // Round to 2 decimal places

            plusButton.isDisabled = newValue >= maxValue;
            minusButton.isDisabled = newValue <= minValue;
            plusButton.colorTint = (plusButton.isDisabled) ? Color.Gray : Color.White;
            minusButton.colorTint = (minusButton.isDisabled) ? Color.Gray : Color.White;

            int upgradeLevelsCount = (int)((maxValue - minValue) / valueIncrease);
            int currUpgradeLevel = (int)((newValue - minValue) / valueIncrease);

            spriteTextComponent.text = index == 3 || index == 4 ? $"{name};</orange> {newValue * 100f:F0}%" : $"{name};</orange> {newValue}";

            upgradeSpriteTextComponent.text = $"</white>({currUpgradeLevel}∕{upgradeLevelsCount})";
        }
    }

    private void CreateUpgradeOptionObjects(Panel parentPanel, int width, int height)
    {
        string[] texts = ["HEALTH", "SHIELD", "ATTACK", "CRIT RATE", "CRIT DMG"];
        float[] valueIncrease = [20, 10, 5, 0.1f, 0.2f];
        float[] valuesMin = [100, 50, 5, 0.5f, 0.2f];
        float[] valuesMax = [500, 300, 50, 1f, 2f];

        playerStats = valuesMin;

        GameObject upgradeToolbarParent = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Salamandra-UpgradePose"),
            panelColor: Color.White
        );
        upgradeToolbarParent.transform.localPosition = new Vector2(-50, 0);
        parentPanel.gameObject.AddChild(upgradeToolbarParent, isOverlay: true);

        int toolbarWidth = 360;
        int titleToolbarWidth = toolbarWidth - 80;
        int toolbarHeight = 50;
        float xPos = 180 + toolbarWidth / 2;
        float yPos = -220;

        GameObject availablePointsObject = PrefabObjectSliderWithLabels.PanelObject(
           width: titleToolbarWidth,
           height: toolbarHeight - 10,
           texture2D: JSON_Manager.uiSpriteSheet,
           sourceRectangle: JSON_Manager.GetUITile("Button3"),
           panelColor: GameConstantsAndValues.PanelColor_GrayFull
        );
        availablePointsObject.transform.localPosition = new Vector2(xPos, yPos + 5); ;
        upgradeToolbarParent.AddChild(availablePointsObject, isOverlay: true);

        availablePointsSpriteText = new SpriteTextComponent(
            width: titleToolbarWidth - 20,
            height: toolbarHeight - 10,

        JSON_Manager.customBitmapFont,
            $"AVAILABLE POINTS; </yellow>{pointsAvailable - pointsUsed}"
            , fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Middle,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_HM, spacingX: 2, spacingY: 0, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        GameObject_TextField titleTextField = new GameObject_TextField(availablePointsSpriteText);
        availablePointsObject.AddChild(titleTextField, isOverlay: true);


        // upgrade options
        for (int i = 0; i < texts.Length; i++)
        {
            float yPos_ = yPos + (i + 1) * (toolbarHeight + 5);
            UpgradeOption upgradeOption = CreateUpgradeOptionObject(
                parent: upgradeToolbarParent,
                width: toolbarWidth,
                height: toolbarHeight,
                index: i,
                text: texts[i],
                value: valuesMin[i],
                valueIncrease: valueIncrease[i],
                minValue: valuesMin[i],
                maxValue: valuesMax[i],
                localPosition: new Vector2(xPos, yPos_)
            );

            playerStatValues[i] = upgradeOption;
        }
    }

    private UpgradeOption CreateUpgradeOptionObject(GameObject parent, int width, int height,
        int index, string text, float value, float valueIncrease, float minValue, float maxValue, Vector2 localPosition)
    {
        GameObject upgradeBarObject = PrefabObjectSliderWithLabels.PanelObject(
           width: width,
           height: height,
           texture2D: JSON_Manager.uiSpriteSheet,
           sourceRectangle: JSON_Manager.GetUITile("Button3"),
           panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull
        );
        upgradeBarObject.transform.localPosition = localPosition;
        parent.AddChild(upgradeBarObject, isOverlay: true);

        //label
        SpriteTextComponent spriteTextComponent = new SpriteTextComponent(
            width: width - 20,
            height: height,
        JSON_Manager.customBitmapFont,
            $"{text} </orange> Z", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_HM, spacingX: 2, spacingY: 0, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        GameObject_TextField titleTextField = new GameObject_TextField(spriteTextComponent);
        upgradeBarObject.AddChild(titleTextField, isOverlay: true);


        SpriteTextComponent upgradeSpriteTextComponent = new SpriteTextComponent(
            width: width - 95,
            height: height,
            JSON_Manager.customBitmapFont,
            $"(22∕22)", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal,
            textCenterX: BitmapFont_equalHeight_dynamicWidth.CenterX.Right,
            textCenterY: BitmapFont_equalHeight_dynamicWidth.CenterY.Middle,
            fontSize: GameConstantsAndValues.FONT_SIZE_HM, spacingX: 2, spacingY: 0, color: Color.White,
            graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice
        );
        titleTextField = new GameObject_TextField(upgradeSpriteTextComponent);
        upgradeBarObject.AddChild(titleTextField, isOverlay: true);

        int buttonSize = height - 6;
        // plus button

        Button plusButtonObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: -40 - 2 * buttonSize,//-10 - buttonSize,
            curr_y_offset: 0,
            upgradeBarObject.GetComponent<Panel>(), PivotCentering.Enum_Pivot.CenterRight
        ).GetComponent<Button>();

        plusButtonObject.sourceRectangle = JSON_Manager.GetUITile("PlusMinusButton");
        GameObject plusSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 30,
            height: 30,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("+ButtonIcon"),
            panelColor: Color.White
        );
        plusButtonObject.gameObject.AddChild(plusSpriteObject, isOverlay: true);

        parent.AddChild(upgradeBarObject, isOverlay: true);

        // minus button
        Button minusButtonObject = Menu.ButtonGameObject(
            buttonText: "",
            buttonWidth: buttonSize,
            buttonHeight: buttonSize,
            buttonColor: Color.White,
            curr_x_offset: -5,  //-35 - buttonSize, 
            curr_y_offset: 0,
            upgradeBarObject.GetComponent<Panel>(), PivotCentering.Enum_Pivot.CenterRight
        ).GetComponent<Button>();

        minusButtonObject.sourceRectangle = JSON_Manager.GetUITile("PlusMinusButton");
        GameObject minusSpriteObject = PrefabObjectSliderWithLabels.PanelObject(
            width: 30,
            height: 6,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("-ButtonIcon"),
            panelColor: Color.White
        );
        minusSpriteObject.GetComponent<Panel>().resizeType = IResizableVisualComponent.ResizeType.Fill;
        minusButtonObject.gameObject.AddChild(minusSpriteObject, isOverlay: true);

        UpgradeOption upgradeOption = new UpgradeOption(
            upgradeOptions: this,
            fullGameObject: upgradeBarObject,
            plusButton: plusButtonObject,
            minusButton: minusButtonObject,
            name: text,
            spriteTextComponent,
            upgradeSpriteTextComponent,
            index: index,
            value: value,
            valueIncrease: valueIncrease,
            minValue: minValue,
            maxValue: maxValue
        );

        return upgradeOption;
    }
}

