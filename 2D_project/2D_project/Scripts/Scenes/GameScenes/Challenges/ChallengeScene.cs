using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

internal class ChallengeScene(Game game) : Scene_Stage(game)
{
    public BossEnemy finalEnemy { get; set; }
    protected ChallengeData challengeData { get; set; }

    GameObject_ChallangeScoreUI challengeScoreUI;

    private bool hasChallengeStarted;
    protected override void InitializeContent()
    {
        Debug.WriteLineIf(true, $"initializing content{this.GetType().Name}");
        challengeScoreUI = new GameObject_ChallangeScoreUI(isPanelInvisible: true);
        HUD_challenge.Create_HUD_Challenge(challengeScoreUI, this);

        base.InitializeContent();
    }

    protected override void LevelStart()
    {
        base.LevelStart();
        ChallengeStart();
    }

    public override void RestartStage(bool resetRespawn)
    {
        base.RestartStage(resetRespawn);
        ChallengeStart();
    }

    protected virtual void ChallengeStart()
    {
        // TODO -> reset entire scene logic
        isPaused = false;

        Player.Instance.onDeath -= OnPlayerDeath;
        Player.Instance.onDeath += OnPlayerDeath;

        hudPauseMenu.PauseMenu.SetActive(false);

        hasChallengeStarted = true;

        //StartLevelPartAtIndex(this, 0);
        finalEnemy.ResetEnemy();
    }

    protected override void OnPlayerDeath(object sender, EventArgs e)
    {
        ChallengeEnd();

        gameOverMenu.UpdateDescription("GAME OVER");
        isPaused = true;
        hudPauseMenu.PauseMenu.SetActive(isPaused);
    }

    public override void Update(GameTime gameTime)
    {
        if (InputController.Instance.IsKeyPressed(Keys.Escape))
        {
            PauseGame();
        }

        if (!isPaused && hasChallengeStarted)
        {
            // update challenge ui
            HUD_challenge.UpdateChallengeUI(challengeScoreUI, challengeData);

            // check for challenge end
            if (finalEnemy.healthBar.currHealth <= 0)
            {
                gameOverMenu.UpdateDescription("LEVEL COMPLETED");

                isPaused = true;
                hudPauseMenu.PauseMenu.SetActive(isPaused);
                ChallengeEnd();
            }
        }

        base.Update(gameTime);
    }

    protected virtual void AssignBossArrivalReicever(int gameObjectID)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.id == gameObjectID)
            {
                MoveStopOnCollisionComponent msocc = gameObject.GetComponent<MoveStopOnCollisionComponent>();
                msocc.OnStop += BossArrival;
            }
        }
    }

    protected void BossArrival(object sender, EventArgs e)
    {
        finalEnemy.ResetEnemy();
        //finalEnemy.gameObject.SetActive(true);
    }

    protected virtual void ChallengeEnd()
    {
        double defaultTimeScore = 10000;
        challengeData.challangeTimeScore = (finalEnemy.healthBar.currHealth > 0) ? defaultTimeScore : UnpausedTotalSceneTime;

        for (int i = 0; i < challengeData.starAchievedArray.Length; i++)
        {
            challengeData.starAchievedArray[i] = challengeData.challangeTimeScore <= challengeData.starConditionArray[i];
        }

        // save entire session
        ExisitingPlayerSessions.SaveActiveSession();

        isPaused = true;

        // open game over menu

        Player.Instance.onDeath -= OnPlayerDeath;

        hasChallengeStarted = false;
    }

    public override void UnloadContent()
    {
        return;
        RemoveGameObjectFromScene(Player.Instance.gameObject, isOverlay: false);
        SoundController.instance.StopMusic();
        base.UnloadContent();
    }
}