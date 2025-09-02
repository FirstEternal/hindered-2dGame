using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class GrasserPhase1Colliders(GameObject parent, SpriteAnimated spriteAnimated) : PhaseColliderObject(parent, spriteAnimated)
{
    protected GameObject frame1GameColliderObject;
    protected GameObject frame2GameColliderObject;

    protected override void CreateColliders()
    {
        if (colliders.Count != 0) return;

        Create_Frame1ColliderObject(parent); // also adds collider to the list
        //Create_Frame2ColliderObject(parent); // also adds collider to the list

        // --> add ignore tags
        foreach (Collider collider in colliders)
        {
            collider.gameObject.tag = GameConstantsAndValues.Tags.GravitationalEnemy.ToString();
            collider.AddTagsToIgnoreList([
                GameConstantsAndValues.Tags.Enemy.ToString(),
                GameConstantsAndValues.Tags.EnemySpawned.ToString(),
                GameConstantsAndValues.Tags.Hidden.ToString(),
            ]);
        }

        // make gameObject set active to manually
        frame1GameColliderObject.SetActiveWithParentEnabled = false;
        //frame2GameColliderObject.SetActiveWithParentEnabled = false;

        frame1GameColliderObject.SetActive(false);
        //frame2GameColliderObject.SetActive(false);
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
        colliderObjectHead.CreateTransform(localPosition: new Vector2(0, 0));
        Collider colliderHead = new OBBRectangleCollider(
            width: 300 * scaleX,
            height: 300 * scaleY,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectHead.AddComponent(colliderHead);
        colliders.Add(colliderHead);

        frame1GameColliderObject.AddChild(colliderObjectHead);
    }

    protected virtual void Create_Frame2ColliderObject(GameObject parent)
    {
        return;
    }

    public override void UpdateColliders()
    {
        frame1GameColliderObject.SetActive(true);

        /*
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
        }*/
    }
}
