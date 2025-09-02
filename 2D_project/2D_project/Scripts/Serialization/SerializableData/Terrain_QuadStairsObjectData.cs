using Microsoft.Xna.Framework;
using System;
internal class Terrain_QuadStairsObjectData : IComponentTypeData, IPhysicsObjectData
{
    public string Type { get; set; }
    public bool[] CreateStairsIndices { get; set; }
    public string CornerTileName { get; set; }
    public string SideCornerTileName { get; set; }
    public string SideTileName { get; set; }
    public string MiddleTileName { get; set; }
    public int StairTilesInDirection { get; set; }
    public int TilesTillNextStairsWidth { get; set; }
    public int TilesTillNextStairsHeight { get; set; }
    public int StairTilesTillGrowth { get; set; }

    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }

    public static Terrain_QuadStairs Deserialize(Terrain_QuadStairsObjectData data)
    {
        // Basic validation
        if ((data.StairTilesTillGrowth == 0 && string.IsNullOrEmpty(data.CornerTileName)) ||
            string.IsNullOrEmpty(data.MiddleTileName) ||
            string.IsNullOrEmpty(data.SideTileName) ||
            (data.StairTilesTillGrowth > 0 && string.IsNullOrEmpty(data.SideCornerTileName)) ||
            data.StairTilesInDirection <= 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_Stairs.");
        }

        // Create the Terrain_Rectangle component
        Terrain_QuadStairs terrainQuadStairs = new Terrain_QuadStairs(
            createStairsIndices: data.CreateStairsIndices,
            cornerTileName: data.CornerTileName,
            sideCornerTileName: data.SideCornerTileName,
            sideTileName: data.SideTileName,
            middleTileName: data.MiddleTileName,

            stairTilesInDirection: data.StairTilesInDirection,
            stairTilesTillGrowth: data.StairTilesTillGrowth,
            tilesTillNextStairsWidth: data.TilesTillNextStairsWidth,
            tilesTillNextStairsHeight: data.TilesTillNextStairsHeight,
            mass: data.Mass,
            isMovable: data.IsMovable,
            isGravity: data.IsGravity,
            angularVelocity: data.AngularVelocity
        );

        // Return the Terrain_Rectangle instance
        return terrainQuadStairs;
    }
}
