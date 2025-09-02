using Microsoft.Xna.Framework;

public class ConditionController : GameComponent
{
    public static ConditionController? Instance;

    public ConditionController(Game game) : base(game)
    {
        if (Instance is null) Instance = this;
    }

    List<ICondition> enabledConditions = new List<ICondition>();
    List<ICondition> disabledConditions = new List<ICondition>();

    public override void Update(GameTime gameTime)
    {
        foreach (ICondition condition in enabledConditions)
        {
            condition.Update(gameTime);
        }
        base.Update(gameTime);
    }

    public void EnableCondition(ICondition condition, bool enabled)
    {
        if (enabled)
        {
            enabledConditions.Add(condition);
            disabledConditions.Remove(condition);
            return;
        }

        enabledConditions.Remove(condition);
        disabledConditions.Add(condition);
    }

    public void AddCondition(ICondition condition)
    {
        enabledConditions.Add(condition);
    }

    public void RemoveCondition(ICondition condition)
    {
        enabledConditions.Remove(condition);
    }
}
