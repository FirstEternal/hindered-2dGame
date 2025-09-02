using System;

internal class TerrainFadeOutObjectData : IComponentTypeData
{
    public string Type { get; set; }
    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }
    public static Terrain_FadeOut Deserialize(TerrainFadeOutObjectData data)
    {
        // Basic validation
        if (data.WidthInTiles == 0 || data.HeightInTiles == 0)
        {
            throw new ArgumentException("Invalid JSON data for Terrain_FadeOut.");
        }

        // Create the Terrain_Rectangle component
        Terrain_FadeOut terrainFadeOut = new Terrain_FadeOut(
            widthInTiles: data.WidthInTiles,
            heightInTiles: data.HeightInTiles
        );

        // Return the Terrain_Rectangle instance
        return terrainFadeOut;
    }
}