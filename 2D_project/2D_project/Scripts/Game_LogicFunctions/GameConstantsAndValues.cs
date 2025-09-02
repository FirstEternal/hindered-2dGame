using Microsoft.Xna.Framework;

internal class GameConstantsAndValues
{
    public enum FactionType
    {
        Boulderer,
        Burner,
        Drowner,
        Froster,
        Grasser,
        Shader,
        Thunderer,
        AntiMatter,
        AntiVerse,
        AntiMatterHand,
        AntiVerseHand,
    }

    public static int SQUARE_TILE_WIDTH;

    public const int FONT_SIZE_S = 15;
    public const int FONT_SIZE_M = 20;
    public const int FONT_SIZE_HM = 25;
    public const int FONT_SIZE_H = 30;

    public enum Tags
    {
        Terrain,
        Player,
        PlayerSpawned, // also counts player it self
        Enemy,
        EnemySpawned, // also counts enemy spawned
        Hidden, // invisible colliders
        MovableTerrain,
        GravitationalEnemy,
        Button,
        AntiTerrainPlayerSpawned, // projectiles that ignore terrain
        AntiTerrainEnemySpawned, // projectiles that ignore terrain
        EnemyWeakToProjectileOnly,
        PlayerMeleeSpawned,
        EnemyMeleeSpawned,
    }

    public enum States
    {
        IDLE,
        AGGRESSIVE,
        SPECIAL_ATTACK,
    }

    public enum SoundEffectNames
    {
        SFX_ON_HOVER_ENTER,
        SFX_ON_CLICK,
    }

    public static readonly Color PanelColor_Black = new Color(0, 0, 0, 200);
    public static readonly Color PanelColor_DarkBlue = new Color(53, 69, 111, 200);
    public static readonly Color PanelColor_Gray = new Color(42, 48, 60, 200);
    public static readonly Color PanelColor_GrayFull = new Color(42, 48, 60, 255);
    public static readonly Color PanelColor_lightBlue = new Color(83, 101, 153, 255);
    public static readonly Color PanelColor_lightBlue1 = new Color(74, 96, 146, 200);
    public static readonly Color PanelColor_DarkBlueFull = new Color(53, 58, 73, 255);

    public static int instaKillDamage = 10000;
    public static int bossMinId = 1000000;
    public static int BUTTON_HEIGHT = 50;
    public static int MENU_BUTTON_WIDTH = 250;
}

