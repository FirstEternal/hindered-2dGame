internal class Menu_Level3 : Menu_Level
{
    public Menu_Level3(Panel parentPanel, int startingChallengeIndex, int bossButtonCount, int availablePoints, bool hasStoryStages) : base(parentPanel, startingChallengeIndex, bossButtonCount, availablePoints, hasStoryStages)
    {
        challengeIndices = [startingChallengeIndex, startingChallengeIndex + 1, startingChallengeIndex + 2];

        AssignButtonFunctions(
            // button actions
            [
            // -> button 1
            // -> button 2
            (parameters) => {
                SetActive(false);
                AssignLevel(bossName:"Grasser", challengeIndices[0]);
            },

            ],
            // button parameters
            [
                ["Scene_Stage3"],
                null,
                null,
                null
            ]
        );
    }
}