using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class DrownerPhase3Colliders(GameObject parent, SpriteAnimated spriteAnimated) : DrownerPhase1Colliders(parent, spriteAnimated)
{
    protected GameObject frame4GameColliderObject;
    protected override void CreateColliders()
    {
        if (colliders.Count != 0) return;

        Create_Frame1ColliderObject(parent); // also adds collider to the list
        Create_Frame2ColliderObject(parent); // also adds collider to the list
        Create_Frame3ColliderObject(parent); // also adds collider to the list
        Create_Frame4ColliderObject(parent); // also adds collider to the list

        // --> add ignore tags
        foreach (Collider collider in colliders)
        {
            collider.gameObject.tag = GameConstantsAndValues.Tags.GravitationalEnemy.ToString();
            collider.AddTagsToIgnoreList([
                GameConstantsAndValues.Tags.Enemy.ToString(),
                GameConstantsAndValues.Tags.EnemySpawned.ToString(),
                GameConstantsAndValues.Tags.Hidden.ToString(),
                GameConstantsAndValues.Tags.Terrain.ToString()
            ]);
        }

        // make gameObject set active to manually
        frame1GameColliderObject.SetActiveWithParentEnabled = false;
        frame2GameColliderObject.SetActiveWithParentEnabled = false;
        frame3GameColliderObject.SetActiveWithParentEnabled = false;
        frame4GameColliderObject.SetActiveWithParentEnabled = false;

        frame1GameColliderObject.SetActive(false);
        frame2GameColliderObject.SetActive(false);
        frame3GameColliderObject.SetActive(false);
        frame4GameColliderObject.SetActive(false);
    }

    private void Create_Frame4ColliderObject(GameObject parent)
    {
        // very similar to frame3 just wings are a bit higher
        // create gameObject holding colliders
        frame4GameColliderObject = new GameObject();
        frame4GameColliderObject.CreateTransform();

        parent.AddChild(frame4GameColliderObject);

        float scaleX = parent.transform.globalScale.X;
        float scaleY = parent.transform.globalScale.Y;

        // 1.) convex collider - Right wing + head 
        GameObject colliderObjectWings = new GameObject();
        colliderObjectWings.CreateTransform();
        Collider colliderWings = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(250 /** scaleX*/, -140 /** scaleY*/),
                new Vector2(0, -60 /** scaleY*/),
                new Vector2(-250 /** scaleX*/, -140 /** scaleY*/),
            ]),
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectWings.AddComponent(colliderWings);

        // 2.) particle collider - legs
        GameObject colliderObjectLeftWing = new GameObject();
        colliderObjectLeftWing.CreateTransform(localPosition: new Vector2(5, 0));

        Collider colliderLeftWing = new ParticleCollider(
            radius: 50 * scaleX,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLeftWing.AddComponent(colliderLeftWing);


        colliders.Add(colliderWings);
        colliders.Add(colliderLeftWing);

        frame4GameColliderObject.AddChild(colliderObjectWings);
        frame4GameColliderObject.AddChild(colliderObjectLeftWing);
    }

    public override void UpdateColliders()
    {
        SpriteEffects newSpriteEffects = spriteAnimated.spriteEffects;
        int newFrameIndex = spriteAnimated.currFrameIndex;

        // 1.) check if sprite effects have changed
        if (currSpriteEffects != newSpriteEffects)
        {
            currSpriteEffects = newSpriteEffects;

            if (spriteAnimated.currFrameIndex == 0)
            {
                // update colliders -> reverse local position x (flip verticaly)
                foreach (Collider collider in colliders)
                {
                    collider.gameObject.transform.localPosition =
                        new Vector2(-collider.gameObject.transform.localPosition.X, collider.gameObject.transform.localPosition.Y);
                }
            }
        }

        // 2.) check if animation frame has changed (frames: 1 and frames: 2,3 have different colliders)
        if (currFrameIndex != newFrameIndex)
        {
            currFrameIndex = newFrameIndex;

            frame1GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 0);
            frame2GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 1);
            frame3GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 2 || spriteAnimated.currFrameIndex == 4);
            frame4GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 3);
        }
    }
}
