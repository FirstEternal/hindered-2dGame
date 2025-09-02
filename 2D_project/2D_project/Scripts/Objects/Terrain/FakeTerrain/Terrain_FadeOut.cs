using GamePlatformer;

internal class Terrain_FadeOut : ObjectComponent, IResettable
{
    int tileWidth;
    int tileHeight;

    FadeIndicator fadeIndicator;

    public Terrain_FadeOut(int widthInTiles, int heightInTiles)
    {
        this.tileWidth = widthInTiles * GameConstantsAndValues.SQUARE_TILE_WIDTH;
        this.tileHeight = heightInTiles * GameConstantsAndValues.SQUARE_TILE_WIDTH;
    }

    public override void Initialize()
    {
        fadeIndicator = new FadeIndicator(Game2DPlatformer.Instance, gameObject.transform.globalPosition, tileWidth, tileHeight, duration: 1);

        fadeIndicator.OnFadeComplete += (fi) =>
        {
            gameObject.SetActive(false);
        };

        originalIsActive = gameObject.isActive;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        fadeIndicator.Start(gameObject.transform.globalPosition);
    }

    private bool originalIsActive;
    public void Reset()
    {
        gameObject.SetActive(originalIsActive);
    }

}