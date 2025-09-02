using Microsoft.Xna.Framework;
using System;
using static Terrain_Tile;

internal class Terrain_Stairs(
    string cornerTileName, string sideCornerTileName, string sideTileName, string middleTileName,
    int tilesInDirection, int tilesTillGrowth = 0, bool isHorizontallyFlipped = false,
    float mass = int.MaxValue, bool isMovable = false, bool isGravity = false, Vector2? velocity = null, bool spawnOnlyStairs = false)
    : PhysicsComponent(mass, isMovable: isMovable, isGravity: isGravity, velocity: velocity), ITerrain, IResettable
{
    private bool spawnOnlyStairs = spawnOnlyStairs;
    public int TilesInDirection { get; set; } = tilesInDirection;
    public string CornerTileName { get; set; } = cornerTileName;
    public string SideCornerTileName { get; set; } = sideCornerTileName;
    public string SideTileName { get; set; } = sideTileName;
    public string MiddleTileName { get; set; } = middleTileName;
    public bool IsHorizontallyFlipped { get; set; } = isHorizontallyFlipped;
    public int TilesTillGrowth { get; set; } = tilesTillGrowth;

    public override void Initialize()
    {
        // CAN CHANGE LATER
        string tileData_corner = TilesTillGrowth == 0 ? CornerTileName : SideCornerTileName;
        string tileData_fill = MiddleTileName;
        string tileData_side = SideTileName;

        // all tiles have the same size
        float tileWidth = GameConstantsAndValues.SQUARE_TILE_WIDTH;
        float tileHeight = GameConstantsAndValues.SQUARE_TILE_WIDTH;

        Vector2 vertical = new Vector2(0, 1);
        Vector2 horizontal = new Vector2(1, 0);

        // spawn for each side : 1 x tile_3, spawn 1 scale 18 x_2

        //
        //float rot90 = MathF.PI / 2;
        float rot180 = MathF.PI;
        float rot270 = -MathF.PI / 2;

        // corners TL -> TR -> BL -> BR
        float actualWidth = TilesInDirection * tileWidth;
        float actualHeight = actualWidth;
        int prevSize = 0;

        for (int i = 0; i < TilesInDirection; i++)
        {
            int axisDirection = IsHorizontallyFlipped ? -1 : 1;
            // 1. grow corner (stair case)
            float posX = axisDirection * (actualWidth / 2f - tileWidth / 2f - i * tileWidth);
            float posY = actualHeight / 2.0f - tileHeight / 2.0f - i * tileHeight;
            CreateTile(this, tileData_corner, new Vector2(posX, posY), localScale: Vector2.One, localRotation: rot180,
                shouldSpawnCollider: true, isHorizontallyFlipped: IsHorizontallyFlipped,
                topSnapEnabled: true,
                isStairCase: true);

            if (spawnOnlyStairs) continue;
            //if (i == 0) continue;
            // 2. fill under tile
            int size = i - prevSize;

            if (TilesTillGrowth == 0) prevSize++;

            int actualTilesGrowth = TilesTillGrowth;
            if (TilesTillGrowth + i > TilesInDirection - 1)
            {
                actualTilesGrowth = TilesInDirection - 1 - i;
            }
            // fill bottom left

            // follows x-axis direction
            if (i > 0) CreateTile(this, tileData_fill, localPosition: new Vector2(posX - axisDirection * ((actualTilesGrowth + 2) / 2.0f * tileWidth - tileWidth), -prevSize / 2.0f * tileHeight + actualHeight / 2.0f), localScale: new Vector2(actualTilesGrowth + 1, prevSize), localRotation: rot180, isHorizontallyFlipped: IsHorizontallyFlipped);
            prevSize = i;

            // upper       
            // 3. fill the rest
            if (TilesTillGrowth == 0) continue;

            if (i > 0)
            {
                CreateTile(this, tileData_side, new Vector2(posX, posY + tileHeight / 2.0f + (size / 2.0f) * tileHeight), localScale: new Vector2(size, 1), localRotation: axisDirection * rot270, isHorizontallyFlipped: IsHorizontallyFlipped);

                if (actualTilesGrowth > 0)
                {
                    // FIX THIS ONE -> ROTATION WHEN FLIPPED
                    // connect stairs with side tiles
                    CreateTile(this, tileData_side, new Vector2(posX - axisDirection * ((actualTilesGrowth + 1) / 2.0f * tileWidth), posY + tileHeight), localScale: new Vector2(actualTilesGrowth, 1), localRotation: rot180, isHorizontallyFlipped: IsHorizontallyFlipped);
                    // fill under the connection
                    CreateTile(this, tileData_fill, new Vector2(posX - axisDirection * ((actualTilesGrowth + 1) / 2.0f * tileWidth), posY + 3 * tileHeight / 2.0f + (size - 1) / 2.0f * tileHeight), localScale: new Vector2(actualTilesGrowth, size - 1), localRotation: rot180, isHorizontallyFlipped: IsHorizontallyFlipped);
                }
            }

            i += TilesTillGrowth; // skip to the next growth tile
        }

        base.LoadContent();
        originalIsActive = gameObject.isActive;
    }

    private bool originalIsActive;
    public void Reset()
    {
        gameObject.SetActive(originalIsActive);
    }
}