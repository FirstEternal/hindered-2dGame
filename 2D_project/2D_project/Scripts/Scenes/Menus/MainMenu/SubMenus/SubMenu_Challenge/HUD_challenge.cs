using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class HUD_challenge
{
    public static void Create_HUD_Challenge(GameObject_ChallangeScoreUI challengeScoreUI, Scene scene)
    {
        GameObject backgroundPanelObject = new GameObject();
        scene.AddGameObjectToScene(backgroundPanelObject, isOverlay: true);
        backgroundPanelObject.CreateTransform(localScale: new Vector2(0.6f, 0.6f));
        int width = 500;
        int height = 300;
        backgroundPanelObject.transform.globalPosition = backgroundPanelObject.transform.localScale * new Vector2(width / 2, height / 2) + new Vector2(-10, 90);

        Panel backgroundPanel = new Panel(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            colorTint: Color.Black
        //new Color(
        //    r: GameConstantsAndValues.PanelColor_DarkBlue.R,
        //    g: GameConstantsAndValues.PanelColor_DarkBlue.G,
        //    b: GameConstantsAndValues.PanelColor_DarkBlue.B,
        //    alpha: (byte)100)
        );

        backgroundPanel.sourceRectangle = JSON_Manager.GetUITile("background");
        backgroundPanelObject.AddComponent(backgroundPanel);

        backgroundPanelObject.AddChild(challengeScoreUI, isOverlay: true);
        challengeScoreUI.CreateUI();
        challengeScoreUI.transform.globalPosition = new Vector2(0, 0);
    }

    public static void UpdateChallengeUI(GameObject_ChallangeScoreUI challengeScoreUI, ChallengeData challengeData)
    {
        challengeData.challangeTimeScore = (int)SceneManager.Instance.activeScene.UnpausedTotalSceneTime;
        // update challenge data
        for (int i = 0; i < challengeData.starAchievedArray.Length; i++)
        {
            challengeData.starAchievedArray[i] = challengeData.challangeTimeScore <= challengeData.starConditionArray[i];
        }

        // update challenge ui 
        challengeScoreUI.UpdateValues(challengeData);
    }
}
