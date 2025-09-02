using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
/// <summary>
/// stair indices follow clockwise: TopRight->BottomRight->BottomLeft->TopLeft
/// </summary>
internal class Terrain_QuadStairs(
    bool[] createStairsIndices,
    string cornerTileName, string sideCornerTileName, string sideTileName, string middleTileName,
    int stairTilesInDirection = 1, int tilesTillNextStairsWidth = 0, int tilesTillNextStairsHeight = 0, int stairTilesTillGrowth = 0,
    float mass = int.MaxValue, bool isMovable = false, bool isGravity = false, float angularVelocity = 0)
    : PhysicsComponent(mass, isMovable: isMovable, isGravity: isGravity, angularVelocity: angularVelocity), ITerrain, IResettable
{
    //string CornerTileName = cornerTileName;
    private Terrain_Stairs[] stairsArray = [null, null, null, null];
    public float length;
    public float height;

    private bool originalIsActive;
    public void Reset()
    {
        // maybe TODO?
        gameObject.SetActive(originalIsActive);
    }
    public enum StairIndex
    {
        TopRight,
        BottomRight,
        BottomLeft,
        TopLeft
    }
    public override void Initialize()
    {
        if (createStairsIndices is [false, false, false, false])
        {
            throw new Exception("at least one index should be set to true");
        }
        float separatorDistanceHeight = GameConstantsAndValues.SQUARE_TILE_WIDTH * tilesTillNextStairsHeight / 2.0f;
        float separatorDistanceWidth = GameConstantsAndValues.SQUARE_TILE_WIDTH * tilesTillNextStairsWidth / 2.0f;
        length = separatorDistanceHeight + 2 * stairTilesInDirection;
        height = separatorDistanceWidth + 2 * stairTilesInDirection;
        float stairsPos = GameConstantsAndValues.SQUARE_TILE_WIDTH * stairTilesInDirection / 2.0f;
        float separatorPosHeight = separatorDistanceHeight + stairsPos;
        float separatorPosWidth = separatorDistanceWidth + stairsPos;
        //tilesTillNextStairs
        Vector2 topRightPos = new Vector2(separatorPosWidth, -separatorPosHeight);
        Vector2 bottomRightPos = new Vector2(separatorPosWidth, separatorPosHeight);
        Vector2 bottomLeftPos = new Vector2(-separatorPosWidth, separatorPosHeight);
        Vector2 topLeftPos = new Vector2(-separatorPosWidth, -separatorPosHeight);

        CreateStair(StairIndex.TopRight, localPosition: topRightPos, localRotationAngle: 0, isHorizontallyFlipped: false);
        CreateStair(StairIndex.BottomRight, localPosition: bottomRightPos, localRotationAngle: MathF.PI, isHorizontallyFlipped: true);
        CreateStair(StairIndex.BottomLeft, localPosition: bottomLeftPos, localRotationAngle: MathF.PI, isHorizontallyFlipped: false);
        CreateStair(StairIndex.TopLeft, localPosition: topLeftPos, localRotationAngle: 0, isHorizontallyFlipped: true);

        base.Initialize();

        originalIsActive = gameObject.isActive;
    }

    private void CreateStair(StairIndex stairIndex, Vector2 localPosition, float localRotationAngle, bool isHorizontallyFlipped)
    {
        int index = (int)stairIndex;
        if (!createStairsIndices[index]) return;

        GameObject stairsObject = new GameObject(tag: "Terrain");
        stairsObject.CreateTransform(localPosition: localPosition, localRotationAngle: localRotationAngle);
        stairsArray[index] = new Terrain_Stairs(
            cornerTileName: cornerTileName,
            sideCornerTileName: sideCornerTileName,
            sideTileName: sideTileName,
            middleTileName: middleTileName,
            tilesInDirection: stairTilesInDirection,
            tilesTillGrowth: stairTilesTillGrowth,
            isHorizontallyFlipped: isHorizontallyFlipped,
            spawnOnlyStairs: true);
        stairsObject.AddComponent(stairsArray[index]);

        gameObject.AddChild(stairsObject);
    }

    public void EnableStairs(StairIndex stairIndex, bool enable)
    {
        Terrain_Stairs stairs = stairsArray[(int)stairIndex];
        Debug.WriteLineIf(stairs == null, $"stair at {stairIndex.ToString()} does not exist");
        stairs?.gameObject.SetActive(enable);
    }
}
