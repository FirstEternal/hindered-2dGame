using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

public abstract class ObjectComponent : IDisposable
{
    //public bool enabled { get; set; }
    public bool propagatedCollisionEnabled = true;

    public GameObject? gameObject;

    // Track whether Dispose has been called
    private bool _disposed = false;

    public virtual void Initialize() { }
    public virtual void LoadContent() { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void FixedUpdate(GameTime gameTime) { }

    public void ApplyActiveChange(object? sender, EventArgs e)
    {
        if (gameObject is null) return;
        Action action = gameObject.isActive ? OnEnable : OnDisable;
        action();
    }
    public virtual void OnDisable() { }

    public virtual void OnEnable() { }

    public virtual void OnDetectionRange(Collider collider) { }

    public virtual void OnCollisionEnter(Collider collider) { }

    // IDisposable implementation
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Suppress finalization to prevent it from being called by the garbage collector
    }

    // Dispose pattern to release both managed and unmanaged resources
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose of any managed resources here if needed (e.g., objects like textures, sound, etc.)
            // For example:
            // if (_texture != null)
            // {
            //     _texture.Dispose();
            //     _texture = null;
            // }
        }

        // Dispose of unmanaged resources here if any (currently none)

        _disposed = true;
    }

    // Finalizer (if needed for unmanaged resources)
    ~ObjectComponent()
    {
        Dispose(false); // Dispose of unmanaged resources
    }
}
