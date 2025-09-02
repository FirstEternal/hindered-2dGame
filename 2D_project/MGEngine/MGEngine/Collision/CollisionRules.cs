public static class CollisionRules
{
    // This will store the allowed collisions between tags
    private static Dictionary<string, HashSet<string>> collisionMap = new Dictionary<string, HashSet<string>>();

    // Initialize method that can be called from outside to populate collision rules
    public static void SetCollisionRules(Dictionary<string, HashSet<string>> externalCollisionMap)
    {
        // Clear any existing rules
        collisionMap.Clear();

        // Add the external collision map (new rules)
        foreach (var kvp in externalCollisionMap)
        {
            if (!collisionMap.ContainsKey(kvp.Key))
            {
                collisionMap[kvp.Key] = new HashSet<string>();
            }
            foreach (var tag in kvp.Value)
            {
                collisionMap[kvp.Key].Add(tag);
            }
        }
    }

    // This checks if two tags can collide based on the collision map
    public static bool CanCollide(string tagA, string tagB)
    {
        // Ensure the map contains the tag
        if (!collisionMap.ContainsKey(tagA)) return false;

        // Check if tagB is allowed to collide with tagA
        return collisionMap[tagA].Contains(tagB);
    }

    // Optional: Method to allow external code to dynamically add or update rules
    public static void AddCollisionRule(string tagA, string tagB)
    {
        if (!collisionMap.ContainsKey(tagA))
        {
            collisionMap[tagA] = new HashSet<string>();
        }
        collisionMap[tagA].Add(tagB);
    }

    // Optional: Method to allow external code to remove collision rule
    public static void RemoveCollisionRule(string tagA, string tagB)
    {
        if (collisionMap.ContainsKey(tagA))
        {
            collisionMap[tagA].Remove(tagB);
        }
    }

    // Optional: Method to get the entire collision map (for debugging or further external use)
    public static Dictionary<string, HashSet<string>> GetCollisionMap()
    {
        return new Dictionary<string, HashSet<string>>(collisionMap);
    }
}
