using GamePlatformer;
using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

internal class CollapseOnPlayerCollisionPlatform : ObjectComponent, IResettable
{
    Timer rebuildTimer;
    public List<string> CollisionTagIDs;
    public CollapseOnPlayerCollisionPlatform(float collapseTimer, float rebuildTimer, List<string> collisionTagIDs)
    {
        maxCollapseTimer = collapseTimer;

        this.rebuildTimer = new Timer(Game2DPlatformer.Instance, countdownTime: rebuildTimer);
        this.rebuildTimer.OnCountdownEnd += Rebuild;

        this.CollisionTagIDs = collisionTagIDs;
        offset = 4;
    }
    private bool isCollapsing;
    public event EventHandler OnCollapse;

    private float currCollapseTimer;
    private readonly float maxCollapseTimer;

    private int offset;
    private bool offsetFlagged;

    private Vector2 rebuildPosition;
    private bool wasOriginalyMovable;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (isCollapsing)
        {
            currCollapseTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currCollapseTimer < 0)
            {
                // collapse
                OnCollapse?.Invoke(this, EventArgs.Empty);
                gameObject.SetActive(false);
                rebuildTimer.BeginTimer();
            }
        }
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);
        if (isCollapsing)
        {
            gameObject.transform.globalPosition.Y += offset * (offsetFlagged ? -1 : 1);
            gameObject.transform.globalPosition.X += offset * (offsetFlagged ? 1 : -1);

            offsetFlagged = !offsetFlagged;
        }
    }
    private void Rebuild(Timer timer)
    {
        isCollapsing = false;
        gameObject.transform.globalPosition = rebuildPosition;
        gameObject.SetActive(true);
        if (!wasOriginalyMovable) gameObject.GetComponent<PhysicsComponent>().isMovable = false;
    }

    public override void OnCollisionEnter(Collider collider)
    {
        base.OnCollisionEnter(collider);
        OnCollision(collider);
    }

    public override void OnDetectionRange(Collider collider)
    {
        base.OnDetectionRange(collider);
        OnCollision(collider);
    }
    protected virtual void OnCollision(Collider collider)
    {
        if (isCollapsing) return;

        if (CollisionTagIDs.Contains(collider.gameObject.tag))
        {
            // begin collapse
            isCollapsing = true;
            currCollapseTimer = maxCollapseTimer;
            offsetFlagged = false;
            rebuildPosition = gameObject.transform.globalPosition;
            wasOriginalyMovable = gameObject.GetComponent<PhysicsComponent>().isMovable;
            gameObject.GetComponent<PhysicsComponent>().isMovable = true;
        }
    }

    public void Reset()
    {
        isCollapsing = false;
        gameObject.transform.globalPosition = rebuildPosition;
        gameObject.SetActive(true);
        if (!wasOriginalyMovable) gameObject.GetComponent<PhysicsComponent>().isMovable = false;
    }
}
