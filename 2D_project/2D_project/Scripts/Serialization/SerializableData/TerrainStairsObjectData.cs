using Microsoft.Xna.Framework;
using System;

internal class TerrainStairsObjectData : IComponentTypeData, IPhysicsObjectData
{
    public string Type { get; set; }
    public int TilesInDirection { get; set; }
    public string CornerTileName { get; set; }
    public string SideCornerTileName { get; set; }
    public string SideTileName { get; set; }
    public string MiddleTileName { get; set; }
    public bool IsHorizontallyFlipped { get; set; }
    public int TilesTillGrowth { get; set; }
    public float Mass { get; set; }
    public bool IsMovable { get; set; }
    public bool IsGravity { get; set; }
    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }

    public static Terrain_Stairs Deserialize(TerrainStairsObjectData data)
    {
        // Basic validation
        if ((data.TilesTillGrowth == 0 && string.IsNullOrEmpty(data.CornerTileName)) ||
            string.IsNullOrEmpty(data.MiddleTileName) ||
            string.IsNullOrEmpty(data.SideTileName) ||
            (data.TilesTillGrowth > 0 && string.IsNullOrEmpty(data.SideCornerTileName)) ||
            data.TilesInDirection <= 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_Stairs.");
        }
        // Create the Terrain_Stairs component
        Terrain_Stairs terrainStairs = new Terrain_Stairs(
            cornerTileName: data.CornerTileName,
            sideCornerTileName: data.SideCornerTileName,
            sideTileName: data.SideTileName,
            middleTileName: data.MiddleTileName,
            tilesInDirection: data.TilesInDirection,
            tilesTillGrowth: data.TilesTillGrowth,
            isHorizontallyFlipped: data.IsHorizontallyFlipped,
            mass: data.Mass,
            isMovable: data.IsMovable,
            isGravity: data.IsGravity,
            velocity: data.Velocity
        );

        return terrainStairs;
    }
    /*

    public static string SerializeTerrainStairs(Terrain_Stairs terrainStairs)
    {
        var data = new TerrainStairsObjectData
        {
            Type = "Terrain_Stairs",
            Tag = terrainStairs.gameObject.tag,
            ID = terrainStairs.gameObject.id,
            GlobalPosition = terrainStairs.gameObject.transform.globalPosition,
            LocalRotation = terrainStairs.gameObject.transform.localRotationAngle,
            LocalScale = terrainStairs.gameObject.transform.localScale,
            CornerTileName = terrainStairs.CornerTileName,
            SideCornerTileName = terrainStairs.SideCornerTileName,
            SideTileName = terrainStairs.SideTileName,
            MiddleTileName = terrainStairs.MiddleTileName,
            TilesInDirection = terrainStairs.TilesInDirection,
            TilesTillGrowth = terrainStairs.TilesTillGrowth,
            IsHorizontallyFlipped = terrainStairs.IsHorizontallyFlipped,
            Mass = terrainStairs.Mass,
            IsMovable = terrainStairs.isMovable,
            IsGravity = terrainStairs.isGravity,
            Velocity = terrainStairs.Velocity,
        };

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        
        try
        {
            string jsonFilePath = Path.Combine("..", "..", "..", "Scripts", "Terrain", "Serialization", "k.json");
            //Scripts\Terrain\Serialization
            // Write the JSON to a file
            File.WriteAllText(jsonFilePath, json);
            string absolutePath = Path.GetFullPath(jsonFilePath);

            Debug.WriteLine($"Absolute path: {absolutePath}");
        }
        catch
        {
            Debug.WriteLine("json error");
        }

        return json;
    }
    public static Terrain_Stairs DeserializeTerrainStairs(string json)
    {
        // Deserialize JSON into TerrainStairsObjectData
        TerrainStairsObjectData data = JsonConvert.DeserializeObject<TerrainStairsObjectData>(json);

        // Basic validation (optional but recommended)
        if (string.IsNullOrEmpty(data.CornerTileName) ||
            string.IsNullOrEmpty(data.MiddleTileName) ||
            string.IsNullOrEmpty(data.SideTileName) ||
            string.IsNullOrEmpty(data.SideCornerTileName) ||
            data.TilesInDirection <= 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_Stairs.");
        }

        // Create GameObject and set its transform
        GameObject gameObject = new GameObject(tag: data.Tag, id:data.ID);
        gameObject.CreateTransform(localRotationAngle: data.LocalRotation, localScale: data.LocalScale);
        gameObject.transform.globalPosition = data.GlobalPosition;

        // Create the Terrain_Stairs component
        Terrain_Stairs terrainStairs = new Terrain_Stairs(
            cornerTileName: data.CornerTileName,
            sideCornerTileName: data.SideCornerTileName,
            sideTileName: data.SideTileName,
            middleTileName: data.MiddleTileName,
            tilesInDirection: data.TilesInDirection,
            tilesTillGrowth: data.TilesTillGrowth,
            isHorizontallyFlipped: data.IsHorizontallyFlipped,
            mass: data.Mass,
            isMovable: data.IsMovable,
            isGravity: data.IsGravity,
            velocity: data.Velocity
        );

        // Attach the Terrain_Stairs component to the GameObject
        gameObject.AddComponent(terrainStairs);

        // Return the Terrain_Stairs instance
        return terrainStairs;
    }
    */
}


