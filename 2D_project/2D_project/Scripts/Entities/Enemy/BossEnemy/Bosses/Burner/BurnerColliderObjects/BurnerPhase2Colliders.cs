using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

internal class BurnerPhase2Colliders(GameObject parent, SpriteAnimated spriteAnimated) : BurnerPhase1Colliders(parent, spriteAnimated)
{
    protected override void Create_Frame1ColliderObject(GameObject parent)
    {
        // create gameObject holding colliders
        frame1GameColliderObject = new GameObject();
        frame1GameColliderObject.CreateTransform();

        parent.AddChild(frame1GameColliderObject);

        // -> particle collider for frame 1

        // 1.) AARect collider - upper body
        GameObject colliderObjectBody = new GameObject();
        colliderObjectBody.CreateTransform(localPosition: new Vector2(0, -50));
        Collider colliderBody = new OBBRectangleCollider(
            width: 80 * parent.transform.globalScale.X,
            height: 180 * parent.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectBody.AddComponent(colliderBody);

        // 2.) AARect collider - Right Wing
        GameObject colliderObjectRightWing = new GameObject();
        colliderObjectRightWing.CreateTransform(localPosition: new Vector2(120, -100));
        Collider colliderRightWing = new OBBRectangleCollider(
            width: 150 * parent.transform.globalScale.X,
            height: 90 * parent.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectRightWing.AddComponent(colliderRightWing);

        // 3.) Particle collider - Left Wing
        GameObject colliderObjectLeftWing = new GameObject();
        colliderObjectLeftWing.CreateTransform(localPosition: new Vector2(-120, -100));
        Collider colliderLeftWing = new OBBRectangleCollider(
            width: 150 * parent.transform.globalScale.X,
            height: 90 * parent.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        colliderObjectLeftWing.AddComponent(colliderLeftWing);

        colliders.Add(colliderBody);
        colliders.Add(colliderRightWing);
        colliders.Add(colliderLeftWing);

        frame1GameColliderObject.AddChild(colliderObjectBody);
        frame1GameColliderObject.AddChild(colliderObjectRightWing);
        frame1GameColliderObject.AddChild(colliderObjectLeftWing);
    }

    // Create_Frame2ColliderObject is the same as inherited

    public override void UpdateColliders()
    {
        int newFrameIndex = spriteAnimated.currFrameIndex;

        // 2.) check if animation frame has changed (frames: 1, 2 and frames: 3 have different colliders)
        if (currFrameIndex != newFrameIndex)
        {
            currFrameIndex = newFrameIndex;

            frame1GameColliderObject.SetActive(spriteAnimated.currFrameIndex != 2); // is actually frame 1,2 here
            frame2GameColliderObject.SetActive(spriteAnimated.currFrameIndex == 2); // is actually frame 3 here
        }
    }
}