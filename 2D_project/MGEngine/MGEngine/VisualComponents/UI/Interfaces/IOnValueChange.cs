public interface IOnValueChange
{
    public delegate void OnValueChangeAction(object[] parameters);

    public OnValueChangeAction? onValueChangeAction { get; set; }
    object[]? onValueChangeParameters { get; set; }

    public EventHandler? OnValueChange { get; set; }
}
