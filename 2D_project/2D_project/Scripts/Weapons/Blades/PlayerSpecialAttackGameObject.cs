using MGEngine.ObjectBased;

internal class PlayerSpecialAttackGameObject
{
    SpriteAnimated charge1Sprite;
    SpriteAnimated charge2Sprite;
    SpriteAnimated charge3Sprite;
    public PlayerSpecialAttackGameObject()
    {
        GameObject gameObject = new GameObject();
        gameObject.CreateTransform();

        charge1Sprite = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("Charge_0", 7),
            frameTimers: [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue]
        );

        charge2Sprite = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("Charge_1", 7),
            frameTimers: [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue]
        );

        charge3Sprite = new SpriteAnimated(
            texture2D: JSON_Manager.playerSpriteSheet,
            sourceRectangles: JSON_Manager.GetPlayerSourceRectangle("Charge_2", 7),
            frameTimers: [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue]
        );
    }
}
