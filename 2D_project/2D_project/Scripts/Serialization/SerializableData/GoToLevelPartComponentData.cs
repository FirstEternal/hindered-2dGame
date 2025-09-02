internal class GoToLevelPartComponentData : IComponentTypeData
{
    public string Type { get; set; }
    public int LevelPartIndex { get; set; }

    public static GoToLevelPartComponent Deserialize(GoToLevelPartComponentData data)
    {
        // Create the Terrain_Rectangle component
        GoToLevelPartComponent goToLevelPartComponent = new GoToLevelPartComponent(
            levelPartIndex: data.LevelPartIndex
        );

        // Return the Terrain_Rectangle instance
        return goToLevelPartComponent;
    }
}