
namespace MGEngine.Collision.Colliders
{
    public abstract class Collider(bool isAftermath, bool isRelaxPosition = true, bool isEnergyExchange = false) : ObjectComponent
    {
        // collidingComponent -> for example: sprite, 3d object ...
        // use case: Sprite sprite = (Sprite) collidingComponent

        public List<string> ignoreTagList = new List<string>();

        public void AddTagsToIgnoreList(List<String> tags)
        {
            foreach (var tag in tags)
            {
                ignoreTagList.Add(tag);
            }
        }

        public bool isEnergyExchange { get; private set; } = isEnergyExchange; // true: exchange energy on collision, false: do nothing
        public bool isRelaxPosition { get; private set; } = isRelaxPosition; // true: relax position on collision, false: keep the position
        public bool isAftermath { get; private set; } = isAftermath; // true: collision aftermath, false: collision detection only

        public EventHandler? onCollision;

        public void OnCollisionDetected(Collider other)
        {
            onCollision?.Invoke(this, new CollisionEventArgs(collider: other, isAftermath: isAftermath));
        }

        public override void Dispose()
        {
            // Cleanup any resources like unsubscribing from events
            onCollision = null; // Example: If there’s an event handler, clean it up

            // If there are other resources, release them here
        }
    }

    public class CollisionEventArgs(Collider collider, bool isAftermath) : EventArgs
    {
        internal Collider collider { get; } = collider;
        internal bool IsAftermath { get; } = isAftermath;
    }
}
