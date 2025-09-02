using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
internal class GameObject_ChallangeScoreUI : Menu
{
    Label challangeScoreLabel;
    Panel[] starConditionPanels = new Panel[3];
    Label[] starConditionLabels = new Label[3];

    bool isPanelInvisible;
    public GameObject_ChallangeScoreUI(bool isPanelInvisible = false)
    {
        this.isPanelInvisible = isPanelInvisible;
    }

    public void CreateUI()
    {
        CreateMenu();
    }

    protected override void CreateMenu()
    {
        int starWidth = 40;
        int starHeight = 40;
        int innerPanelOffset = 10;
        int offset = 10;

        int panelWidth = 420;
        int panelHeight = starWidth * 3 + offset * 4 + innerPanelOffset;
        // create ui
        GameObject background_inner_panelObject = CreateBackgroundPanel(panelWidth, panelHeight, isPanelInvisible);

        LabelGameObject(labelText: "CHALLENGE: ", 420, starHeight, labelColor: new Color(0, 0, 0, 0), 0, -40,
            background_inner_panelObject.parent.GetComponent<Panel>(), PivotCentering.Enum_Pivot.TopCenter).GetComponent<Label>();

        challangeScoreLabel = LabelGameObject(labelText: "CHALLENGE: ", 420, starHeight, labelColor: new Color(0, 0, 0, 0), 0, 40,
            background_inner_panelObject.parent.GetComponent<Panel>(), PivotCentering.Enum_Pivot.BottomLeft).GetComponent<Label>();

        Panel parentLabel = background_inner_panelObject.GetComponent<Panel>();

        // create stars gained UI
        int currOffsetY = offset;
        GameObject star1 = StarUI(index: 0, parentLabel, offset: new Vector2(10, currOffsetY), starWidth, starHeight);
        currOffsetY = currOffsetY + starWidth + offset;
        GameObject star2 = StarUI(index: 1, parentLabel, offset: new Vector2(10, currOffsetY), starWidth, starHeight);
        currOffsetY = currOffsetY + starWidth + offset;
        GameObject star3 = StarUI(index: 2, parentLabel, offset: new Vector2(10, currOffsetY), starWidth, starHeight);
    }

    private GameObject StarUI(int index, Panel parentPanel, Vector2 offset, int width, int height)
    {
        // create a star panel
        GameObject art_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("ChallangeStar"),
            panelColor: Color.Black//Color.White
        );
        parentPanel.gameObject.AddChild(art_panelObject, isOverlay: true);

        PivotCentering.UpdatePivot(parentPanel, art_panelObject.GetComponent<Panel>(), art_panelObject.transform,
            PivotCentering.Enum_Pivot.TopLeft, offset);

        starConditionPanels[index] = art_panelObject.GetComponent<Panel>();
        // create a textField
        int labelWidth = 340; // panel width - star width - 10
        int startWidth = 40;
        int labelHeight = startWidth;
        starConditionLabels[index] = LabelGameObject(labelText: "", labelWidth, labelHeight, labelColor: new Color(0, 0, 0, 0), startWidth, (int)offset.Y,
            parentPanel, PivotCentering.Enum_Pivot.TopLeft).GetComponent<Label>();

        return art_panelObject;
    }

    // MAKE SURE TO UPDATE VALUES BEFORE SETING MENU GAMEOBJECT TO ACTIVE
    public void UpdateValues(ChallengeData challengeData)
    {
        for (int i = 0; i < starConditionLabels.Length; i++)
        {
            // update star condition
            //starConditionLabels[i].textField.spriteTextComponent.text = $"Within {challengeData.starConditionArray[i]}s".ToUpper();
            starConditionLabels[i].textField.spriteTextComponent.text = $"{challengeData.starConditionArray[i]}s".ToUpper();
            // update star image -> if within time conditions
            starConditionPanels[i].colorTint = challengeData.starAchievedArray[i] ? Color.White : Color.Black;
        }
        string challengeScore = challengeData.challangeTimeScore < 8000 ? challengeData.challangeTimeScore.ToString("F2") : "X";
        //challengeScore = "5";
        //challangeScoreLabel.textField.spriteTextComponent.text = $"Challenge score: {challengeScore}".ToUpper();
        challangeScoreLabel.textField.spriteTextComponent.text = $"score: {challengeScore}".ToUpper();
    }
}
