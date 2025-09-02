using Microsoft.Xna.Framework;
using System;

internal class TerrainSpikeLaneObjectData : IComponentTypeData, IPhysicsObjectData
{
    public string Type { get; set; }
    public string SpikeTileName { get; set; }
    public string CornerTileName { get; set; }
    public string SideTileName { get; set; }
    public string MiddleTileName { get; set; }
    public int TilesInDirection { get; set; }
    public int TilesBeforeSkip { get; set; }
    public int SkipTillNext { get; set; }
    public int TilesInHeight { get; set; }
    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }
    public bool AreSpikesSeparated { get; set; }

    public static Terrain_SpikeLane Deserialize(TerrainSpikeLaneObjectData data)
    {
        if (string.IsNullOrEmpty(data.SpikeTileName)
            || data.TilesInDirection <= 0
            || data.TilesInHeight > 0 && (data.MiddleTileName is null && data.SideTileName is null))
        {
            throw new ArgumentException("Invalid JSON data for Terrain_SpikeLane.");
        }

        Terrain_SpikeLane terrainSpikeLane = new Terrain_SpikeLane(
            areSpikesSeparated: data.AreSpikesSeparated,
            spikeTileName: data.SpikeTileName,
            middleTileName: data.MiddleTileName,
            cornerTileName: data.CornerTileName,
            sideTileName: data.SideTileName,
            tilesInDirection: data.TilesInDirection,
            TilesInHeight: data.TilesInHeight,
            tilesBeforeSkip: data.TilesBeforeSkip,
            skipTillNext: data.SkipTillNext,
            mass: data.Mass,
            isMovable: data.IsMovable,
            isGravity: data.IsGravity,
            velocity: data.Velocity,
            angularVelocity: data.AngularVelocity
        );

        return terrainSpikeLane;
    }
}