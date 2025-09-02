public interface IOnHover
{
    public delegate void OnHoverEnterAction(object[] parameters);
    public delegate void OnHoverExitAction(object[] parameters);

    public OnHoverEnterAction? onHoverEnterAction { get; set; }
    object[]? onHoverEnterParameters { get; set; }

    public OnHoverExitAction? onHoverExitAction { get; set; }
    object[]? onHoverExitParameters { get; set; }

    public EventHandler? OnHoverEnter { get; set; }
    public EventHandler? OnHoverExit { get; set; }

    public bool isHovered { get; set; }
}
