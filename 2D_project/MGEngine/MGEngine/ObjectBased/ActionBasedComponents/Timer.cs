using Microsoft.Xna.Framework;

public class Timer : GameComponent
{
    public bool IsRunning { get; private set; } = false;

    private float countdownTime;
    private float currentTime;

    public Action<Timer>? OnCountdownEnd;

    public Timer(Game game, float countdownTime) : base(game)
    {
        this.countdownTime = countdownTime;
        this.currentTime = countdownTime;
        game.Components.Add(this);
    }

    public void OverrideTimer(float countdownTime)
    {
        this.countdownTime = countdownTime;
        this.currentTime = countdownTime;
    }

    public void BeginTimer()
    {
        if (isDisposed) throw new ObjectDisposedException(nameof(Timer));
        currentTime = countdownTime;
        IsRunning = true;
    }


    public override void Update(GameTime gameTime)
    {
        if (!IsRunning) return;

        currentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (currentTime <= 0)
        {
            IsRunning = false;
            OnCountdownEnd?.Invoke(this);
            // Optional auto-cleanup:
            //Game.Components.Remove(this);
        }
    }

    private bool isDisposed = false;

    protected override void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing)
        {
            // Clear references to avoid memory leaks
            OnCountdownEnd = null;
        }

        isDisposed = true;
        base.Dispose(disposing);
    }

}
