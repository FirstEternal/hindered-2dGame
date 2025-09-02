using Microsoft.Xna.Framework;
using System;

internal class TeleportObjectData : IComponentTypeData
{
    public string Type { get; set; }
    public Vector2? portalLocation { get; set; }
    public string portalSpriteName { get; set; }

    public static TeleportObject Deserialize(TeleportObjectData data)
    {
        // Basic validation
        if (data.portalSpriteName is null)
        {
            throw new ArgumentException("Invalid JSON data for TeleportObject.");
        }

        // Create the Terrain_Rectangle component
        TeleportObject teleportObject = new TeleportObject(
            portalLocation: data.portalLocation ?? Vector2.Zero,
            portalSpriteName: data.portalSpriteName
        );

        // Return the Terrain_Rectangle instance
        return teleportObject;
    }
}
