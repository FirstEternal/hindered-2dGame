
public class Condition_OnButtonClick : ICondition
{
    public Condition_OnButtonClick(Button button)
    {
        button.OnClick += OnButtonClick;
    }

    private void OnButtonClick(object? sender, EventArgs e)
    {
        OnConditionMet?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? OnConditionMet;

}
