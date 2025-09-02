using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class GameObject_ChallangeBossInformation : Menu
{
    Label bossNameLabel;
    int maxArtPanelWidth;
    int maxArtPanelHeight;
    Panel bossArtPanel;

    public GameObject_ChallangeBossInformation(int panelWidth, int panelHeight) : base()
    {
        Panel panel = new Panel(panelWidth, panelHeight, texture2D: JSON_Manager.uiSpriteSheet, new Color(0, 0, 0, 0));
        panel.sourceRectangle = JSON_Manager.GetUITile("background");
        panel.origin = new Vector2(panel.sourceRectangle.Width / 2, panel.sourceRectangle.Height / 2);
        AddComponent(panel);
    }
    public void CreateUI()
    {
        CreateMenu();
    }

    protected override void CreateMenu()
    {
        int bossNameLabelHeight = 40;
        // img panel
        maxArtPanelWidth = (int)(GetComponent<Panel>().width - 20);
        maxArtPanelHeight = (int)(GetComponent<Panel>().height - 20 - bossNameLabelHeight);

        GameObject art_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: maxArtPanelWidth,
            height: maxArtPanelHeight,
            texture2D: JSON_Manager.enemiesSpriteSheet,
            sourceRectangle: Rectangle.Empty,
            panelColor: Color.White
        );
        AddChild(art_panelObject, isOverlay: true);
        bossArtPanel = art_panelObject.GetComponent<Panel>();

        Color aplha = new Color(0, 0, 0, 0);
        bossNameLabel = LabelGameObject(labelText: "", maxArtPanelWidth, bossNameLabelHeight, labelColor: aplha, 10, 0,
            GetComponent<Panel>(), PivotCentering.Enum_Pivot.TopLeft).GetComponent<Label>();
    }

    // MAKE SURE TO UPDATE VALUES BEFORE SETING MENU GAMEOBJECT TO ACTIVE
    public void UpdateValues(string bossName, Rectangle bossArtSourceRectangle)
    {
        bossArtPanel.sourceRectangle = bossArtSourceRectangle;
        bossArtPanel.origin = new Vector2(bossArtPanel.sourceRectangle.Width / 2, bossArtPanel.sourceRectangle.Height / 2);
        bossNameLabel.textField.spriteTextComponent.text = $"{bossName}";

        // TODO FIGURE OUT IMAGE SCALING -> so that there are no ugly pixels -> make new ui if necessary
        /*
        // update aspect ratio
        int originalWidth = bossArtSourceRectangle.Width;
        int originalHeight = bossArtSourceRectangle.Height;

        float aspectRatio = originalWidth / originalHeight;


        if (aspectRatio > 1) // Wider than tall
        {
            // Base scaling on width
            bossArtPanel.width = maxArtPanelWidth;
            bossArtPanel.height = (int)(maxArtPanelWidth / aspectRatio);

            // If height exceeds maxArtPanelHeight, scale based on height instead
            if (bossArtPanel.height > maxArtPanelHeight)
            {
                bossArtPanel.height = maxArtPanelHeight;
                bossArtPanel.width = (int)(maxArtPanelHeight * aspectRatio);
            }
        }
        else // Taller than wide or square
        {
            // Base scaling on height
            bossArtPanel.height = maxArtPanelHeight;
            bossArtPanel.width = (int)(maxArtPanelHeight * aspectRatio);

            // If width exceeds maxArtPanelWidth, scale based on width instead
            if (bossArtPanel.width > maxArtPanelWidth)
            {
                bossArtPanel.width = maxArtPanelWidth;
                bossArtPanel.height = (int)(maxArtPanelWidth / aspectRatio);
            }
        }*/
    }
}