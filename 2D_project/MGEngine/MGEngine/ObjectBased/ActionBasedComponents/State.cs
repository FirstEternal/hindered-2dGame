using Microsoft.Xna.Framework;
public class State
{
    readonly Dictionary<ICondition, bool> exitConditions = new Dictionary<ICondition, bool>();

    public EventHandler? OnStateEnd;
    public readonly string stateName;
    public State(string stateName, ICondition[] exitConditions)
    {
        this.stateName = stateName;
        // subscribe to all exit conditions
        foreach (ICondition condition in exitConditions)
        {
            this.exitConditions[condition] = false;
            condition.OnConditionMet -= Condition_OnExitConditionMet;
            condition.OnConditionMet += Condition_OnExitConditionMet;
        }
    }

    public virtual void OnStateEnter()
    {
        // subscribe to all conditions
        foreach (ICondition condition in exitConditions.Keys)
        {
            exitConditions[condition] = false;
        }
    }

    public virtual void Update(GameTime gameTime)
    {
        // update all exit conditions
        foreach (ICondition condition in exitConditions.Keys)
        {
            condition.Update(gameTime);
        }
    }

    private void Condition_OnExitConditionMet(object? sender, EventArgs e)
    {
        if (sender is null) return;

        ICondition condition = (ICondition)sender;

        exitConditions[condition] = true;

        if (!exitConditions.ContainsValue(false))
        {
            // all conditions are met -> exit state
            OnStateExit();
        }
    }

    public virtual void OnStateExit()
    {
        // unsubscribe from all conditions
        foreach (ICondition condition in exitConditions.Keys)
        {
            exitConditions[condition] = false;
        }

        OnStateEnd?.Invoke(this, EventArgs.Empty);
    }
}
