using Microsoft.Xna.Framework;

public class ButtonResponseSystem : GameComponent
{
    //private List<Button> activeButtons = new List<Button>();

    public static ButtonResponseSystem Instance;
    private float refreshTimer = 0.25f;
    private float currRefreshTimer = 0;
    private bool isPaused;

    public void Pause()
    {
        isPaused = true;
        IsOnCooldown = true;
    }

    public void Resume()
    {
        isPaused = false;

        IsOnCooldown = currRefreshTimer > 0;
    }

    public bool IsOnCooldown { get; private set; }

    public ButtonResponseSystem(Game game, float refreshTimer) : base(game)
    {
        if (Instance == null) Instance = this;
        this.refreshTimer = refreshTimer;
        /*
        SceneManager.Instance.OnSceneChange -= OnSceneChange_StopCooldown;
        SceneManager.Instance.OnSceneChange += OnSceneChange_StopCooldown;
        */
    }

    private void OnSceneChange_StopCooldown(object sender, EventArgs e)
    {
        currRefreshTimer = 0;
        Resume();
    }

    public void SubscribeButton(Button button)
    {
        button.OnClick -= BeginCooldown;
        button.OnClick += BeginCooldown;
    }

    private void BeginCooldown(object sender, EventArgs e)
    {
        IsOnCooldown = true;
        currRefreshTimer = refreshTimer;
    }

    public void ChangeDownTimeTimer(float refreshTimer)
    {
        this.refreshTimer = refreshTimer;
    }

    public override void Update(GameTime gameTime)
    {
        if (isPaused || !IsOnCooldown) return;
        currRefreshTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currRefreshTimer <= 0)
        {
            IsOnCooldown = false;
        }
    }
}
