public class Condition_OnEvent : ICondition
{
    public Condition_OnEvent(object? sender, string eventName)
    {
        if (sender == null) throw new ArgumentNullException(nameof(sender));
        if (string.IsNullOrEmpty(eventName)) throw new ArgumentNullException(nameof(eventName));

        var eventInfo = sender.GetType().GetEvent(eventName);
        if (eventInfo == null)
        {
            throw new ArgumentException($"The event '{eventName}' does not exist on type '{sender.GetType()}'.");
        }

        eventInfo.AddEventHandler(sender, new EventHandler(OnEvent));
    }
    private void OnEvent(object? sender, EventArgs e)
    {
        OnConditionMet?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? OnConditionMet;
}
