using Microsoft.Xna.Framework;

namespace MGEngine.ObjectBased
{
    public class Transform
    {
        public GameObject? gameObject;

        // Location
        public Vector2 spawnPosition = Vector2.Zero; // global spawn position in world space

        private Vector2 _localPosition = Vector2.Zero;
        public Vector2 localPosition
        {
            get => _localPosition;
            set
            {
                _localPosition = value;
                UpdateTransform(); // Update the global transform when the local position changes
            }
        }

        public Vector2 globalPosition = Vector2.Zero;

        private float _localRotationAngle = 0;
        public float localRotationAngle
        {
            get => _localRotationAngle;
            set
            {
                _localRotationAngle = value;
                UpdateTransform(); // Update the global transform when the local rotation changes
            }
        }

        public float globalRotationAngle = 0;

        public Vector2 globalScale = Vector2.One;

        private Vector2 _localScale = Vector2.One;
        public Vector2 localScale
        {
            get => _localScale;
            set
            {
                _localScale = value;
                UpdateTransform(); // Update the global transform when the local scale changes
            }
        }

        // Constructor to initialize the transform with optional local values
        public Transform(GameObject gameObject, Vector2? localPosition = null, Vector2? localScale = null, float localRotationAngle = 0)
        {
            this.gameObject = gameObject;
            _localPosition = localPosition ?? Vector2.Zero;
            _localScale = localScale ?? Vector2.One;
            _localRotationAngle = localRotationAngle;

            UpdateTransform(); // Ensure the global transform is up-to-date at creation
        }

        // Reset global values to the local values, useful when resetting the object
        public void ResetGlobalValuesToLocal()
        {
            globalScale = localScale;
            globalPosition = localPosition;
            globalRotationAngle = localRotationAngle;
            spawnPosition = globalPosition; // Store the initial spawn position for future resets
        }

        // Update the global position, scale, and rotation based on the local values and parent transform
        public void UpdateTransform()
        {
            Transform? parentTransform = gameObject?.parent?.transform;

            // If the parent exists, apply its transform to calculate global transform
            Vector2 trueGlobalScale = parentTransform?.globalScale ?? Vector2.One;
            float trueGlobalRotation = parentTransform?.globalRotationAngle ?? 0;
            Vector2 trueGlobalPosition = parentTransform?.globalPosition ?? globalPosition;

            // Update global properties
            globalScale = trueGlobalScale * localScale;
            globalRotationAngle = trueGlobalRotation + localRotationAngle;
            globalPosition = trueGlobalPosition + GameWorldSpaceMath.RotateAndScalePoint(localPosition, trueGlobalRotation, trueGlobalScale);

            // Update the transforms of children after recalculating the global transform
            gameObject?.UpdateChildrenTransforms();
        }

        public virtual void Dispose()
        {
            // Clean up if necessary
            gameObject = null; // Nullify the reference to the GameObject
        }
    }
    /*
    internal class Transform
    {
        public GameObject gameObject;
        // Location
        public Vector2 spawnPosition = Vector2.Zero;        // global spawn position in world space

        private Vector2 _localPosition = Vector2.Zero;
        public Vector2 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
                UpdateTransform();
            }
        }


        public Vector2 globalPosition = Vector2.Zero;

        private float _localRotationAngle = 0;
        public float localRotationAngle
        {
            get
            {
                return _localRotationAngle;
            }
            set
            {
                _localRotationAngle = value;
                UpdateTransform();
            }
        }

        public float globalRotationAngle = 0;

        public Vector2 globalScale = Vector2.One;

        private Vector2 _localScale = Vector2.One;
        public Vector2 localScale
        {
            get
            {
                return _localScale;
            }
            set
            {
                _localScale = value;
                UpdateTransform();
            }
        }

        public Transform(GameObject gameObject, Vector2? localPosition = null, Vector2? localScale = null,float localRotationAngle = 0)
        {
            this.gameObject = gameObject;
            _localPosition = localPosition ?? Vector2.Zero;
            _localScale = localScale ?? Vector2.One;
            _localRotationAngle = localRotationAngle;
            UpdateTransform();
        }

        public void ResetGlobalValuesToLocal()
        {
            globalScale = localScale;
            globalPosition = localPosition;
            globalRotationAngle = localRotationAngle;
            spawnPosition = globalPosition;
        }

        public void UpdateTransform()
        {
            Transform parentTransform = gameObject.parent?.transform;
            
            Vector2 trueGlobalScale = (parentTransform is not null) ? parentTransform.globalScale : Vector2.One; 
            float trueGlobalRotation = (parentTransform is not null) ? parentTransform.globalRotationAngle : 0; 
            Vector2 trueGlobalPosition = (parentTransform is not null) ? parentTransform.globalPosition : globalPosition;
            globalScale = trueGlobalScale * localScale;
            globalRotationAngle = trueGlobalRotation + localRotationAngle;
            globalPosition = trueGlobalPosition + GameWorldSpaceMath.RotateAndScalePoint(localPosition, trueGlobalRotation, trueGlobalScale);
            gameObject.UpdateChildrenTransforms();
            gameObject.UpdateChildrenTransforms();
        }
    }
    */
}