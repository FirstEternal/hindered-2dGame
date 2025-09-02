using Microsoft.Xna.Framework;

internal class Scene_MenuScene(Game game) : TestingScene(game)
{
    protected override void InitializeContent()
    {
        new FullMenu(this);
        base.InitializeContent();
    }
}
