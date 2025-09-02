using Microsoft.Xna.Framework;

internal class Button_HoverColorChange
{
    public static void AddColorChangeAndSoundEffectOnHover(Button button, Color originalColor)
    {
        float opacity = 0.5f;
        Color colorWithOpacity = new Color((byte)11, (byte)125, (byte)200, (byte)(opacity * 255));

        button.AssignOnHoverEnterAction((parameters) =>
        {
            OnHoverColorChange([button, colorWithOpacity]);
            PlayButtonSoundEffect(GameConstantsAndValues.SoundEffectNames.SFX_ON_HOVER_ENTER.ToString());
        }
        , null);
        button.AssignOnHoverExitAction(OnHoverColorChange, [button, originalColor]);
    }

    public static void AddSoundEffectAndOnClickAction(Button button, IOnClick.OnClickAction action, object[] parameters)
    {
        button.AssignOnClickAction((parameters) =>
        {
            action(parameters);
            PlayButtonSoundEffect(GameConstantsAndValues.SoundEffectNames.SFX_ON_CLICK.ToString());
        }, parameters);
    }

    public static void OnHoverColorChange(object[] parameters)
    {
        Button button = (Button)parameters[0];
        if (!button.canPress) return;

        Color color = (Color)parameters[1];
        button.colorTint = color;
    }

    public static void PlayButtonSoundEffect(string soundEffectName)
    {
        if (soundEffectName == "") return;
        SoundController.instance.PlaySoundEffect(soundEffectName);
    }
}

