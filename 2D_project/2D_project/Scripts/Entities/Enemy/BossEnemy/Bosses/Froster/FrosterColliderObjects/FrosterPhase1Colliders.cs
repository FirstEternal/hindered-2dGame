using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class FrosterPhase1Colliders(GameObject parent, SpriteAnimated spriteAnimated) : PhaseColliderObject(parent, spriteAnimated)
{
    protected GameObject frame1GameColliderObject;
    protected GameObject frame2GameColliderObject;

    protected override void CreateColliders()
    {
        if (colliders.Count != 0) return;

        Create_Frame1ColliderObject(parent); // also adds collider to the list
        Create_Frame2ColliderObject(parent); // also adds collider to the list

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

        frame1GameColliderObject.SetActive(false);
        frame2GameColliderObject.SetActive(false);
    }

    protected virtual void Create_Frame1ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame1GameColliderObject = new GameObject();
        frame1GameColliderObject.CreateTransform();

        parent.AddChild(frame1GameColliderObject);

        float scaleX = parent.transform.globalScale.X;
        float scaleY = parent.transform.globalScale.Y;

        // 1.) Particle collider - head
        GameObject colliderObjectHead = new GameObject();
        colliderObjectHead.CreateTransform(localPosition: new Vector2(80, -115));
        Collider colliderHead = new ParticleCollider(
            radius: 40 * scaleX,
            isAftermath: true,
            isRelaxPosition: false
        );

        colliderObjectHead.AddComponent(colliderHead);

        // 2.) Convex collider - Left Wing
        GameObject colliderObjectLeftWing = new GameObject();
        colliderObjectLeftWing.CreateTransform();
        Collider colliderLeftWing = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(10, -130 /** scaleY*/),
                new Vector2(150 /** scaleX*/, 30 /** scaleY*/),
                new Vector2(45 /** scaleX*/, 160 /** scaleY*/),
            ]),
            isAftermath: true,
            isRelaxPosition: false
        );

        colliderObjectLeftWing.AddComponent(colliderLeftWing);

        // 3.) Convex collider - left wing
        GameObject colliderObjectLegs = new GameObject();
        colliderObjectLegs.CreateTransform();
        Collider colliderLegs = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(-15, -110 /** scaleY*/),
                new Vector2(10, -70 /** scaleY*/),
                new Vector2(-120 /** scaleX*/, 130 /** scaleY*/),
            ]),
            isAftermath: true,
            isRelaxPosition: false
        );

        colliderObjectLegs.AddComponent(colliderLegs);

        colliders.Add(colliderHead);
        colliders.Add(colliderLegs);
        colliders.Add(colliderLeftWing);

        frame1GameColliderObject.AddChild(colliderObjectHead);
        frame1GameColliderObject.AddChild(colliderObjectLegs);
        frame1GameColliderObject.AddChild(colliderObjectLeftWing);
    }

    protected virtual void Create_Frame2ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame2GameColliderObject = new GameObject();
        frame2GameColliderObject.CreateTransform();
        parent.AddChild(frame2GameColliderObject);

        float scaleX = parent.transform.globalScale.X;
        float scaleY = parent.transform.globalScale.Y;

        // 1.) Particle collider - Left Wing + head
        GameObject colliderObjectHead = new GameObject();
        colliderObjectHead.CreateTransform();
        Collider colliderHead = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(70 /** scaleX*/, -100 /** scaleY*/),
                new Vector2(160 /** scaleX*/, -55 /** scaleY*/),
                new Vector2(-100 /** scaleX*/, 105 /** scaleY*/),
            ]),
            isAftermath: true,
            isRelaxPosition: false
        );

        colliderObjectHead.AddComponent(colliderHead);

        // 2.) Convex collider - Right Wing
        GameObject colliderObjectRightWing = new GameObject();
        colliderObjectRightWing.CreateTransform();
        Collider colliderRightWing = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(-180, -170 /** scaleY*/),
                new Vector2(30 /** scaleX*/, -70 /** scaleY*/),
                new Vector2(0 /** scaleX*/, -30 /** scaleY*/),
            ]),
            isAftermath: true,
            isRelaxPosition: false
        );

        colliderObjectRightWing.AddComponent(colliderRightWing);

        // 3.) OBB collider - left wing
        GameObject colliderObjectLegs = new GameObject();
        colliderObjectLegs.CreateTransform(localPosition: new Vector2(55, 70));
        Collider colliderLegs = new OBBRectangleCollider(
            width: 40 * scaleX,
            height: 90 * scaleY,
            isAftermath: true,
            isRelaxPosition: false
        );

        colliderObjectLegs.AddComponent(colliderLegs);

        colliders.Add(colliderHead);
        colliders.Add(colliderLegs);
        colliders.Add(colliderRightWing);

        frame2GameColliderObject.AddChild(colliderObjectHead);
        frame2GameColliderObject.AddChild(colliderObjectLegs);
        frame2GameColliderObject.AddChild(colliderObjectRightWing);
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
            frame2GameColliderObject.SetActive(spriteAnimated.currFrameIndex != 0);
        }
    }
}
