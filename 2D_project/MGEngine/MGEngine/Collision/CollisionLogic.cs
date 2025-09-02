using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using static GridScene;

public static class CollisionLogic
{

    // NEW
    private static List<Collider> allColliders = new List<Collider>();
    private static Dictionary<string, List<Collider>> collidersByTag = new Dictionary<string, List<Collider>>();


    // Method to populate the collider list
    public static void PopulateColliders(List<GameObject> gameObjectList)
    {
        allColliders.Clear();

        // Clear the dictionary
        collidersByTag.Clear();

        // Add colliders from all game objects (including inactive ones)
        foreach (GameObject gameObject in gameObjectList)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                // Add collider to the allColliders list
                allColliders.Add(collider);
            }
        }
    }

    // Get colliders from nearby dynamic and static objects in grid cells within camera bounds
    public static List<Collider> GetNearbyColliders(List<GridCell> cellsToUpdate)
    {
        List<Collider> nearbyColliders = new List<Collider>();

        // Iterate through the nearby grid cells
        foreach (var cell in cellsToUpdate)
        {
            // Fetch colliders from dynamic (movable) objects
            foreach (var dynamicObj in cell.dynamicObjects)
            {
                if (dynamicObj.isActive)
                {
                    var collider = dynamicObj.GetComponent<Collider>();
                    if (collider != null)
                    {
                        nearbyColliders.Add(collider);
                    }
                }
            }

            // Fetch colliders from static objects
            foreach (var staticObj in cell.staticObjects)
            {
                if (staticObj.isActive)
                {
                    var collider = staticObj.GetComponent<Collider>();
                    if (collider != null)
                    {
                        nearbyColliders.Add(collider);
                    }
                }
            }
        }
        return nearbyColliders;
    }

    // Method to filter colliders based on ignored tags
    public static void CheckCollisionsBetweenColliders(List<Collider> queuedColliderList)
    {
        for (int i = 0; i < queuedColliderList.Count; i++)
        {
            Collider colliderA = queuedColliderList[i];
            if (colliderA.gameObject.isActive == false) continue;
            for (int j = i + 1; j < queuedColliderList.Count; j++)
            {
                Collider colliderB = queuedColliderList[j];
                if (colliderB.gameObject.isActive == false) continue;

                if (colliderA.gameObject.id == 33333 && colliderB.gameObject.tag == "Button")
                {
                    // ok                
                    Debug.WriteLine(colliderB.gameObject.tag + ": " + CollisionRules.CanCollide(colliderA.gameObject.tag, colliderB.gameObject.tag));
                }
                // Check if the tags of the two colliders can collide
                if (!CollisionRules.CanCollide(colliderA.gameObject.tag, colliderB.gameObject.tag))
                {
                    continue; // Skip this pair if they can't collide
                }

                // Proceed with the regular collision check if allowed
                CheckCollision(colliderA, colliderB);
            }
        }
    }
    // END NEW*/


    /// <summary>
    /// check for collision on all overlay gameObjects present in the scene
    /// </summary>
    public static void OverlaySceneCollision()
    {
        if (SceneManager.Instance.activeScene?.overlayGameObjects is not null) Collisions(SceneManager.Instance.activeScene.overlayGameObjects);
    }
    public static void SceneCollisions(List<Collider> queuedColliderList)
    {
        if (SceneManager.Instance.activeScene?.gameObjects is not null) CheckCollisionsBetweenColliders(queuedColliderList);
    }

    public static void SceneCollisions()
    {
        //if (SceneManager.Instance.activeScene?.overlayGameObjects is not null) Collisions(SceneManager.Instance.activeScene.gameObjects);
        if (SceneManager.Instance.activeScene?.gameObjects is not null) CheckCollisionsBetweenColliders(allColliders);
    }

    private static void Collisions(List<GameObject> gameObjectList)
    {
        // make a queue of all active objects with colliders
        List<Collider> collidersList = new List<Collider>();

        // find all active colliders within the active scene
        foreach (GameObject gameObject in gameObjectList)
        {
            if (!gameObject.isActive) continue;

            Collider collider = gameObject.GetComponent<Collider>();
            if (collider == null) continue;

            // object is active and has collider 
            collidersList.Add(collider);
        }

        // check for collisions between colliders
        for (int i = 0; i < collidersList.Count; i++)
        {
            Collider colliderA = collidersList[i];
            for (int j = i + 1; j < collidersList.Count; j++)
            {
                Collider colliderB = collidersList[j];
                CheckCollision(colliderA, colliderB);
            }
        }
    }

    /// <summary>
    /// check for collision on two colliders and apply collision logic
    /// </summary>
    private static void CheckCollision(Collider cA, Collider cB)
    {
        // object does not have colliders -> no collision to detect
        if (cA == null || cB == null) return; // should never happen, just a failsave for faster calculation

        // check if ignoring collision
        if (cA.ignoreTagList.Contains(cB.gameObject.tag) || cB.ignoreTagList.Contains(cA.gameObject.tag)) return;

        // check for collider type
        if (ParticleParticleCollision.Particle_ParticleColliderCollision(cA, cB)

           || ParticleHalfPlaneCollision.ParticleHalfPlaneColliderCollision(cA, cB)
           || ParticleHalfPlaneCollision.ParticleHalfPlaneColliderCollision(cB, cA)

           || ParticleAAHalfPlaneCollision.Particle_AAHalfPlaneColliderCollision(cA, cB)
           || ParticleAAHalfPlaneCollision.Particle_AAHalfPlaneColliderCollision(cB, cA)

           || ParticleConvexCollision.Particle_ConvexCollision(cA, cB)
           || ParticleConvexCollision.Particle_ConvexCollision(cB, cA)

           || ParticleAARectangleCollision.Particle_AARectangleColliderCollision(cA, cB)
           || ParticleAARectangleCollision.Particle_AARectangleColliderCollision(cB, cA)

           || AARectangleAAHalfPlaneCollision.AARectangle_AAHalfPlaneColliderCollision(cA, cB)
           || AARectangleAAHalfPlaneCollision.AARectangle_AAHalfPlaneColliderCollision(cB, cA)

           || AARectangleAARectangleCollision.AARectangle_AARectangleColliderCollision(cA, cB)

           || ConvexConvexCollision.Convex_ConvexCollision(cA, cB)

           || ParticleOBBRectangleCollision.Particle_OBBRectangleColliderCollision(cA, cB)
           || ParticleOBBRectangleCollision.Particle_OBBRectangleColliderCollision(cB, cA)

           || OBBRectangleConvexCollision.OBBRectangle_ConvexCollision(cA, cB)
           || OBBRectangleConvexCollision.OBBRectangle_ConvexCollision(cB, cA)
           //|| OBBRectangleOBBRectangleCollision.OBBRectangle_OBBRectangleColliderCollision(cA, cB)

           /*
           || OBBRectangleConvexCollision.OBBRectangle_ConvexCollision(cA, cB)
           || OBBRectangleConvexCollision.OBBRectangle_ConvexCollision(cB, cA)
           */
           //|| OBBRectangleOBBRectangleCollision.OBBRectangle_OBBRectangleColliderCollision(cA, cB)
           )
        {
            // there was collision
            ApplyCollision(cA, cB);
        }
    }

    /// <summary>
    /// apply collision logic on both colliders
    /// </summary>
    private static void ApplyCollision(Collider cA, Collider cB)
    {
        // call collision on both colliders
        cA.OnCollisionDetected(cB);
        cB.OnCollisionDetected(cA);
    }

    public static void RelaxCollision(PhysicsComponent physicsComponent1, bool relaxPC1, PhysicsComponent physicsComponent2, bool relaxPC2, Vector2 relaxDistance)
    {
        if (physicsComponent1 is null && physicsComponent2 is null) return;

        float relaxPercentage1 = 0.5f;
        float relaxPercentage2 = 0.5f;
        if (physicsComponent1 is not null && physicsComponent2 is not null)
        {
            float mass1 = physicsComponent1.Mass > 0 ? physicsComponent1.Mass : 1;
            float mass2 = physicsComponent2.Mass > 0 ? physicsComponent2.Mass : 1;
            relaxPercentage1 = mass2 / (mass1 + mass2);
            relaxPercentage2 = mass1 / (mass1 + mass2);
        }
        else if (physicsComponent1 is not null)
        {
            relaxPercentage1 = 1;
            relaxPercentage2 = 0;
        }
        else if (physicsComponent2 is not null)
        {
            relaxPercentage1 = 0;
            relaxPercentage2 = 1;
        }
        if (physicsComponent1 is not null && physicsComponent1.isMovable && relaxPC1) physicsComponent1.gameObject.transform.globalPosition -= relaxDistance * relaxPercentage1;
        if (physicsComponent2 is not null && physicsComponent2.isMovable && relaxPC2) physicsComponent2.gameObject.transform.globalPosition -= relaxDistance * relaxPercentage2;
    }

    /// <summary>
    /// RectangleCollider(rotated rectangle) - RectangleCollider(rotated rectangle) collision
    /// </summary>
    private static bool RectangleCollider_RectangleColliderCollision(Collider cA, Collider cB)
    {
        // TODO: -> apply collision check on GPU

        // rectangle vs rectangle collision logic
        if (cA is not RectangleCollider || cB is not RectangleCollider) return false;
        RectangleCollider rcA = (RectangleCollider)cA;
        RectangleCollider rcB = (RectangleCollider)cB;

        return RotatedRectangle.RotatedRectangleCollision(rcA.rotatedRectangle, rcB.rotatedRectangle);
    }
}
