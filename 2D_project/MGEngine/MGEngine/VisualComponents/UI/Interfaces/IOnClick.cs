public interface IOnClick
{
    public delegate void OnClickAction(object[] parameters);

    public OnClickAction? onClickAction { get; set; }
    object[]? onClickParameters { get; set; }

    public EventHandler? OnClick { get; set; }

    public bool isPressed { get; set; }
}
