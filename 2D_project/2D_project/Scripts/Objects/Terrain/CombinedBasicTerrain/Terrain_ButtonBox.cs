using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

internal class Terrain_ButtonBox(
    string cornerTileName, string sideTileName, string middleTileName, string[] pressureTargetTags,
    int tilesInWidth = 0, int tilesInHeight = 0,
    float mass = int.MaxValue, bool isMovable = false, bool isGravity = false, float angularVelocity = 0, int resetInXSeconds = int.MaxValue)
    : PhysicsComponent(mass, isMovable: isMovable, isGravity: isGravity, angularVelocity: angularVelocity), ITerrain, IResettable
{

    private string[] pressureTargetTags = pressureTargetTags;

    //string CornerTileName = cornerTileName;
    public float tilesInWidth = tilesInWidth;
    public float tilesInHeight = tilesInHeight;
    private int resetInXSeconds = resetInXSeconds;

    PressureButton pressureButton;
    public event EventHandler OnPressureButtonPressed;

    private bool originalIsActive;
    public void Reset()
    {
        ResetButton();
        gameObject.SetActive(originalIsActive);
    }

    public void ResetButton()
    {
        pressureButton.Reset();
    }

    public override void Initialize()
    {
        if (tilesInHeight < 4) throw new Exception("minimum 4 height in tiles requiered for terrainButtonBox");
        // bottom rectangle
        GameObject bottomRectObject = new GameObject(tag: GameConstantsAndValues.Tags.Terrain.ToString());
        bottomRectObject.CreateTransform(localPosition: new Vector2(0, (tilesInHeight - 1) / 2 * GameConstantsAndValues.SQUARE_TILE_WIDTH));

        gameObject.AddChild(bottomRectObject);
        Terrain_Rectangle bottomRect = new Terrain_Rectangle(
            cornerTileName: cornerTileName,
            sideTileName: sideTileName,
            middleTileName: middleTileName,
            tilesInWidth: (int)tilesInWidth,
            tilesInHeight: 1,
            isMovable: isMovable,
            cutBottom: true
        );

        bottomRectObject.AddComponent(bottomRect);

        // top rectangle
        GameObject topRectObject = new GameObject(tag: GameConstantsAndValues.Tags.Terrain.ToString());
        topRectObject.CreateTransform(localRotationAngle: MathHelper.Pi, localPosition: new Vector2(0, -(tilesInHeight - 1) / 2 * GameConstantsAndValues.SQUARE_TILE_WIDTH));

        gameObject.AddChild(topRectObject);
        Terrain_Rectangle topRect = new Terrain_Rectangle(
            cornerTileName: cornerTileName,
            sideTileName: sideTileName,
            middleTileName: middleTileName,
            tilesInWidth: (int)tilesInWidth,
            tilesInHeight: 1,
            isMovable: isMovable,
            cutBottom: true
        );

        topRectObject.AddComponent(topRect);

        // left rectangle
        GameObject leftRectObject = new GameObject(tag: GameConstantsAndValues.Tags.Terrain.ToString());
        leftRectObject.CreateTransform(localRotationAngle: MathHelper.Pi / 2, localPosition: new Vector2(-(tilesInWidth - 1) / 2 * GameConstantsAndValues.SQUARE_TILE_WIDTH, 0));

        gameObject.AddChild(leftRectObject);
        Terrain_Rectangle leftRect = new Terrain_Rectangle(
            cornerTileName: cornerTileName,
            sideTileName: sideTileName,
            middleTileName: middleTileName,
            tilesInWidth: (int)tilesInHeight - 2,
            tilesInHeight: 1,
            isMovable: isMovable,
            cutBottom: false
        );

        leftRectObject.AddComponent(leftRect);

        // create pressure button
        GameObject pressureButtonObject = new GameObject();
        float pressureButtonWidth = 16;
        pressureButtonObject.CreateTransform(localPosition: new Vector2(-(tilesInWidth - 1) / 2 * GameConstantsAndValues.SQUARE_TILE_WIDTH + pressureButtonWidth, 0));
        pressureButton = new PressureButton(pressureTargetTags, resetInXSeconds);
        pressureButtonObject.AddComponent(pressureButton);

        // resend pressure button pressed event -> so that it can be assigned inside json
        pressureButton.OnPressureButtonPressed += (object sender, EventArgs e) => { OnPressureButtonPressed?.Invoke(this, EventArgs.Empty); };

        gameObject.AddChild(pressureButtonObject);

        base.Initialize();

        if (AngularVelocity > 0) Debug.WriteLine("ok");

        originalIsActive = gameObject.isActive;
    }
}