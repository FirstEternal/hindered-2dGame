internal class Menu_Level1 : Menu_Level
{
    public Menu_Level1(Panel parentPanel, int startingChallengeIndex, int bossButtonCount, int availablePoints, bool hasStoryStages) : base(parentPanel, startingChallengeIndex, bossButtonCount, availablePoints, hasStoryStages)
    {
        challengeIndices = [startingChallengeIndex, startingChallengeIndex+1];
        AssignButtonFunctions(
            // button actions
            [
            // -> button 1
            LoadStageViaString,

            // -> button 2
            (parameters) => {
                SetActive(false);
                AssignLevel(bossName:"Burner", challengeIndices[0]);
            },
            // -> button 3
            (parameters) => {
                SetActive(false);
                AssignLevel(bossName:"Drowner", challengeIndices[1]);
            },


            //(parameters) => { SceneManager.Instance.LoadScene("Boss_Fight_Drowner"); }
            ],
            // button parameters
            [
                ["Scene_Stage1"],
                null,
                null
            ]
        );
    }
}