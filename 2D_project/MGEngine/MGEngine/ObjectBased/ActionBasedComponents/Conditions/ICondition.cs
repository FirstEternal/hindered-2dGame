using Microsoft.Xna.Framework;
public interface ICondition
{

    event EventHandler? OnConditionMet;
    public void Update(GameTime gameTime) { }
}