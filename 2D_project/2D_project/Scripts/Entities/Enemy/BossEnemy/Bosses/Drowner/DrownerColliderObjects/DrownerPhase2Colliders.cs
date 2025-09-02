using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class DrownerPhase2Colliders(GameObject parent, SpriteAnimated spriteAnimated) : DrownerPhase1Colliders(parent, spriteAnimated)
{
    protected override void CreateColliders()
    {
        if (colliders.Count != 0) return;

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
        frame3GameColliderObject.SetActiveWithParentEnabled = false;
        frame2GameColliderObject.SetActiveWithParentEnabled = false;

        frame2GameColliderObject.SetActive(false);
        frame3GameColliderObject.SetActive(false);
    }

    protected override void Create_Frame2ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame2GameColliderObject = new GameObject();
        frame2GameColliderObject.CreateTransform();

        parent.AddChild(frame2GameColliderObject);

        float scaleX = 1; // parent.transform.globalScale.X;
        //float scaleY = 1; // parent.transform.globalScale.Y;

        // 1.) convex collider - Wings 
        GameObject colliderObjectWings = new GameObject();
        colliderObjectWings.CreateTransform();
        Collider colliderWings = new ConvexCollider(
            isScalable: false,
            bounds: new ConvexPolygon([
                new Vector2(-20, -160 /** scaleY*/),
                new Vector2(150 /** scaleX*/, -80 /** scaleY*/),
                new Vector2(0 /** scaleX*/, -40 /** scaleY*/),
                new Vector2(-150 /** scaleX*/, -120 /** scaleY*/),
            ]),
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectWings.AddComponent(colliderWings);

        // 2.) particle collider - body
        GameObject colliderObjectBody = new GameObject();
        colliderObjectBody.CreateTransform(localPosition: new Vector2(90, 0));

        Collider colliderBody = new ParticleCollider(
            radius: 55 * scaleX,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectBody.AddComponent(colliderBody);

        // 3.) Particle collider - Legs
        GameObject colliderObjectLegs = new GameObject();
        colliderObjectLegs.CreateTransform(localPosition: new Vector2(25, 65));
        Collider colliderLegs = new ParticleCollider(
            radius: 35 * scaleX,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLegs.AddComponent(colliderLegs);

        colliders.Add(colliderWings);
        colliders.Add(colliderLegs);
        colliders.Add(colliderBody);

        frame2GameColliderObject.AddChild(colliderObjectWings);
        frame2GameColliderObject.AddChild(colliderObjectLegs);
        frame2GameColliderObject.AddChild(colliderObjectBody);
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

            frame3GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 0); // frame 1 is basically frame 3 inherited
            frame2GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 1);
        }
    }
}