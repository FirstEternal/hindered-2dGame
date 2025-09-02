internal class HUD_GameOverMenu
{
    SpriteTextComponent menuText;
    public void UpdateDescription(string menuTitle)
    {
        menuText.text = menuTitle;
    }

    public HUD_GameOverMenu(HUD_PauseMenu pauseMenu)
    {
        menuText = pauseMenu.PauseMenu.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteTextComponent>();
    }
}

