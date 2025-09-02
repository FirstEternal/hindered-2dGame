internal class Menu_Level2 : Menu_Level
{
    public Menu_Level2(Panel parentPanel, int startingChallengeIndex, int bossButtonCount, int availablePoints, bool hasStoryStages) : base(parentPanel, startingChallengeIndex, bossButtonCount, availablePoints, hasStoryStages)
    {
        challengeIndices = [startingChallengeIndex, startingChallengeIndex+1];
        AssignButtonFunctions(
            // button actions
            [
            // -> button 1
            LoadStageViaString,

            // -> button 3
            (parameters) => {
                SetActive(false);
                AssignLevel(bossName:"Froster", challengeIndices[1]);
            },
            ],
            // button parameters
            [
                ["Scene_Stage2"],
                null,
                null
            ]
        );
    }
}
