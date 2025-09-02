using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MGEngine.ObjectBased
{
    public class GameObject : IDisposable
    {
        // object id -> useful for debugging
        public int id;
        public string tag;

        private Scene? scene = null;

        public void SceneStatusChanged(Scene? scene, bool initiateRemoval)
        {
            this.scene = scene;
            // game object was added or removed from the scene
            Action<bool> childrenAction = initiateRemoval ? RemoveChildrenFromScene : AddChildrenToScene;
            // if the same scene, and initiateRemoval -> remove
            // if not the same scene, and !initiateRemoval -> add

            childrenAction?.Invoke(false);
            /*
            // iterate through children
            foreach (GameObject child in children)
            {
                child.SceneStatusChanged(Scene ? scene, bool initiateRemoval);
            }*/
            /*
            this.scene = scene;


            if (this.scene == scene) return;

            // scene changed
            this.scene = scene;

            // if null -> it was removed
            bool wasAdded = scene is not null; 

            // iterate through children
            foreach (GameObject child in children)
            {
                child.AssignScene(scene);
            }*/
        }

        public GameObject(int id = -1, string tag = "")
        {

            // add to list to be used at a later time
            GameObjectManager.allObjects.Add(this);
            if (id == -1)
            {
                // give it id from the list
                id = GameObjectManager.allObjects.Count;
            }
            this.id = id;
            //_tag = tag;
            this.tag = tag;
        }

        // is object active
        public bool isActive { get; private set; }
        public bool SetActiveWithParentEnabled = true;

        private EventHandler? onIsActiveChange; // event for changing isActive
        public virtual void SetActive(bool value)
        {
            if (isActive == value) return; // do nothing

            // update on set active 
            isActive = value;

            // iterate through children
            foreach (GameObject child in children)
            {
                // set child active if setting through parent enabled
                if (!value || child.SetActiveWithParentEnabled) child.SetActive(value);
            }

            onIsActiveChange?.Invoke(this, EventArgs.Empty);
        }

        // transform
        private Transform? transformValue; // just placeholder for transform getter
        public Transform? transform
        {
            get
            {
                return transformValue;

            }
            private set
            {
                transformValue = value;
            }
        }

        // child game objects
        public GameObject? parent;

        private List<GameObject> children = new List<GameObject>();
        public int childCount => children.Count;

        public GameObject GetChild(int index)
        {
            return children[index];
        }

        public void AddChild(GameObject gameObject, bool isOverlay = false)
        {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));
            if (gameObject == this) throw new InvalidOperationException("Cannot add self as a child.");
            if (IsDescendant(gameObject)) throw new InvalidOperationException("Cannot add a descendant as a child.");


            children.Add(gameObject);

            AddChildrenToScene(isOverlay);

            gameObject.parent = this;

            UpdateChildrenTransforms();

            if (!isActive) gameObject.SetActive(false);
        }

        private bool IsDescendant(GameObject potentialDescendant)
        {
            GameObject? current = this;
            while (current != null)
            {
                if (current == potentialDescendant) return true;
                current = current.parent;
            }
            return false;
        }


        public void RemoveChild(GameObject gameObject)
        {
            if (gameObject == null) return;

            children.Remove(gameObject);
            gameObject.parent = null;
            gameObject.transform?.ResetGlobalValuesToLocal();
        }

        // components

        private readonly List<IResettable> resettables = new();

        public void ResetAll()
        {
            foreach (var resettable in resettables)
                resettable.Reset();
        }

        private List<ObjectComponent> components = new List<ObjectComponent>();

        // MYBE OPTIMIZE SEARCHING
        //private Dictionary<Type, ObjectComponent> components = new Dictionary<Type, ObjectComponent>();
        public ObjectComponent AddComponent(ObjectComponent component)
        {
            if (component is Collider collider)
            {
                // subscribe collider to collision
                collider.onCollision -= OnCollision;
                collider.onCollision += OnCollision;
            }

            if (component is IResettable resettable)
                resettables.Add(resettable);

            components.Add(component); // add component
            component.gameObject = this;

            // subscribe component to on active change event
            onIsActiveChange -= component.ApplyActiveChange;
            onIsActiveChange += component.ApplyActiveChange;
            return component;
        }

        public T? GetComponent<T>() where T : ObjectComponent
        {
            foreach (ObjectComponent component in components)
            {
                // If the component is of type T, return it
                if (component is T)
                {
                    return (T)component;
                }
            }
            return null;
        }

        public ObjectComponent? GetComponent(Type componentType)
        {
            foreach (ObjectComponent component in components)
            {
                if (componentType.IsInstanceOfType(component))
                {
                    return component;
                }
            }
            return null;
        }


        // collision
        private void OnCollision(object? sender, EventArgs e)
        {
            if (e is not CollisionEventArgs collisionEventArgs) return;

            // get sprite collider
            Collider other = collisionEventArgs.collider;

            if (other == null) return;

            // reset collision applied

            // Assign the action based on whether it's an aftermath or not
            Action<Collider, bool> collisionAction = collisionEventArgs.IsAftermath ? OnCollisionEnter : OnDetectionRange;

            // Invoke the appropriate action
            collisionAction(other, false);
        }

        private HashSet<Collider> processedCollisions = new HashSet<Collider>();

        private void OnCollisionEnter(Collider other, bool parentCollisionApplied)
        {
            if (processedCollisions.Contains(other)) return; // Ignore duplicate collisions

            processedCollisions.Add(other); // Register the collision

            if (!parentCollisionApplied) parent?.OnCollisionEnter(other, false);

            // iterate through components
            foreach (ObjectComponent component in components)
            {
                if (parentCollisionApplied && parent is not null && !component.propagatedCollisionEnabled) continue;
                component.OnCollisionEnter(other);
            }

            // iterate through children
            foreach (GameObject child in children)
            {
                child.OnCollisionEnter(other, true);
            }
        }

        private void OnDetectionRange(Collider other, bool parentCollisionApplied)
        {
            if (processedCollisions.Contains(other)) return; // Ignore duplicate collisions

            processedCollisions.Add(other); // Register the collision

            if (!parentCollisionApplied)
            {
                parent?.OnDetectionRange(other, false);
            }

            // iterate through components
            foreach (ObjectComponent component in components)
            {
                if (parentCollisionApplied && parent is not null && !component.propagatedCollisionEnabled) continue;
                component.OnDetectionRange(other);
            }

            // iterate through children
            foreach (GameObject child in children)
            {
                child.OnDetectionRange(other, true);
            }
        }

        /*
        public void RemoveComponent<Type>() where Type: Component{
            if (component is IResettable resettable)
                resettables.Remove(components);
        }*/

        /*
        public Type AddComponent<Type>(GameObject gameObject) where Type : Component, new()
        {
            Type component = new Type();
            components.Add(component);
            return component;
        }*/

        public void Initialize()
        {
            // object is not active -> no updates to be made
            //if (!isActive) return;

            // TODO make it so that it wont change collection on sprite being added at the same time as sprite

            // iterate through components
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Initialize();
            }
        }

        public void LoadContent()
        {
            // object is not active -> no updates to be made
            //if (!isActive) return;

            // TODO make it so that it wont change collection on sprite being added at the same time as sprite

            // iterate through components
            for (int i = 0; i < components.Count; i++)
            {
                components[i].LoadContent();
            }

            /*
            foreach (ObjectComponent component in components)
            {
                component.LoadContent();
            }*/
        }

        public void Update(GameTime gameTime)
        {
            // object is not active -> no updates to be made
            if (!isActive) return;

            // iterate through components
            foreach (ObjectComponent component in components)
            {
                component.Update(gameTime);
            }

            if (willBeDisposed)
            {
                SelfDispose();
            }

            processedCollisions.Clear(); // clear collisions
        }

        public void FixedUpdate(GameTime gameTime)
        {
            // object is not active -> no updates to be made
            if (!isActive) return;

            // iterate through components
            foreach (ObjectComponent component in components)
            {
                component.FixedUpdate(gameTime);
            }
        }

        public void CreateTransform(Vector2? localPosition = null, Vector2? localScale = null, float localRotationAngle = 0)
        {
            transform = new Transform(this, localPosition, localScale, localRotationAngle);
            isActive = true;
        }

        public void UpdateChildrenTransforms()
        {
            if (transform is null) return;

            foreach (GameObject child in children)
            {
                child.transform?.UpdateTransform();
            }
            /*
            Transform parentTransform = parent?.transform;

            Debug.WriteLine("id: " + id);
            UpdateTransform(transform);
            
            //transform.globalScale = transform.localScale * ((parent is not null) ? parent.transform.globalScale : Vector2.One); 

            foreach (GameObject child in children)
            {
                Debug.WriteLine("has children");
                if (child.transform is null) continue;
                UpdateTransform(child.transform);
                child.UpdateGlobalTransformValues();
            }*/
        }
        public void AddChildrenToScene(bool isOverlay)
        {
            // MYBE HAVE ITS OWN SCENE
            //Scene? scene = SceneManager.Instance.GetGameObjectScene(this);

            if (scene is null) return;

            Action<GameObject> action = isOverlay ? child => { if (!scene.overlayGameObjects.Contains(child)) scene.AddGameObjectToScene(child, isOverlay: true); }
            : child => { if (!scene.gameObjects.Contains(child)) scene.AddGameObjectToScene(child, isOverlay: false); };

            foreach (GameObject child in children)
            {
                action(child);
                child.AddChildrenToScene(isOverlay);
            }
        }

        public void RemoveChildrenFromScene(bool isOverlay)
        {
            // MYBE HAVE ITS OWN SCENE
            //Scene? scene = SceneManager.Instance.GetGameObjectScene(this);

            if (scene is null) return;

            Action<GameObject> action = isOverlay ? child => { if (scene.overlayGameObjects.Contains(child)) scene.RemoveGameObjectFromScene(child, isOverlay: true); }
            : child => { if (scene.gameObjects.Contains(child)) scene.RemoveGameObjectFromScene(child, isOverlay: false); };

            foreach (GameObject child in children)
            {
                action(child);
                child.RemoveChildrenFromScene(isOverlay);
            }
        }

        private bool willBeDisposed = false;

        private void SelfDispose()
        {
            isActive = false;
            // Dispose of children recursively
            var childrenToDispose = new List<GameObject>(children); // Create a copy of the list to avoid modifying the original during iteration
            foreach (GameObject child in childrenToDispose)
            {
                child.Dispose(); // Recursively dispose each child game object
            }

            // Clear the children list to ensure no references are left
            children.Clear();

            // Dispose of components associated with this game object
            var componentsToDispose = new List<ObjectComponent>(components); // Create a copy of the list for safe iteration
            foreach (ObjectComponent component in componentsToDispose)
            {
                component.Dispose(); // Assuming that ObjectComponent implements IDisposable
            }

            // Clear the components list to ensure no references are left
            components.Clear();

            // Unsubscribe from the active change event
            onIsActiveChange = null;

            // If this object has a parent, remove it from the parent's children list
            if (parent != null)
            {
                parent.RemoveChild(this);
            }

            // Reset the transform if it's not null
            if (transform != null)
            {
                transform = null;
            }

            // Optionally perform additional cleanup if needed (e.g., clearing references to other managers, textures, etc.)
        }
        public void Dispose()
        {
            willBeDisposed = true;
        }
    }
}
