using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class HomeBaseScene(Game game) : TestingScene(game)
{
    Game game = game;
    HomeBasePortal homeBasePortal;
    int elementCount = 0;
    protected override void InitializeContent()
    {
        // background -> black border
        GameObject background_panelObject = PrefabObjectSliderWithLabels.PanelObject(
            width: GameWindow.Instance.windowWidth,
            height: GameWindow.Instance.windowHeight,
            localPosition: new Vector2(GameWindow.Instance.windowWidth / 2, GameWindow.Instance.windowHeight / 2),
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("background"),
            panelColor: new Color(0, 0, 0, 0)
        );
        AddGameObjectToScene(background_panelObject, isOverlay: true);

        GameObject buttonObject = Menu.ButtonGameObject(
            buttonText: "Element Unlock System",
            buttonWidth: 600,
            buttonHeight: 40,
            buttonColor: Color.White,
            curr_x_offset: 0,
            curr_y_offset: -40,
            parentPanel: background_panelObject.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.BottomCenter
        );

        AddGameObjectToScene(buttonObject, isOverlay: true);
        base.InitializeContent();
        homeBasePortal = new HomeBasePortal(game, scene: this);


        buttonObject.GetComponent<Button>().AssignOnClickAction((parameters) =>
        {
            elementCount++;
            if (elementCount > 1) return;

            int[] values = { 0, 1, 2, 3, 4, 5, 6 };
            bool findNewElement = true;

            while (findNewElement)
            {
                GameConstantsAndValues.FactionType element = (GameConstantsAndValues.FactionType)values[Game2DPlatformer.Instance.random.Next(values.Length)];
                if (homeBasePortal.elementsAcquired[element])
                {
                    findNewElement = true;
                }
                else
                {
                    findNewElement = false;
                    homeBasePortal.AddElement(element);
                }
            }

        }, null);

    }
}
