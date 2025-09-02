using Microsoft.Xna.Framework;
using System.Diagnostics;

public class StateController
{
    List<State> states = new List<State>();
    private State? currentState;
    private int currentStateIndex = 0;

    public string GetCurrentStateName()
    {
        return currentState?.stateName ?? "";
    }
    public EventHandler? OnStateChange;
    public StateController(List<State> availableStates)
    {
        if (states is null)
        {
            Debug.WriteLine("Creating State Controller without states is not recommended");
            return;
        }
        states = availableStates;

        currentState = states[currentStateIndex];
        currentState.OnStateEnd -= TransitionToNextState;
        currentState.OnStateEnd += TransitionToNextState;
    }


    public void Update(GameTime gameTime)
    {
        // Update the current state
        if (currentState is not null)
        {
            currentState.Update(gameTime);
        }
    }

    public void TransitionToNextState(object? sender, EventArgs e)
    {
        if (currentState is null) return;

        // unscubscribe from current state
        currentState.OnStateEnd -= TransitionToNextState;

        // update currentStateIndex
        if (++currentStateIndex >= states.Count) currentStateIndex = 0;

        // scubscribe to new current state
        currentState = states[currentStateIndex];
        currentState.OnStateEnd -= TransitionToNextState;
        currentState.OnStateEnd += TransitionToNextState;
        currentState.OnStateEnter();

        OnStateChange?.Invoke(this, EventArgs.Empty);

        Debug.WriteLine($"new state:  {currentState.stateName}");
    }
}
