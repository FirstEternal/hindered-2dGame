using Microsoft.Xna.Framework;

public interface ITransformData
{
    public Vector2 GlobalPosition { get; set; }
    public float LocalRotation { get; set; }
    public Vector2? LocalScale { get; set; }
}
