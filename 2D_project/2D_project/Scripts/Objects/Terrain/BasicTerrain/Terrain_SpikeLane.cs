using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;

internal class Terrain_SpikeLane(
    string spikeTileName, string middleTileName, int tilesInDirection, bool areSpikesSeparated, int tilesBeforeSkip = 1, int skipTillNext = 0,
    float mass = int.MaxValue, bool isMovable = false, bool isGravity = false, Vector2? velocity = null, float angularVelocity = 0, int TilesInHeight = 0, string sideTileName = null,
    string cornerTileName = null)
    : PhysicsComponent(mass, isMovable: isMovable, isGravity: isGravity, velocity: velocity, angularVelocity: angularVelocity), ITerrain, IResettable
{

    readonly bool areSpikesSeparated = areSpikesSeparated;
    readonly int tilesInHeight = TilesInHeight;
    readonly int skipTillNext = skipTillNext;
    readonly int tilesBeforeSkip = tilesBeforeSkip;
    readonly int tilesInDirection = tilesInDirection;
    readonly string spikeTileName = spikeTileName;
    readonly string cornerTileName = cornerTileName;
    readonly string middleTileName = middleTileName;
    readonly string sideTileName = sideTileName;

    public override void Initialize()
    {
        // CAN CHANGE LATER
        string tileData_spike = spikeTileName;

        // all tiles have the same size
        float tileWidth = GameConstantsAndValues.SQUARE_TILE_WIDTH;
        float tileHeight = GameConstantsAndValues.SQUARE_TILE_WIDTH;

        int untilSkipCount = tilesBeforeSkip;
        for (int i = 0; i < tilesInDirection; i++)
        {
            Terrain_Tile.CreateTile(this, tileData_spike, new Vector2(tileWidth * i, 0), localScale: new Vector2(1, 1), localRotation: MathF.PI, shouldSpawnCollider: true, isTrap: true);
            untilSkipCount--;
            if (untilSkipCount == 0)
            {
                if (areSpikesSeparated && tilesInHeight > 0)
                {
                    CreateRectangleTerrain(localPosition: new Vector2((i - (float)(tilesBeforeSkip - 1) / 2) * tileWidth, (float)(tilesInHeight + 1) / 2 * tileHeight), tilesInWidth: tilesBeforeSkip);
                }
                untilSkipCount = tilesBeforeSkip;
                i += skipTillNext;
            }
        }

        if (!areSpikesSeparated && tilesInHeight > 0)
        {
            CreateRectangleTerrain(localPosition: new Vector2(((float)tilesInDirection - 1) / 2 * tileWidth, (float)(tilesInHeight + 1) / 2 * tileHeight), tilesInWidth: tilesInDirection);
        }
        /*
        for (int i = 0; i < gameObject.childCount; i++) {
            gameObject.GetChild(i).AddComponent(new Trap(initialDamage: 10, isOnlyPlayerAffected: true));
        }*/

        base.LoadContent();

        originalIsActive = gameObject.isActive;
    }


    private void CreateRectangleTerrain(Vector2 localPosition, int tilesInWidth)
    {
        GameObject terrainRect = new GameObject(tag: GameConstantsAndValues.Tags.Terrain.ToString());
        terrainRect.CreateTransform(localPosition: localPosition);
        Terrain_Rectangle terrain_Rectangle = new Terrain_Rectangle(
            cornerTileName: cornerTileName,
            sideTileName: sideTileName,
            middleTileName: middleTileName,
            tilesInWidth: tilesInWidth,
            tilesInHeight: tilesInHeight,
            cutBottom: false
        );

        terrainRect.AddComponent(terrain_Rectangle);

        gameObject.AddChild(terrainRect);
    }

    private bool originalIsActive;
    public void Reset()
    {
        gameObject.SetActive(originalIsActive);
    }
}
