using Microsoft.Xna.Framework;
using System;

internal class TerrainInvisibleColliderObjectData : IComponentTypeData, IPhysicsObjectData
{
    public string Type { get; set; }
    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }
    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }

    public static Terrain_InvisibleCollider Deserialize(TerrainInvisibleColliderObjectData data)
    {
        // Basic validation
        if (data.WidthInTiles == 0 || data.HeightInTiles == 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_InvisibleCollider.");
        }

        // Create the Terrain_Rectangle component
        Terrain_InvisibleCollider terrainInvisibleCollider = new Terrain_InvisibleCollider(
            widthInTiles: data.WidthInTiles,
            heightInTiles: data.HeightInTiles,
            mass: int.MaxValue
        );

        // Return the Terrain_Rectangle instance
        return terrainInvisibleCollider;
    }
}