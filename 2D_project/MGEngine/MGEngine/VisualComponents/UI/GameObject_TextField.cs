using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

public class GameObject_TextField : GameObject
{
    public SpriteTextComponent spriteTextComponent;
    public GameObject_TextField(SpriteTextComponent text, Vector2? localPosition = null, Vector2? localScale = null, float localRotationAngle = 0, int id = -1, string tag = "") : base(id, tag)
    {
        CreateTransform(localPosition, localScale, localRotationAngle);
        this.spriteTextComponent = text;

        AddComponent(text);
    }
}
