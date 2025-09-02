using Microsoft.Xna.Framework;
using System;
using static Terrain_Tile;

internal class Terrain_Rectangle(
    string cornerTileName, string sideTileName, string middleTileName,
    int tilesInWidth, int tilesInHeight, bool cutBottom = false,
    float mass = int.MaxValue, bool isMovable = false, bool isGravity = false, Vector2? velocity = null)
    : PhysicsComponent(mass, isMovable: isMovable, isGravity: isGravity, velocity: velocity), ITerrain, IResettable
{
    public int TilesInWidth { get; set; } = tilesInWidth;
    public int TilesInHeight { get; set; } = tilesInHeight;
    public bool CutBottom { get; set; } = cutBottom;

    public string CornerTileName { get; set; } = cornerTileName;
    public string SideTileName { get; set; } = sideTileName;
    public string MiddleTileName { get; set; } = middleTileName;
    public override void Initialize()
    {
        /*
        TileData MiddleTileName = SpriteSheetManager.GetTile(MiddleTileName);
        TileData SideTileName = SpriteSheetManager.GetTile(SideTileName);
        TileData CornerTileName = SpriteSheetManager.GetTile(CornerTileName);
        */
        // all tiles have the same size
        float tileWidth = GameConstantsAndValues.SQUARE_TILE_WIDTH;
        float tileHeight = GameConstantsAndValues.SQUARE_TILE_WIDTH;

        // spawn for each side : 1 x tile_3, spawn 1 scale 18 x_2
        float tile_corner_width = tileWidth;
        float tile_side_width = tileWidth * TilesInWidth - 2 * tile_corner_width;
        float tile_corner_height = tileHeight;
        float tile_side_height = tileHeight * TilesInHeight - 2 * tile_corner_height;

        //
        float rot90 = MathF.PI / 2.0f;
        float rot180 = MathF.PI;
        float rot270 = -MathF.PI / 2.0f;

        // corners TL -> TR -> BL -> BR
        float actualWidth = TilesInWidth * tileWidth;
        float actualHeight = TilesInHeight * tileHeight;
        // Tl
        CreateTile(this, CornerTileName, new Vector2(-(tile_corner_width + tile_side_width) / 2, -(tile_corner_height + tile_side_height) / 2.0f), localScale: Vector2.One, localRotation: rot180, shouldSpawnCollider: true, isHorizontallyFlipped: true, topSnapEnabled: true);
        // TR
        CreateTile(this, CornerTileName, new Vector2((tile_corner_width + tile_side_width) / 2.0f, -(tile_corner_height + tile_side_height) / 2.0f), localScale: Vector2.One, localRotation: rot180, shouldSpawnCollider: true, topSnapEnabled: true);

        // TC
        if (tile_side_width > 0) CreateTile(this, SideTileName, new Vector2(0, -(tile_corner_height + tile_side_height) / 2.0f), localScale: new Vector2(TilesInWidth - 2, 1), localRotation: rot180, shouldSpawnCollider: true, topSnapEnabled: true);

        // CL
        if (tile_side_height > 0 && TilesInHeight > 1) CreateTile(this, SideTileName, new Vector2(-(tile_corner_width + tile_side_width) / 2.0f, 0), localScale: new Vector2(TilesInHeight - 2, 1), localRotation: rot90, shouldSpawnCollider: true, leftSnapEnabled: true);

        // CR
        if (tile_side_height > 0 && TilesInHeight > 1) CreateTile(this, SideTileName, new Vector2((tile_corner_width + tile_side_width) / 2.0f, 0), localScale: new Vector2(TilesInHeight - 2, 1), localRotation: rot270, shouldSpawnCollider: true, rightSnapEnabled: true);
        if (TilesInHeight > 1)
        {
            if (!CutBottom)
            {
                // BL
                CreateTile(this, CornerTileName, new Vector2(-(tile_corner_width + tile_side_width) / 2.0f, +(tile_corner_height + tile_side_height) / 2.0f), localScale: Vector2.One, localRotation: 0, shouldSpawnCollider: true, leftSnapEnabled: true);
                // BR
                CreateTile(this, CornerTileName, new Vector2((tile_corner_width + tile_side_width) / 2.0f, +(tile_corner_height + tile_side_height) / 2.0f), localScale: Vector2.One, localRotation: 0, shouldSpawnCollider: true, isHorizontallyFlipped: true, rightSnapEnabled: true);
                // BC
                if (tile_side_width > 0) CreateTile(this, SideTileName, new Vector2(0, +(tile_corner_height + tile_side_height) / 2.0f), localScale: new Vector2(TilesInWidth - 2, 1), localRotation: 0, shouldSpawnCollider: true, bottomSnapEnabled: true);
            }
            else
            {
                // replace bottom corners with side tiles
                // BL
                CreateTile(this, SideTileName, new Vector2(-(tile_corner_width + tile_side_width) / 2.0f, +(tile_corner_height + tile_side_height) / 2.0f), localScale: Vector2.One, localRotation: rot90, shouldSpawnCollider: true, leftSnapEnabled: true);
                // BR
                CreateTile(this, SideTileName, new Vector2((tile_corner_width + tile_side_width) / 2.0f, +(tile_corner_height + tile_side_height) / 2.0f), localScale: Vector2.One, localRotation: rot270, shouldSpawnCollider: true, isHorizontallyFlipped: true, rightSnapEnabled: true);

                // replace bottom layer with empty
                CreateTile(this, MiddleTileName, new Vector2(0, +(tile_corner_height + tile_side_height) / 2.0f), localScale: new Vector2(TilesInWidth - 2, 1), localRotation: 0, shouldSpawnCollider: false, bottomSnapEnabled: true);
            }
        }
        // Fill the middle

        Vector2 fillScale = new Vector2(TilesInWidth - 2, TilesInHeight - 2);
        //if (tile_side_width > 0 && tile_side_height > 0) CreateTile(tileData_middle, new Vector2(fillScale.X * GameConstantsAndValues.TILE_WIDTH/2f, fillScale.Y * GameConstantsAndValues.TILE_HEIGHT/2f), scale: fillScale, rotation: 0, scene: scene);
        if (tile_side_width > 0 && tile_side_height > 0) CreateTile(this, MiddleTileName, new Vector2(0, 0), localScale: fillScale, localRotation: 0);


        originalIsActive = gameObject.isActive;
    }

    private bool originalIsActive;
    public void Reset()
    {
        gameObject.SetActive(originalIsActive);
    }
}
