using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using MGEngine.Physics;
using Microsoft.Xna.Framework;

internal class MoveOnCollisionPlatformComponent : MovePlayerWithPlatformLogic, IResettable
{
    private float movementSpeed;
    private Vector2 velocity;

    private GameObject startGameObject;
    private GameObject endGameObject;

    private enum MovementType
    {
        Forward,
        Backward
    }

    private MovementType _movementType = MovementType.Backward;

    // Resetable values
    private bool originalIsActive;

    private Vector2 originalGlobalPosition;
    private float originalLocalRotation;
    private Vector2 originalLocalScale;

    private Vector2 originalVelocity;
    private bool originalIsMovable;
    private float originalAngularVelocity;
    // end

    public MoveOnCollisionPlatformComponent(Vector2 StartTilePosition, Vector2 EndTilePosition, int WidthInTiles, int HeightInTiles, float MovementSpeed = 200)
    {
        movementSpeed = MovementSpeed;
        startGameObject = new GameObject(tag: GameConstantsAndValues.Tags.Hidden.ToString());
        startGameObject.CreateTransform();
        Terrain_InvisibleCollider colliderObject = new Terrain_InvisibleCollider(widthInTiles: WidthInTiles, heightInTiles: HeightInTiles, mass: 10000);
        startGameObject.AddComponent(colliderObject);
        startGameObject.transform.globalPosition = StartTilePosition * GameConstantsAndValues.SQUARE_TILE_WIDTH;

        endGameObject = new GameObject(tag: GameConstantsAndValues.Tags.Hidden.ToString());
        endGameObject.CreateTransform();
        colliderObject = new Terrain_InvisibleCollider(widthInTiles: WidthInTiles, heightInTiles: HeightInTiles, mass: 10000);
        endGameObject.AddComponent(colliderObject);
        endGameObject.transform.globalPosition = EndTilePosition * GameConstantsAndValues.SQUARE_TILE_WIDTH;
    }

    public void AdjustToLevelStartPosition(Vector2 levelStartPosition)
    {
        startGameObject.transform.globalPosition += levelStartPosition;
        endGameObject.transform.globalPosition += levelStartPosition;
    }

    public override void Initialize()
    {
        base.Initialize();
        Scene scene = SceneManager.Instance.GetGameObjectScene(gameObject);
        scene.AddGameObjectToScene(startGameObject, isOverlay: false);
        scene.AddGameObjectToScene(endGameObject, isOverlay: false);
        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();
        Movement.AssignVelocity(
            finalPosition: endGameObject.transform.globalPosition,
            physicsComponent: physicsComponent,
            speed: movementSpeed
        );
        velocity = physicsComponent.Velocity;

        // resetable values
        originalIsActive = gameObject.isActive;
        originalGlobalPosition = gameObject.transform.globalPosition;
        originalLocalRotation = gameObject.transform.localRotationAngle;
        originalLocalScale = gameObject.transform.localScale;

        PhysicsComponent ps = gameObject.GetComponent<PhysicsComponent>();
        originalVelocity = ps.Velocity;
        originalIsMovable = ps.isMovable;
        originalAngularVelocity = ps.AngularVelocity;
    }

    public override void BeginMovement()
    {
        // assign movement values
        _movementType = (_movementType == MovementType.Forward) ? MovementType.Backward : MovementType.Forward;

        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();
        physicsComponent.Velocity = (_movementType == MovementType.Forward) ? velocity : -velocity;

        base.BeginMovement();
    }
    protected override void OnCollision(Collider collider)
    {
        // disable movement upon reaching desination
        if (_movementType == MovementType.Forward && collider.gameObject == endGameObject
           || _movementType == MovementType.Backward && collider.gameObject == startGameObject)
        {
            PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();
            StopMovement();
            physicsComponent.Velocity = Vector2.Zero;
            physicsComponent.Acceleration = Vector2.Zero;

            // restart movement
            BeginMovement();
        }

        base.OnCollision(collider); // apply player movement check
    }

    public void Reset()
    {
        _movementType = MovementType.Forward;

        gameObject.SetActive(originalIsActive);
        gameObject.transform.globalPosition = originalGlobalPosition;
        gameObject.transform.localRotationAngle = originalLocalRotation;
        gameObject.transform.localScale = originalLocalScale;

        PhysicsComponent ps = gameObject.GetComponent<PhysicsComponent>();
        ps.Velocity = originalVelocity;
        ps.isMovable = originalIsMovable;
        ps.AngularVelocity = originalAngularVelocity;

        PhysicsComponent physicsComponent = gameObject.GetComponent<PhysicsComponent>();
        Movement.AssignVelocity(
            finalPosition: endGameObject.transform.globalPosition,
            physicsComponent: physicsComponent,
            speed: movementSpeed
        );
    }
}