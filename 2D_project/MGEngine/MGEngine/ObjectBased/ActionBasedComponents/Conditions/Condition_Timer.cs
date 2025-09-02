using Microsoft.Xna.Framework;

public class Condition_Timer(float timer) : ICondition
{
    private readonly float time = timer;
    float timeRemaining = timer;

    public bool isTimerPaused = false;

    public event EventHandler? OnConditionMet;

    public void ResetTimer()
    {
        timeRemaining = time;
        isTimerPaused = false;
    }

    public void Update(GameTime gameTime)
    {
        if (isTimerPaused) return;

        timeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (timeRemaining < 0)
        {
            timeRemaining = time;
            OnConditionMet?.Invoke(this, EventArgs.Empty);
        }
    }
}