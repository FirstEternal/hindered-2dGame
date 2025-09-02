using GamePlatformer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

internal class JSON_Manager
{
    public static Texture2D tileSpriteSheet { get; private set; }
    public static Texture2D weaponBowSpriteSheet { get; private set; }
    public static Texture2D weaponBladeSpriteSheet { get; private set; }
    public static Texture2D playerSpriteSheet { get; private set; }
    public static Texture2D enemiesSpriteSheet { get; private set; }
    public static Texture2D uiSpriteSheet { get; private set; }

    private static Dictionary<string, TileData> tiles;

    private static Dictionary<string, TileData> weaponBowTileData;
    private static Dictionary<string, TileData> weaponBladeTileData;

    private static Dictionary<string, TileData> uiTileData;

    private static Dictionary<string, TileData> enemiesTileData;
    private static Dictionary<string, TileData> playerTileData;

    public static BitmapFont_equalHeight_dynamicWidth customBitmapFont;
    public static BitmapFont_equalHeight_dynamicWidth SpaceBitmapFont;

    public static string songs;
    public static string soundEffects;

    public void LoadJson()
    {
        // Load the JSON file
        string jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "TerrainSpriteSheet.json");
        string imageFilePath = Path.Combine("sprites", "TerrainSpriteSheet");
        var json = File.ReadAllText(jsonFilePath);

        tileSpriteSheet = Game2DPlatformer.Instance.Content.Load<Texture2D>(imageFilePath);//"sprites/TerrainSpriteSheet");

        // Deserialize the JSON into FrameData
        var frameData = JsonConvert.DeserializeObject<FrameData>(json);
        tiles = frameData.frames; // Access the dictionary of frames

        foreach (var tile in tiles.Values)
        {
            GameConstantsAndValues.SQUARE_TILE_WIDTH = tile.sourceSize.w;
            break;
        }

        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "WeaponBowSpriteSheet.json");
        json = File.ReadAllText(jsonFilePath);
        imageFilePath = Path.Combine("sprites", "WeaponBowSpriteSheet");
        weaponBowSpriteSheet = Game2DPlatformer.Instance.Content.Load<Texture2D>(imageFilePath);
        weaponBowTileData = JsonConvert.DeserializeObject<FrameData>(json).frames;

        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "WeaponBladeSpriteSheet.json");
        json = File.ReadAllText(jsonFilePath);
        imageFilePath = Path.Combine("sprites", "WeaponBladeSpriteSheet");
        weaponBladeSpriteSheet = Game2DPlatformer.Instance.Content.Load<Texture2D>(imageFilePath);
        weaponBladeTileData = JsonConvert.DeserializeObject<FrameData>(json).frames;

        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "PlayerSpriteSheet.json");
        json = File.ReadAllText(jsonFilePath);
        imageFilePath = Path.Combine("sprites", "PlayerSpriteSheet");
        playerSpriteSheet = Game2DPlatformer.Instance.Content.Load<Texture2D>(imageFilePath);
        playerTileData = JsonConvert.DeserializeObject<FrameData>(json).frames;

        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "EnemiesSpriteSheet.json");
        json = File.ReadAllText(jsonFilePath);
        imageFilePath = Path.Combine("sprites", "EnemiesSpriteSheet");
        enemiesSpriteSheet = Game2DPlatformer.Instance.Content.Load<Texture2D>(imageFilePath);
        enemiesTileData = JsonConvert.DeserializeObject<FrameData>(json).frames;

        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "UISpriteSheet.json");
        json = File.ReadAllText(jsonFilePath);
        imageFilePath = Path.Combine("sprites", "UISpriteSheet");
        uiSpriteSheet = Game2DPlatformer.Instance.Content.Load<Texture2D>(imageFilePath);
        uiTileData = JsonConvert.DeserializeObject<FrameData>(json).frames;

        //Texture2D fontTexture = Game2DPlatformer.Instance.Content.Load<Texture2D>(Path.Combine("sprites", "SpaceFontSpriteSheet"));
        //Texture2D fontTexture = Game2DPlatformer.Instance.Content.Load<Texture2D>(Path.Combine("sprites", "Custom_FontBitMap"));
        Texture2D fontTexture_normal = Game2DPlatformer.Instance.Content.Load<Texture2D>(Path.Combine("sprites", "grey_FontBitMap"));
        Texture2D fontTexture_bold = Game2DPlatformer.Instance.Content.Load<Texture2D>(Path.Combine("sprites", "grey_FontBitMap"));
        Texture2D fontTexture_italic = Game2DPlatformer.Instance.Content.Load<Texture2D>(Path.Combine("sprites", "grey_FontBitMap"));

        //jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "SpaceFontSpriteSheet.json");
        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "grey_FontBitMap.json");
        json = File.ReadAllText(jsonFilePath);
        Dictionary<string, TileData> fontDict_normal = JsonConvert.DeserializeObject<FrameData>(json).frames;

        Dictionary<string, TileData> fontDict_bold = null;
        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "Custom_FontBitMap_Bold.json");
        if (File.Exists(jsonFilePath))
        {
            json = File.ReadAllText(jsonFilePath);
            fontDict_bold = JsonConvert.DeserializeObject<FrameData>(json).frames;
        }

        Dictionary<string, TileData> fontDict_italic = null;
        jsonFilePath = Path.Combine("..", "..", "..", "Content", "sprites", "Custom_FontBitMap_Italic.json");
        if (File.Exists(jsonFilePath))
        {
            json = File.ReadAllText(jsonFilePath);
            fontDict_italic = JsonConvert.DeserializeObject<FrameData>(json).frames;
        }

        //Dictionary<char, Rectangle> charDict = new Dictionary<char, Rectangle>();
        Dictionary<char, BitmapFont_equalHeight_dynamicWidth.Character> charDict_normal = new Dictionary<char, BitmapFont_equalHeight_dynamicWidth.Character>();
        Dictionary<char, BitmapFont_equalHeight_dynamicWidth.Character> charDict_bold = new Dictionary<char, BitmapFont_equalHeight_dynamicWidth.Character>();
        Dictionary<char, BitmapFont_equalHeight_dynamicWidth.Character> charDict_italic = new Dictionary<char, BitmapFont_equalHeight_dynamicWidth.Character>();

        int i = 0;
        foreach (string charKey in fontDict_normal.Keys)
        {
            // 1.) normal
            Frame frame = fontDict_normal[charKey].frame;
            char charVal = charKey[charKey.Length - 5];

            charDict_normal[charVal] = new BitmapFont_equalHeight_dynamicWidth.Character(
                fontTexture: fontTexture_normal,
                sourceRectangle: new Rectangle(frame.x, frame.y, frame.w, frame.h),
                origin: GetOrigin(fontDict_normal[charKey])
            );

            // 2.) bold
            if (fontDict_bold is not null)
            {
                string charName = $"{charKey}_bold";
                frame = fontDict_bold[charName].frame;

                charDict_bold[charVal] = new BitmapFont_equalHeight_dynamicWidth.Character(
                    fontTexture: fontTexture_bold,
                    sourceRectangle: new Rectangle(frame.x, frame.y, frame.w, frame.h),
                    origin: GetOrigin(fontDict_bold[charName])
                );
            }

            // 3.) italic

            if (fontDict_italic is not null)
            {
                string charName = $"{charKey}_italic";
                frame = fontDict_italic[charName].frame;

                charDict_italic[charVal] = new BitmapFont_equalHeight_dynamicWidth.Character(
                    fontTexture: fontTexture_italic,
                    sourceRectangle: new Rectangle(frame.x, frame.y, frame.w, frame.h),
                    origin: GetOrigin(fontDict_italic[charName])
                );
            }

            i++;
        }
        customBitmapFont = new BitmapFont_equalHeight_dynamicWidth(fontTexture_normal, charDict_normal, charDict_bold, charDict_italic, originalFontSize: 60);
    }

    public static Rectangle GetTileSourceRectangle(string tileName)
    {
        if (tiles == null) throw new NullReferenceException();
        TileData tileData1 = tiles.TryGetValue(tileName + ".png", out TileData tileData) ? tileData : throw new FileNotFoundException();
        return GetRectangle(tileData1);
    }
    public static Rectangle GetUITile(string tileName)
    {
        if (uiTileData == null) throw new NullReferenceException();
        TileData tileData1 = uiTileData.TryGetValue(tileName + ".png", out TileData tileData) ? tileData : throw new FileNotFoundException();
        if (tileData1 is null) return Rectangle.Empty;

        return new Rectangle(
            tileData1.frame.x,
            tileData1.frame.y,
            tileData1.frame.w,
            tileData1.frame.h
        );
    }

    public static Rectangle GetWeaponBowSourceRectangle(string tileName)
    {
        if (weaponBowTileData == null) throw new NullReferenceException();
        TileData tileData1 = weaponBowTileData.TryGetValue(tileName + ".png", out TileData tileData) ? tileData : throw new FileNotFoundException();
        return GetRectangle(tileData1);
        /*
        if (tileData1 is null) return Rectangle.Empty;

        return new Rectangle(
            tileData1.frame.x,
            tileData1.frame.y,
            tileData1.frame.w,
            tileData1.frame.h
        );

        return GetRectangle(tileData1);*/
    }

    public static Rectangle GetWeaponBladeSourceRectangle(string tileName)
    {
        if (weaponBladeTileData == null) throw new NullReferenceException();
        TileData tileData1 = weaponBladeTileData.TryGetValue(tileName + ".png", out TileData tileData) ? tileData : throw new FileNotFoundException();
        if (tileData1 is null) return Rectangle.Empty;

        return new Rectangle(
            tileData1.frame.x,
            tileData1.frame.y,
            tileData1.frame.w,
            tileData1.frame.h
        );
    }

    public static Rectangle[] GetPlayerSourceRectangle(string tileName, int animatedSpriteCount)
    {
        return GetRectangles(playerTileData, tileName, animatedSpriteCount);
    }

    public static Rectangle[] GetProjectileSourceRectangles(string tileName, int animatedSpriteCount)
    {
        return GetRectangles(weaponBowTileData, tileName, animatedSpriteCount);
    }

    public static Rectangle[] GetEnemiesSourceRectangles(string tileName, int animatedSpriteCount)
    {
        return GetRectangles(enemiesTileData, tileName, animatedSpriteCount);
    }

    public static Vector2[] GetUIOrigins(string tileName, int animatedSpriteCount, Vector2 gameObjectScale)
    {
        return GetPivots(uiTileData, tileName, animatedSpriteCount, gameObjectScale);
    }

    public static Vector2 GetUIOrigin(string tileName, Vector2 gameObjectScale)
    {
        return GetPivot(uiTileData, tileName, gameObjectScale);
    }

    public static Vector2[] GetPlayerOrigin(string tileName, int animatedSpriteCount, Vector2 gameObjectScale)
    {
        return GetPivots(playerTileData, tileName, animatedSpriteCount, gameObjectScale);
    }

    public static Vector2[] GetEnemiesOrigin(string tileName, int animatedSpriteCount, Vector2 gameObjectScale)
    {
        return GetPivots(enemiesTileData, tileName, animatedSpriteCount, gameObjectScale);
    }
    public static Vector2[] GetProjectileOrigin(string tileName, int animatedSpriteCount, Vector2 gameObjectScale)
    {
        return GetPivots(weaponBowTileData, tileName, animatedSpriteCount, gameObjectScale);
    }

    private static Rectangle GetRectangle(TileData tileData)
    {
        if (tileData is null) return Rectangle.Empty;

        return new Rectangle(
            tileData.frame.x,
            tileData.frame.y,
            tileData.frame.w,
            tileData.frame.h
        );
    }

    private static Vector2 GetOrigin(TileData tileData)
    {
        if (tileData == null) return Vector2.Zero;

        // Step 1: Compute pivot in original untrimmed space
        var pivotInOriginal = new Vector2(
            tileData.pivot.x * tileData.sourceSize.w,
            tileData.pivot.y * tileData.sourceSize.h
        );

        // Step 2: Adjust for how the trimmed frame is positioned inside the original
        var offset = new Vector2(
            tileData.spriteSourceSize.x,
            tileData.spriteSourceSize.y
        );

        // Step 3: Pivot relative to trimmed image (used for rendering offset)
        var originInTrimmed = pivotInOriginal - offset;

        return originInTrimmed;
    }

    /*
    private static Vector2 GetOrigin(TileData tileData, Vector2 gameObjectScale)
    {
        if (tileData is null) return Vector2.Zero;

        // pivot * actual frame size
        return new Vector2(tileData.pivot.x, tileData.pivot.y) * new Vector2(tileData.frame.w, tileData.frame.h);
    }*/

    public static Vector2[] GetPivots(Dictionary<string, TileData> tileDataDict, string tileName, int animatedSpriteCount, Vector2 gameObjectScale)
    {
        if (tileDataDict == null) throw new NullReferenceException();
        Vector2[] origins = new Vector2[animatedSpriteCount];
        for (int i = 0; i < animatedSpriteCount; i++)
        {
            TileData tileData1 = tileDataDict.TryGetValue(tileName + $"_{i}.png", out TileData tileData) ? tileData : throw new FileNotFoundException();
            if (tileData1 is null && animatedSpriteCount == 1) tileData1 = tileDataDict.TryGetValue(tileName + $".png", out TileData tileData2) ? tileData2 : throw new FileNotFoundException();
            origins[i] = GetOrigin(tileData1);
        }

        return origins;
    }

    public static Vector2 GetPivot(Dictionary<string, TileData> tileDataDict, string tileName, Vector2 gameObjectScale)
    {
        if (tileDataDict == null) throw new NullReferenceException();

        TileData tileData1 = tileDataDict.TryGetValue(tileName + $".png", out TileData tileData) ? tileData : throw new FileNotFoundException();

        return GetOrigin(tileData1);
    }

    public static Rectangle[] GetRectangles(Dictionary<string, TileData> tileDataDict, string tileName, int animatedSpriteCount)
    {
        if (tileDataDict == null) throw new NullReferenceException();
        Rectangle[] rectangles = new Rectangle[animatedSpriteCount];
        for (int i = 0; i < animatedSpriteCount; i++)
        {
            TileData tileData1 = tileDataDict.TryGetValue(tileName + $"_{i}.png", out TileData tileData) ? tileData : throw new FileNotFoundException();
            rectangles[i] = GetRectangle(tileData1);
        }

        return rectangles;
    }
}

// DESERIALIZE INFORMATION
public class FrameData
{
    public Dictionary<string, TileData> frames { get; set; }
}
public class Frame
{
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class SpriteSourceSize
{
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class SourceSize
{
    public int w { get; set; }
    public int h { get; set; }
}

public class TileData
{
    public Frame frame { get; set; }
    public bool rotated { get; set; }
    public bool trimmed { get; set; }
    public SpriteSourceSize spriteSourceSize { get; set; }
    public SourceSize sourceSize { get; set; }
    public Pivot pivot { get; set; }
}

public class Pivot
{
    public float x { get; set; }
    public float y { get; set; }
}

