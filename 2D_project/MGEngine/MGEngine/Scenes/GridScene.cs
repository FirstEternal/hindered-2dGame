using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

public class GridScene : Scene
{
    public GridScene(Game game, Camera? mainCamera = null) : base(game, mainCamera)
    {
    }
    
    // Grid cell size (adjust to your world scale)
    private const int cellSize = 256;

    // A dictionary to store the grid cells with their coordinates as keys
    public Dictionary<Point, GridCell> grid = new();


    // Grid cell class: holds dynamic (movable) and static objects
    public class GridCell
    {
        public List<GameObject> staticObjects = new();
        public List<GameObject> dynamicObjects = new(); // objects with PhysicsComponent.isMovable = true
    }

    // Converts a world position to a grid cell coordinate
    private Point GetCellCoord(Vector2 position)
    {
        return new Point((int)(position.X / cellSize), (int)(position.Y / cellSize));
    }

    // Add GameObject to the scene, and to the relevant grid cell
    public override void AddGameObjectToScene(GameObject gameObject, bool isOverlay)
    {
        base.AddGameObjectToScene(gameObject, isOverlay);

        if (!isOverlay)
        {
            var cellCoord = GetCellCoord(gameObject.transform.globalPosition);
            if (!grid.TryGetValue(cellCoord, out var cell))
            {
                cell = new GridCell();
                grid[cellCoord] = cell;
            }

            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics != null && physics.isMovable)
                cell.dynamicObjects.Add(gameObject); // Add to dynamic objects (movable)
            else
                cell.staticObjects.Add(gameObject); // Add to static objects
        }
    }

    // Removes GameObject from scene and grid
    public override void RemoveGameObjectFromScene(GameObject gameObject, bool isOverlay)
    {
        base.RemoveGameObjectFromScene(gameObject, isOverlay);

        if (!isOverlay)
        {
            var cellCoord = GetCellCoord(gameObject.transform.globalPosition);
            if (grid.TryGetValue(cellCoord, out var cell))
            {
                if (gameObject.GetComponent<PhysicsComponent>()?.isMovable == true)
                    cell.dynamicObjects.Remove(gameObject);
                else
                    cell.staticObjects.Remove(gameObject);
            }
        }
    }

    // Get the bounds of the camera's view (for determining which grid cells to check)
    public RectangleF GetCameraBounds()
    {
        if (mainCamera?.gameObject?.transform == null)
            return new RectangleF(0, 0, 0, 0);

        Vector2 camPos = mainCamera.gameObject.transform.globalPosition;
        Vector2 viewSize = GameWindow.Instance?.GetWindowSize() ?? new Vector2(1280, 720);
        float zoom = mainCamera.Zoom;

        float worldWidth = viewSize.X / zoom;
        float worldHeight = viewSize.Y / zoom;

        float left = camPos.X - worldWidth / 2f;
        float top = camPos.Y - worldHeight / 2f;

        return new RectangleF(left, top, worldWidth, worldHeight);
    }

    // Returns the grid cells in proximity of the camera's view
    public List<GridCell> GetNearbyCells(RectangleF bounds)
    {
        List<GridCell> cells = new();

        int minX = (int)Math.Floor(bounds.Left / cellSize);
        int maxX = (int)Math.Floor(bounds.Right / cellSize);
        int minY = (int)Math.Floor(bounds.Top / cellSize);
        int maxY = (int)Math.Floor(bounds.Bottom / cellSize);

        // Log bounds and grid cell calculation
        //Debug.WriteLine($"Bounds: Left: {bounds.Left}, Right: {bounds.Right}, Top: {bounds.Top}, Bottom: {bounds.Bottom}");
        //Debug.WriteLine($"Grid Cell Calculation: minX: {minX}, maxX: {maxX}, minY: {minY}, maxY: {maxY}");

        // Iterate through grid cells and log them
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Point cellCoord = new Point(x, y);

                if (grid.ContainsKey(cellCoord))
                {
                    cells.Add(grid[cellCoord]);
                }
            }
        }

        return cells;
    }

    // Update method, optimized to only update objects within camera bounds
    public override void Update(GameTime gameTime)
    {
        if (sceneStateController is not null)
            sceneStateController.Update(gameTime);

        if (!isPaused)
        {
            base.Update(gameTime);
            UnpausedTotalSceneTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get camera bounds and find nearby grid cells
            var cameraBounds = GetCameraBounds();
            var cellsToUpdate = GetNearbyCells(cameraBounds);
            //Debug.WriteLine(cellsToUpdate.Count);

            // Update dynamic (movable) objects in nearby cells
            foreach (var cell in cellsToUpdate)
            {
                foreach (var gameObject in cell.dynamicObjects)
                {
                    if (!gameObject.isActive) continue;

                    gameObject.Update(gameTime);

                    // Apply physics to movable objects
                    var physics = gameObject.GetComponent<PhysicsComponent>();
                    if (physics != null)
                    {
                        Physics.UpdatePhysics(physics, gameTime);
                    }
                }

                // Optionally update static objects (e.g., for things like rendering or logic)
                foreach (var gameObject in cell.staticObjects)
                {
                    if (!gameObject.isActive) continue;
                    gameObject.Update(gameTime);
                }
            }

            // Check for collisions only between relevant objects
            CollisionLogic.SceneCollisions(CollisionLogic.GetNearbyColliders(cellsToUpdate));
        }

        // Update overlay objects (independent of grid logic)
        foreach (var gameObject in overlayGameObjects)
        {
            if (!gameObject.isActive) continue;
            gameObject.Update(gameTime);
        }

        // Check for overlay collisions
        CollisionLogic.OverlaySceneCollision();
    }

    // FixedUpdate for updating dynamic objects
    public override void FixedUpdate(GameTime gameTime)
    {
        return;
        // Only update dynamic objects in relevant cells
        var cameraBounds = GetCameraBounds();
        var cellsToUpdate = GetNearbyCells(cameraBounds);

        foreach (var cell in cellsToUpdate)
        {
            foreach (var gameObject in cell.dynamicObjects)
            {
                if (!gameObject.isActive) continue;
                gameObject.FixedUpdate(gameTime);
            }
        }
    }

    // Call UnloadContent for cleaning up all objects and grid
    public override void UnloadContent()
    {
        base.UnloadContent();

        // Clear grid and all objects in it
        grid.Clear();
    }

    // Optional: Keep the grid up-to-date when moving objects
    public void UpdateObjectGridCell(GameObject gameObject, Vector2 oldPosition)
    {
        var oldCellCoord = GetCellCoord(oldPosition);
        var newCellCoord = GetCellCoord(gameObject.transform.globalPosition);

        if (oldCellCoord != newCellCoord)
        {
            // Remove from old cell
            if (grid.TryGetValue(oldCellCoord, out var oldCell))
            {
                oldCell.dynamicObjects.Remove(gameObject);
                oldCell.staticObjects.Remove(gameObject);
            }

            // Add to new cell
            if (!grid.TryGetValue(newCellCoord, out var newCell))
            {
                newCell = new GridCell();
                grid[newCellCoord] = newCell;
            }

            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics != null && physics.isMovable)
                newCell.dynamicObjects.Add(gameObject);
            else
                newCell.staticObjects.Add(gameObject);
        }
    }
    
}
