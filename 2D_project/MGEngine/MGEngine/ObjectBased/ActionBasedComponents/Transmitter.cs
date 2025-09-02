using Microsoft.Xna.Framework;

public class Transmitter : ObjectComponent
{
    readonly Dictionary<ICondition, bool> conditions = new Dictionary<ICondition, bool>();

    public EventHandler? OnTransmission;
    public Transmitter(ICondition[] conditions, List<Reciever> recievers)
    {
        // subscribe to all conditions
        foreach (ICondition condition in conditions)
        {
            this.conditions[condition] = false;
            condition.OnConditionMet += Condition_OnConditionMet;
        }

        // subscribe all recievers to transmission
        foreach (Reciever reciever in recievers)
        {
            OnTransmission -= reciever.OnRecieve;
            OnTransmission += reciever.OnRecieve;
        }
    }

    public void AddReciever(Reciever reciever)
    {
        // subscribe reciever
        OnTransmission -= reciever.OnRecieve;
        OnTransmission += reciever.OnRecieve;
    }
    public void RemoveReciever(Reciever reciever)
    {
        // unsubscribe reciever
        OnTransmission -= reciever.OnRecieve;
    }

    private void Condition_OnConditionMet(object? sender, EventArgs e)
    {
        if (sender is null) return;

        ICondition condition = (ICondition)sender;

        conditions[condition] = true;

        //if (!conditions.ContainsValue(false)) OnTransmission?.Invoke(this, EventArgs.Empty);
        if (!conditions.ContainsValue(false))
        {
            OnTransmission?.Invoke(this, EventArgs.Empty);

            // reset conditions
            foreach (ICondition _condition in conditions.Keys)
            {
                conditions[_condition] = false;
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // update all conditions
        foreach (ICondition condition in conditions.Keys)
        {
            condition.Update(gameTime);
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }
}
