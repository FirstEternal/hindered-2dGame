using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class DrownerPhase1Colliders(GameObject parent, SpriteAnimated spriteAnimated) : PhaseColliderObject(parent, spriteAnimated)
{
    protected GameObject frame1GameColliderObject;
    protected GameObject frame2GameColliderObject;
    protected GameObject frame3GameColliderObject;
    protected override void CreateColliders()
    {
        if (colliders.Count != 0) return;

        Create_Frame1ColliderObject(parent); // also adds collider to the list
        Create_Frame2ColliderObject(parent); // also adds collider to the list
        Create_Frame3ColliderObject(parent); // also adds collider to the list

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

        frame1GameColliderObject.SetActive(false);
        frame2GameColliderObject.SetActive(false);
        frame3GameColliderObject.SetActive(false);
    }

    protected void Create_Frame1ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame1GameColliderObject = new GameObject();
        frame1GameColliderObject.CreateTransform();

        parent.AddChild(frame1GameColliderObject);

        float scaleX = 1; // parent.transform.globalScale.X;
        float scaleY = 1; // parent.transform.globalScale.Y;

        // 1.) convex collider - Right wing 
        GameObject colliderObjectRightWing = new GameObject();
        colliderObjectRightWing.CreateTransform();
        Collider colliderRightWing = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(0, -150 /** scaleY*/),
                new Vector2(115 /** scaleX*/, -55 /** scaleY*/),
                new Vector2(50 /** scaleX*/, 20 /** scaleY*/),
            ]),
            isAftermath: false,
            isRelaxPosition: false
        );


        colliderObjectRightWing.AddComponent(colliderRightWing);

        // 2.) convex collider - Left Wing
        GameObject colliderObjectLeftWing = new GameObject();
        colliderObjectLeftWing.CreateTransform();

        Collider colliderLeftWing = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(100 * scaleX, -25 * scaleY),
                new Vector2(145 * scaleX, 90 * scaleY),
                new Vector2(-45 * scaleX, 140 * scaleY),
            ]),
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLeftWing.AddComponent(colliderLeftWing);

        // 3.) convex collider - body
        GameObject colliderObjectLegs = new GameObject();
        colliderObjectLegs.CreateTransform();
        Collider colliderLegs = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(-130 * scaleX, -70 * scaleY),
                new Vector2(20 * scaleX, -40 * scaleY),
                new Vector2(40 * scaleX, 40 * scaleY),
            ]),
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLegs.AddComponent(colliderLegs);

        colliders.Add(colliderRightWing);
        colliders.Add(colliderLegs);
        colliders.Add(colliderLeftWing);

        frame1GameColliderObject.AddChild(colliderObjectRightWing);
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

        // 1.) convex collider - Wings 
        GameObject colliderObjectWings = new GameObject();
        colliderObjectWings.CreateTransform();
        Collider colliderWings = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(-50, -95 /** scaleY*/),
                new Vector2(50 /** scaleX*/, -70 /** scaleY*/),
                new Vector2(0 /** scaleX*/, 0 /** scaleY*/),
                new Vector2(-160 /** scaleX*/, -10 /** scaleY*/),
            ]),
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectWings.AddComponent(colliderWings);

        // 2.) particle collider - body
        GameObject colliderObjectBody = new GameObject();
        colliderObjectBody.CreateTransform(localPosition: new Vector2(90, 0));

        Collider colliderBody = new ParticleCollider(
            radius: 70 * scaleX,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectBody.AddComponent(colliderBody);

        // 3.) OBBRectangle collider - Left leg
        GameObject colliderObjectLeftLeg = new GameObject();
        colliderObjectLeftLeg.CreateTransform(localPosition: new Vector2(-5, 50));
        Collider colliderLeftLeg = new OBBRectangleCollider(
            width: 40 * scaleX,
            height: 90 * scaleY,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLeftLeg.AddComponent(colliderLeftLeg);

        colliders.Add(colliderWings);
        colliders.Add(colliderLeftLeg);
        colliders.Add(colliderBody);

        frame2GameColliderObject.AddChild(colliderObjectWings);
        frame2GameColliderObject.AddChild(colliderObjectLeftLeg);
        frame2GameColliderObject.AddChild(colliderObjectBody);
    }

    protected void Create_Frame3ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame3GameColliderObject = new GameObject();
        frame3GameColliderObject.CreateTransform();

        parent.AddChild(frame3GameColliderObject);

        float scaleX = parent.transform.globalScale.X;
        float scaleY = parent.transform.globalScale.Y;

        // 1.) convex collider - Right wing + head 
        GameObject colliderObjectWings = new GameObject();
        colliderObjectWings.CreateTransform();
        Collider colliderWings = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(30, -130 /** scaleY*/),
                new Vector2(250 /** scaleX*/, -90 /** scaleY*/),
                new Vector2(30, -50 /** scaleY*/),
                new Vector2(-250 /** scaleX*/, -90 /** scaleY*/),
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

        frame3GameColliderObject.AddChild(colliderObjectWings);
        frame3GameColliderObject.AddChild(colliderObjectLeftWing);
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
            frame3GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 2);
        }
    }
}
