using Microsoft.Xna.Framework;
using System;

internal class TerrainButtonBoxData : IComponentTypeData, IPhysicsObjectData
{
    public string CornerTileName { get; set; }
    public string SideTileName { get; set; }
    public string MiddleTileName { get; set; }

    public string[] PressureTargetTags { get; set; }

    public int TilesInWidth { get; set; }
    public int TilesInHeight { get; set; }

    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }
    public string Type { get; set; }
    public int ResetInXSeconds { get; set; } = -1;

    public static Terrain_ButtonBox Deserialize(TerrainButtonBoxData data)
    {
        // Basic validation
        if ((data.TilesInWidth < 2 && data.TilesInHeight < 3)
            || (string.IsNullOrEmpty(data.CornerTileName))
            || string.IsNullOrEmpty(data.MiddleTileName)
            || string.IsNullOrEmpty(data.SideTileName)
            || data.PressureTargetTags.Length == 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_ButtonBox.");
        }
        // Create the Terrain_Rectangle component
        Terrain_ButtonBox terrainButtonBox = new Terrain_ButtonBox(
            cornerTileName: data.CornerTileName,
            sideTileName: data.SideTileName,
            middleTileName: data.MiddleTileName,
            tilesInHeight: data.TilesInHeight,
            tilesInWidth: data.TilesInWidth,
            pressureTargetTags: data.PressureTargetTags,
            mass: data.Mass,
            isMovable: data.IsMovable,
            isGravity: data.IsGravity,
            angularVelocity: data.AngularVelocity,
            resetInXSeconds: data.ResetInXSeconds == -1 ? int.MaxValue : data.ResetInXSeconds
        );

        // Return the Terrain_Rectangle instance
        return terrainButtonBox;
    }
}