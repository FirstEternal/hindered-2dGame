using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Diagnostics;

public class Scene(Game game, Camera? mainCamera = null) : GameComponent(game), IFixedUpdate
{
    public Camera mainCamera { get; private set; } = mainCamera ?? new Camera(); // TODO creat transform for the camera

    protected StateController? sceneStateController;
    public float UnpausedTotalSceneTime;
    public EventHandler? OnSceneLoad;
    public EventHandler? OnGameObjectAdded;
    public EventHandler? OnGameObjectRemoved;

    public GameObject GetGameObjectInScene(int gameObjectID, bool isOverlay)
    {
        foreach (GameObject gameObject in isOverlay ? overlayGameObjects : gameObjects)
        {
            if (gameObject.id == gameObjectID)
            {
                return gameObject;
            }
        }

        return null;
    }
    public void CreateStateController(List<State> states)
    {
        sceneStateController = new StateController(states);
        sceneStateController.OnStateChange += StateController_OnStateChange;
    }
    public virtual void StateController_OnStateChange(object? sender, EventArgs e)
    {
        // on state change
    }

    public string GetCurrentStateName()
    {
        return sceneStateController?.GetCurrentStateName() ?? "";
    }

    public bool isPaused = false;

    /// <summary>
    /// rendered by scene Renderer
    /// </summary>
    public List<GameObject> gameObjects = new List<GameObject>();

    public List<Collider> collidersInProximity = new List<Collider>();

    /// <summary>
    /// rendered by Overlay Renderer
    /// </summary>
    public List<GameObject> overlayGameObjects = new List<GameObject>();

    private bool isInitialized = false;

    public virtual void AddGameObjectToScene(GameObject gameObject, bool isOverlay)
    {
        if (isOverlay && !overlayGameObjects.Contains(gameObject)) overlayGameObjects.Add(gameObject);
        else if (!isOverlay && !gameObjects.Contains(gameObject)) gameObjects.Add(gameObject);


        gameObject.SceneStatusChanged(this, initiateRemoval: false);
        //gameObject.AddChildrenToScene(isOverlay);
        /*
        OnGameObjectAdded -= gameObject.OnAddedToScene;
        OnGameObjectAdded += gameObject.OnAddedToScene;

        OnGameObjectAdded -= gameObject.OnRemovedFromScene;
        OnGameObjectAdded += gameObject.OnRemovedFromScene;
        */
    }

    public virtual void RemoveGameObjectFromScene(GameObject gameObject, bool isOverlay)
    {/*
        bool removalSucceded = false;
        if (isOverlay) removalSucceded = overlayGameObjects.Remove(gameObject);
        else removalSucceded = gameObjects.Remove(gameObject);
        */

        bool removalSucceded = isOverlay? overlayGameObjects.Remove(gameObject): gameObjects.Remove(gameObject);
        if (removalSucceded) gameObject.SceneStatusChanged(this, initiateRemoval: true);

        //gameObject.RemoveChildrenFromScene(isOverlay);
        /*
        OnGameObjectAdded -= gameObject.OnAddedToScene;
        OnGameObjectAdded -= gameObject.OnRemovedFromScene;*/
    }

    public void ChangeMainCamera(Camera camera)
    {
        mainCamera = camera;
    }
    private void InitializeSelf()
    {
        
        if (!isInitialized)
        {
            isInitialized = true;
            InitializeContent();
            LoadContent();

            // load content if it has not already been initialized
        }

        UnpausedTotalSceneTime = 0;

        OnSceneLoad?.Invoke(this, EventArgs.Empty);
        

        /*
        Debug.WriteLine(
      $"[InitializeSelf ENTRY] Scene Hash={this.GetHashCode()}, isInitialized={isInitialized}"
  );
        Debug.WriteLine(Environment.StackTrace);

        if (!isInitialized)
        {
            isInitialized = true;
            InitializeContent();
            LoadContent();
        }

        Debug.WriteLine(
            $"[InitializeSelf EXIT] Scene Hash={this.GetHashCode()}, isInitialized={isInitialized}"
        );

        UnpausedTotalSceneTime = 0;
        OnSceneLoad?.Invoke(this, EventArgs.Empty);*/
    }

    /// <summary>
    /// this will dispose of all game objects within a scene, reinitialize content
    /// </summary>
    /// 
    public virtual void UnloadContent()
    {
        // dispose of all gameObjects
        // initialize content for all game objects in the scene
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].Dispose();
        }

        gameObjects.Clear();

        // initialize content for all overlay game objects
        for (int i = 0; i < overlayGameObjects.Count; i++)
        {
            overlayGameObjects[i].Dispose();
        }

        overlayGameObjects.Clear();

        isInitialized = false;
    }

    protected virtual void InitializeContent()
    {
        /*
        Debug.WriteLine(
            $"[InitializeContent ENTRY] Scene Hash={this.GetHashCode()}"
        );
        Debug.WriteLine(Environment.StackTrace);

        Debug.WriteLine("initializing content (StageScene)");*/
    }

    public override void Initialize()
    {
        if (isInitialized) return;

        InitializeSelf();

        // initialize content for all game objects in the scene
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].Initialize();
        }

        // initialize content for all overlay game objects
        for (int i = 0; i < overlayGameObjects.Count; i++)
        {
            overlayGameObjects[i].Initialize();
        }
    }

    /// <summary>
    /// load content to all gameobjects in the scene
    /// </summary>
    public virtual void LoadContent()
    {
        // load content for all game objects in the scene
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].LoadContent();

            // if does not have a parent, update child's global transform values
            /*
            if(gameObjects[i].parent is null)
            {
                gameObjects[i].UpdateChildGlobalTransformValues();
            }*/
        }

        // load content for all overlay game objects
        for (int i = 0; i < overlayGameObjects.Count; i++)
        {
            overlayGameObjects[i].LoadContent();
            /*
            // if does not have a parent, update child's global transform values
            if (overlayGameObjects[i].parent is null)
            {
                //overlayGameObjects[i].UpdateChildGlobalTransformValues();
            }*/
        }
    }

    /// <summary>
    /// 1.) update all gameobjects in the scene 
    /// 2.) apply gravity
    /// 3.) check collision
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        // 0.) update state controller
        if (sceneStateController is not null)
        {
            sceneStateController.Update(gameTime);
        }

        // 1.) update camera
        mainCamera.Update(gameTime);

        if (!isPaused)
        {
           // base.Update(gameTime);

            // 0.) update scene total unpaused timer
            UnpausedTotalSceneTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 1.) update all active game objects
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (!gameObjects[i].isActive) continue;
                gameObjects[i].Update(gameTime);

                // 2.) apply gravity
                Physics.UpdatePhysics(gameObjects[i].GetComponent<PhysicsComponent>(), gameTime);
                // ...
            }

            // 3.) check scene GameObject collisions
            CollisionLogic.SceneCollisions();
        }

        // 4.) update all active overlay objects
        for (int i = 0; i < overlayGameObjects.Count; i++)
        {
            if (!overlayGameObjects[i].isActive) continue;
            overlayGameObjects[i].Update(gameTime);
        }

        // 5.) check overlay GameObject collisions
        CollisionLogic.OverlaySceneCollision();
    }

    /// <summary>
    /// 1.) call fixed update on all gameobjects in the scene 
    /// </summary>
    public virtual void FixedUpdate(GameTime gameTime)
    {
        // populate colliders
        CollisionLogic.PopulateColliders(gameObjects);

        // update all active game objects 
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (!gameObjects[i].isActive) continue;
            gameObjects[i].FixedUpdate(gameTime);
        }
    }
}
