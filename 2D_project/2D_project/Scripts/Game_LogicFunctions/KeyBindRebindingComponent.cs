using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

internal class KeyBindRebindingComponent : ObjectComponent
{
    bool waitingForRebind = false;
    GameAction actionToRebind;
    PrefabObjectKeyBindWithLabel componentToUpdate;

    public void StartRebind(GameAction action, PrefabObjectKeyBindWithLabel component)
    {
        waitingForRebind = true;
        actionToRebind = action;
        componentToUpdate = component;

        componentToUpdate.button.colorTint = GameConstantsAndValues.PanelColor_lightBlue1;
        componentToUpdate.button.textField.spriteTextComponent.textColor = Color.Red;

        ButtonResponseSystem.Instance.Pause();
    }

    public override void Update(GameTime gameTime)
    {
        if (!waitingForRebind) return;

        // Check if any key pressed
        foreach (Keys key in Enum.GetValues(typeof(Keys)))
        {
            if (InputController.Instance.IsKeyPressed(key))
            {
                RebindingFunction(new InputBinding(key));
                break;
            }
        }

        // Check mouse buttons
        foreach (InputController.MouseKey mouseButton in Enum.GetValues(typeof(InputController.MouseKey)))
        {
            if (InputController.Instance.IsMouseKeyPressed(mouseButton))
            {
                RebindingFunction(new InputBinding(mouseButton));
                break;
            }
        }
    }

    private void RebindingFunction(InputBinding inputBinding)
    {
        KeyBindManager.Instance.RebindAction(actionToRebind, inputBinding);

        componentToUpdate.UpdateText(null);
        componentToUpdate.button.colorTint = Color.White;
        componentToUpdate.button.textField.spriteTextComponent.textColor = Color.Black;

        ButtonResponseSystem.Instance.Resume();

        waitingForRebind = false;
    }
}
