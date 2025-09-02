using System;
using System.Collections.Generic;

internal class CollapseOnPlayerCollisionPlatformData : IComponentTypeData
{
    public string Type { get; set; }

    public List<string> CollisionTagIDs { get; set; }

    public float CollapseTime { get; set; }
    public float RebuildTime { get; set; }

    public static CollapseOnPlayerCollisionPlatform Deserialize(CollapseOnPlayerCollisionPlatformData data)
    {
        if (data.CollisionTagIDs is null || data.CollapseTime == 0 || data.RebuildTime == 0) throw new ArgumentException("Collapse timers are not correctly assigned.");

        CollapseOnPlayerCollisionPlatform collapseOnCollisionComponent = new CollapseOnPlayerCollisionPlatform(
            collapseTimer: data.CollapseTime,
            rebuildTimer: data.RebuildTime,
            collisionTagIDs: data.CollisionTagIDs
        );

        return collapseOnCollisionComponent;
    }
}