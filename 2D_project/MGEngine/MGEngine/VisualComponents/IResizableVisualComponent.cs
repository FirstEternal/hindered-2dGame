using Microsoft.Xna.Framework;

public interface IResizableVisualComponent
{
    public enum ResizeType
    {
        Fill,
        Crop,
        None,
        Nine_Slice,
    }

    public ResizeType resizeType { get; set; }
    public float width { get; set; }
    public float height { get; set; }

    public Vector2 ResizedScale();

}
