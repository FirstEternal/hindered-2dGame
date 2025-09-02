using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class BurnerPhase1Colliders(GameObject parent, SpriteAnimated spriteAnimated) : PhaseColliderObject(parent, spriteAnimated)
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

        // -> particle collider for frame 1

        // 1.) AARect collider - upper body + right wing(active during frame 1)
        GameObject colliderObjectTop = new GameObject();
        colliderObjectTop.CreateTransform(localPosition: new Vector2(45, -80));
        Collider colliderTop = new OBBRectangleCollider(
            width: 180 * parent.transform.globalScale.X,
            height: 80 * parent.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectTop.AddComponent(colliderTop);

        // 2.) AARect collider - legs
        GameObject colliderObjectLegs = new GameObject();
        colliderObjectLegs.CreateTransform(localPosition: new Vector2(25, 35));
        Collider colliderLegs = new OBBRectangleCollider(
            width: 60 * parent.transform.globalScale.X,
            height: 140 * parent.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLegs.AddComponent(colliderLegs);

        // 3.) Particle collider - Left Wing
        GameObject colliderObjectLeftWing = new GameObject();
        colliderObjectLeftWing.CreateTransform(localPosition: new Vector2(-75, 17));
        Collider colliderLeftWing = new ParticleCollider(
            radius: 62 * parent.transform.globalScale.X,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLeftWing.AddComponent(colliderLeftWing);

        colliders.Add(colliderTop);
        colliders.Add(colliderLegs);
        colliders.Add(colliderLeftWing);

        frame1GameColliderObject.AddChild(colliderObjectTop);
        frame1GameColliderObject.AddChild(colliderObjectLegs);
        frame1GameColliderObject.AddChild(colliderObjectLeftWing);
    }

    protected virtual void Create_Frame2ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame2GameColliderObject = new GameObject();
        frame2GameColliderObject.CreateTransform(localPosition: new Vector2(-25, -15));
        parent.AddChild(frame2GameColliderObject);
        // -> particle collider for frames 2 & 3
        Collider collider = new ParticleCollider(
            radius: 90 * parent.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        frame2GameColliderObject.AddComponent(collider);

        colliders.Add(collider);
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
