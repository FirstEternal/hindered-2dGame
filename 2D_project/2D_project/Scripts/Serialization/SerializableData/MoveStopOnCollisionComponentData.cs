using Microsoft.Xna.Framework;
using System;

internal class MoveStopOnCollisionComponentData : IComponentTypeData
{
    public string Type { get; set; }
    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }

    public string[] CollisionTagsToStart { get; set; }
    public string[] CollisionTagsToStop { get; set; }
    public bool AdjustToTileSystem { get; set; }

    public bool HideOnStopEnabled { get; set; }

    public static MoveStopOnCollisionComponent Deserialize(MoveStopOnCollisionComponentData data)
    {
        // Basic validation
        if (data.CollisionTagsToStart.Length == 0 && data.CollisionTagsToStop.Length == 0)
        {
            throw new ArgumentException("Invalid JSON data for MoveOnCollision component.");
        }

        MoveStopOnCollisionComponent moveOnCollisionComponent = new MoveStopOnCollisionComponent(
            collisionTagsToStart: data.CollisionTagsToStart,
            collisionTagsToStop: data.CollisionTagsToStop,
            velocity: data.Velocity,
            mass: data.Mass,
            isGravity: data.IsGravity,
            isMovable: data.IsMovable,
            adjustToTileSystem: data.AdjustToTileSystem,
            hideOnStopEnabled: data.HideOnStopEnabled
        );

        return moveOnCollisionComponent;
    }
}