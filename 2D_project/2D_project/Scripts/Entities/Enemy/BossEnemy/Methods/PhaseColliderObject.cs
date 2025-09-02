using MGEngine.Collision.Colliders;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

internal class PhaseColliderObject
{
    protected GameObject parent;
    protected SpriteAnimated spriteAnimated;
    protected SpriteEffects currSpriteEffects;
    protected int currFrameIndex;

    protected List<Collider> colliders = new List<Collider>();

    public PhaseColliderObject(GameObject parent, SpriteAnimated spriteAnimated)
    {
        this.parent = parent;
        this.spriteAnimated = spriteAnimated;

        // create colliders and gameObject that will hold them colliders
        CreateColliders();
    }
    protected virtual void CreateColliders() { }
    public virtual void UpdateColliders() { }
}