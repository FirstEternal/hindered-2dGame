using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;

internal class MovePlayerWithPlatformLogic : ObjectComponent
{
    private bool isPlayerOnPlatform;
    private bool playerTouchedThisFrame;

    public event EventHandler OnStop;

    protected void StopMovement()
    {
        OnStop?.Invoke(this, EventArgs.Empty);

        if (isPlayerOnPlatform)
            Player.Instance.plaftormJumpVelocityY = 0;

        gameObject.GetComponent<PhysicsComponent>().isMovable = false;
    }

    public override void Initialize()
    {
        base.Initialize();
        BeginMovement();
    }

    public virtual void BeginMovement()
    {
        ApplyPlayerPlatformMovement(deltaTime: 0.02f);
        gameObject.GetComponent<PhysicsComponent>().isMovable = true;
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        ApplyPlayerPlatformMovement(deltaTime);

        // After applying movement, update platform state based on contact
        isPlayerOnPlatform = playerTouchedThisFrame;
        playerTouchedThisFrame = false;

        base.Update(gameTime);
    }

    private void ApplyPlayerPlatformMovement(float deltaTime)
    {
        if (isPlayerOnPlatform)
        {
            PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();

            if (physicsComponent.isMovable)
            {
                var movement = deltaTime * (physicsComponent.Velocity + physicsComponent.cumulatedVelocity);
                Player.Instance.gameObject.transform.globalPosition += movement;
                Player.Instance.plaftormJumpVelocityY = physicsComponent.Velocity.Y;
            }
        }
    }

    public override void OnCollisionEnter(Collider collider)
    {
        base.OnCollisionEnter(collider);
        if (gameObject.GetComponent<PhysicsComponent>().isMovable)
        {
            CheckIfPlayerTouched(collider);
            OnCollision(collider);
        }
    }

    public override void OnDetectionRange(Collider collider)
    {
        base.OnDetectionRange(collider);
        if (gameObject.GetComponent<PhysicsComponent>().isMovable)
        {
            CheckIfPlayerTouched(collider);
            OnCollision(collider);
        }
    }
    protected virtual void OnCollision(Collider collider)
    {
        // override in descendants
    }

    private void CheckIfPlayerTouched(Collider collider)
    {
        if (collider.gameObject.GetComponent<Player>() != null)
        {
            playerTouchedThisFrame = true;
        }
    }
}