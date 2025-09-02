using Microsoft.Xna.Framework;

public class Condition_OnAnyCondition : ICondition
{
    public event EventHandler? OnConditionMet;
    List<ICondition> conditions;
    public Condition_OnAnyCondition(List<ICondition> conditions)
    {
        this.conditions = conditions;
        foreach (var condition in conditions)
        {
            condition.OnConditionMet -= OnAnyConditionMet;
            condition.OnConditionMet += OnAnyConditionMet;
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var condition in conditions)
        {
            condition.Update(gameTime);
        }
    }

    private void OnAnyConditionMet(object? sender, EventArgs e)
    {
        OnConditionMet?.Invoke(this, e);
    }


}
