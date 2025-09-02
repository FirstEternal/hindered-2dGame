using Microsoft.Xna.Framework;
using System;

internal class TerrainRectangleObjectData : IComponentTypeData, IPhysicsObjectData
{
    public string Type { get; set; }
    public int TilesInWidth { get; set; }
    public int TilesInHeight { get; set; }
    public bool CutBottom { get; set; }
    public string CornerTileName { get; set; }
    public string SideTileName { get; set; }
    public string MiddleTileName { get; set; }
    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }

    public static Terrain_Rectangle Deserialize(TerrainRectangleObjectData data)
    {
        // Basic validation
        if (string.IsNullOrEmpty(data.CornerTileName) ||
            string.IsNullOrEmpty(data.MiddleTileName) ||
            string.IsNullOrEmpty(data.SideTileName) ||
            data.TilesInWidth <= 0 || data.TilesInHeight <= 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_Rectanngle.");
        }

        // Create the Terrain_Rectangle component
        Terrain_Rectangle terrainRectangle = new Terrain_Rectangle(
            cornerTileName: data.CornerTileName,
            sideTileName: data.SideTileName,
            middleTileName: data.MiddleTileName,
            tilesInHeight: data.TilesInHeight,
            tilesInWidth: data.TilesInWidth,
            cutBottom: data.CutBottom,
            mass: data.Mass,
            isMovable: data.IsMovable,
            isGravity: data.IsGravity,
            velocity: data.Velocity
        );

        // Return the Terrain_Rectangle instance
        return terrainRectangle;
    }
}
