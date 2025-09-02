
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class Condition_KeyPressed(Keys key) : ICondition
{
    public event EventHandler? OnConditionMet;

    Keys key = key;

    public void Update(GameTime gameTime)
    {
        if (InputController.Instance is null) return;
        if (InputController.Instance.IsKeyPressed(key)) OnConditionMet?.Invoke(this, EventArgs.Empty);
    }
}
