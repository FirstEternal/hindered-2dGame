using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;

public class Condition_Collision : ICondition
{
    readonly float timeOutBetweenCollisions = 0.1f;
    float currTime = 0;
    private bool isOnTimeOut;

    public enum CollisionEventType
    {
        onEnter,
        onExit
    }
    public Condition_Collision(List<Collider> colliders, CollisionEventType collisionType)
    {
        foreach (Collider collider in colliders)
        {
            collider.onCollision += collisionType == CollisionEventType.onEnter ? Collider_OnCollision : Collider_OnCollision;
        }
    }

    private void Collider_OnCollision(object? sender, EventArgs e)
    {
        // there should be slight delay, would not want multiple colliders causing the same action
        if (isOnTimeOut) return;

        OnConditionMet?.Invoke(this, EventArgs.Empty);
        isOnTimeOut = true;
        currTime = timeOutBetweenCollisions;
    }
    public event EventHandler? OnConditionMet;

    public void Update(GameTime gameTime)
    {
        if (isOnTimeOut)
        {
            currTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currTime <= 0)
            {
                isOnTimeOut = false;
            }
        }
    }
}
