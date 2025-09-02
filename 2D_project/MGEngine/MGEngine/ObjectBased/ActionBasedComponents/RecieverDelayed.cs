using Microsoft.Xna.Framework;

public class RecieverDelayed : Reciever
{
    private readonly Timer timer;
    public RecieverDelayed(Game game, PerformAction performAction, float delayActionTime) : base(performAction)
    {
        this.performAction = performAction;

        // prevent timer and delay action to be set to exact moment
        float timePrecision = 0.9999f;
        timer = new Timer(game, delayActionTime * timePrecision);

        timer.OnCountdownEnd -= Timer_OnCountdownEnd;
        timer.OnCountdownEnd += Timer_OnCountdownEnd;
    }

    public override void OnRecieve(object? sender, EventArgs e)
    {
        if (timer.IsRunning) return;
        timer.BeginTimer();
    }
    void Timer_OnCountdownEnd(Timer timer)
    {
        if (gameObject is not null) performAction?.Invoke(gameObject);
    }
}
