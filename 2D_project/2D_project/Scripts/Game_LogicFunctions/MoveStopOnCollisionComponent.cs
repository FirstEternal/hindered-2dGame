using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

internal class MoveStopOnCollisionComponent(string[] collisionTagsToStart, string[] collisionTagsToStop, float mass, Vector2? velocity, Vector2? acceleration = null, bool isGravity = true, bool isMovable = true, bool adjustToTileSystem = false, bool hideOnStopEnabled = false) : MovePlayerWithPlatformLogic, IResettable
{
    string[] collisionTagsToStart = collisionTagsToStart;
    string[] collisionTagsToStop = collisionTagsToStop;

    private float mass = mass;
    private Vector2 velocity = velocity ?? Vector2.Zero;
    private Vector2 acceleration = acceleration ?? Vector2.Zero;
    private bool isGravity = isGravity;
    private bool isMovable = isMovable;
    private bool adjustToTileSystem = adjustToTileSystem;

    // Resetable values
    private bool originalIsActive;

    private Vector2 originalGlobalPosition;
    private float originalLocalRotation;
    private Vector2 originalLocalScale;

    private float originalMass;
    private Vector2 originalVelocity;
    private Vector2 originalAcceleration;
    private bool originalIsGravity;
    private bool originalIsMovable;

    private bool isStopped;
    private Vector2 spawnPosition;

    private bool hideOnStopEnabled = hideOnStopEnabled;

    public void Reset()
    {
        PhysicsComponent ps = gameObject.GetComponent<PhysicsComponent>();
        // assign original movement properties
        ps.Mass = originalMass;
        ps.Velocity = originalVelocity;
        ps.Acceleration = originalAcceleration;
        ps.isGravity = originalIsGravity;
        ps.isMovable = originalIsMovable;

        gameObject.SetActive(originalIsActive);
        gameObject.transform.globalPosition = originalGlobalPosition;
        gameObject.transform.localRotationAngle = originalLocalRotation;
        gameObject.transform.localScale = originalLocalScale;

        isStopped = false; //

        spawnPosition = gameObject.transform.globalPosition;
    }

    public override void Initialize()
    {
        base.Initialize();

        // should have it on object
        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();

        if (physicsComponent is null) return;

        // assign original movement properties
        originalMass = physicsComponent.Mass;
        originalVelocity = physicsComponent.Velocity;
        originalAcceleration = physicsComponent.Acceleration;
        originalIsGravity = physicsComponent.isGravity;
        originalIsMovable = physicsComponent.isMovable;

        spawnPosition = gameObject.transform.globalPosition;

        originalIsActive = gameObject.isActive;
        originalGlobalPosition = gameObject.transform.globalPosition;
        originalLocalRotation = gameObject.transform.localRotationAngle;
        originalLocalScale = gameObject.transform.localScale;
    }


    public override void OnCollisionEnter(Collider collider)
    {
        base.OnCollisionEnter(collider);
        if (!isStopped) OnCollision(collider);
    }

    public override void OnDetectionRange(Collider collider)
    {
        base.OnDetectionRange(collider);
        if (!isStopped) OnCollision(collider);
    }
    protected override void OnCollision(Collider collider)
    {
        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();

        if (physicsComponent is null) return;

        //Debug.WriteLineIf(collider.gameObject.tag == "Hidden",collider.gameObject.id);
        // if both the start collider and the stop collider are colliding at the exact same time
        // prioritize stopping movement
        if (collisionTagsToStop.Length == 0 || collisionTagsToStop.Contains(collider.gameObject.tag))
        {
            isStopped = true;
            physicsComponent.Mass = originalMass;
            physicsComponent.Velocity = originalVelocity;
            physicsComponent.Acceleration = originalAcceleration;
            physicsComponent.isGravity = originalIsGravity;
            physicsComponent.isMovable = originalIsMovable;

            if (adjustToTileSystem)
            {
                int tileSize = GameConstantsAndValues.SQUARE_TILE_WIDTH / 2; // allowed to snapp to half tile
                Vector2 snappedPosition = new Vector2(
                    (float)Math.Round(gameObject.transform.globalPosition.X / tileSize) * tileSize,
                    (float)Math.Round(gameObject.transform.globalPosition.Y / tileSize) * tileSize
                );
                // place to the closect tile value
                gameObject.transform.globalPosition = snappedPosition;
            }

            StopMovement();
            if (hideOnStopEnabled) gameObject.SetActive(false);
            return;
        }

        // no collison with stop collider -> check for star collider collision 
        if (!isStopped && (collisionTagsToStop.Length == 0 || collisionTagsToStart.Contains(collider.gameObject.tag)))
        {
            // assign movement properties
            physicsComponent.Mass = mass;
            physicsComponent.Velocity = velocity;
            physicsComponent.Acceleration = acceleration;
            physicsComponent.isGravity = isGravity;
            physicsComponent.isMovable = true;
        }

        base.OnCollision(collider);
    }
}