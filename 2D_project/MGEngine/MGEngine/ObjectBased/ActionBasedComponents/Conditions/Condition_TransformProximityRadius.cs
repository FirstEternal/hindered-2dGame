using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

public class Condition_TransformProximityRadius(bool isWithinRange, float radius, Transform transformA, Transform transformB) : ICondition
{
    private readonly Transform transformA = transformA;
    private readonly Transform transformB = transformB;
    private readonly float withinRange = radius;
    public event EventHandler? OnConditionMet;

    public void Update(GameTime gameTime)
    {
        bool isInsideRange = Vector2.Distance(transformA.globalPosition, transformB.globalPosition) <= withinRange;

        if ((isWithinRange && isInsideRange) || (!isWithinRange && !isInsideRange))
        {
            OnConditionMet?.Invoke(this, EventArgs.Empty);
        }
    }
}
