using MGEngine.ObjectBased;
using MGEngine.Physics;
using Microsoft.Xna.Framework;

public class Condition_TransformPosition(Vector2 position, Transform transform, float tolerationDistance) : ICondition
{
    private readonly Transform transform = transform;
    private readonly Vector2 position = position;
    private readonly float tolerationDistance = tolerationDistance;

    public event EventHandler? OnConditionMet;

    public void Update(GameTime gameTime)
    {
        if (Movement.IsInPosition(position, transform, tolerationDistance))
        {
            OnConditionMet?.Invoke(this, EventArgs.Empty);
        }
    }
}