using Microsoft.Xna.Framework;

internal class MoveOnCollisionPlatformComponentData : IComponentTypeData
{
    public string Type { get; set; }
    public float MovementSpeed;
    public Vector2 StartTilePosition { get; set; }
    public Vector2 EndTilePosition { get; set; }
    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }

    public static MoveOnCollisionPlatformComponent Deserialize(MoveOnCollisionPlatformComponentData data)
    {
        MoveOnCollisionPlatformComponent moveOnCollisionComponent = new MoveOnCollisionPlatformComponent(
            StartTilePosition: data.StartTilePosition,
            EndTilePosition: data.EndTilePosition,
            WidthInTiles: data.WidthInTiles,
            HeightInTiles: data.HeightInTiles,
            MovementSpeed: data.MovementSpeed
        );

        return moveOnCollisionComponent;
    }
}