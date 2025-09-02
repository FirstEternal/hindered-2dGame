using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public enum GameAction
{
    LEFT, // A
    RIGHT, // D
    JUMP, // Space
    CHARGING, // hold left mouse key
    SWAP_WEAPON, // Q
    ATTACK, // press left mouse key
    CHANGE_ELEMENT, // E
    SPECIAL_ABILITY, // R
}

public enum InputType
{
    Keyboard,
    Mouse
}

public struct InputBinding
{
    public InputType Type;
    public Keys Key; // valid if Type == Keyboard
    public InputController.MouseKey MouseButton; // valid if Type == Mouse

    public InputBinding(Keys key)
    {
        Type = InputType.Keyboard;
        Key = key;
        MouseButton = InputController.MouseKey.LeftButton; // dummy default
    }

    public InputBinding(InputController.MouseKey mouseButton)
    {
        Type = InputType.Mouse;
        MouseButton = mouseButton;
        Key = Keys.None;
    }

    public override string ToString()
    {
        if (Type == InputType.Keyboard)
        {
            return Key.ToString().ToUpper();
        }

        switch (MouseButton)
        {
            case InputController.MouseKey.LeftButton:
                return "LMB";
            case InputController.MouseKey.MiddleButton:
                return "MMB";
            case InputController.MouseKey.RightButton:
                return "RMB";
        }

        return "UNKNOWN";
        //return Type == InputType.Keyboard ? Key.ToString().ToUpper() : MouseButton.ToString().ToUpper();
    }
}

public class KeyBindManager
{
    public static KeyBindManager Instance;
    private Dictionary<GameAction, InputBinding> bindings;
    private InputController inputController;

    public Action<string> OnRebind;

    public KeyBindManager(InputController controller)
    {
        if (Instance is not null) return;
        Instance = this;
        inputController = controller;

        // Default bindings
        bindings = new Dictionary<GameAction, InputBinding>
        {
            { GameAction.LEFT, new InputBinding(Keys.A) },
            { GameAction.RIGHT, new InputBinding(Keys.D) },
            { GameAction.JUMP, new InputBinding(Keys.Space) },
            { GameAction.CHARGING, new InputBinding(InputController.MouseKey.LeftButton) }, // for hold detection
            { GameAction.ATTACK, new InputBinding(InputController.MouseKey.LeftButton) }, // for press detection
            { GameAction.SWAP_WEAPON, new InputBinding(Keys.Q) },
            { GameAction.CHANGE_ELEMENT, new InputBinding(Keys.E) },
            { GameAction.SPECIAL_ABILITY, new InputBinding(Keys.R) }
        };
    }

    // Check if the action was just pressed (transition from up to down)
    public bool IsActionPressed(GameAction action)
    {
        if (!bindings.TryGetValue(action, out var binding)) return false;

        if (binding.Type == InputType.Keyboard)
        {
            return inputController.IsKeyPressed(binding.Key);
        }
        else // Mouse
        {
            return inputController.IsMouseKeyPressed(binding.MouseButton);
        }
    }

    // Check if the action is currently held down
    public bool IsActionHeld(GameAction action)
    {
        if (!bindings.TryGetValue(action, out var binding)) return false;

        if (binding.Type == InputType.Keyboard)
        {
            return inputController.IsKeyHeld(binding.Key);
        }
        else
        {
            return inputController.IsMouseKeyHeld(binding.MouseButton);
        }
    }

    // Change the binding for an action
    public void RebindAction(GameAction action, InputBinding newBinding)
    {
        // Step 1: Find any action that already uses this binding
        GameAction? existingAction = null;
        foreach (var kvp in bindings)
        {
            if (kvp.Value.Type == newBinding.Type)
            {
                if (kvp.Value.Type == InputType.Keyboard && kvp.Value.Key == newBinding.Key)
                    existingAction = kvp.Key;
                else if (kvp.Value.Type == InputType.Mouse && kvp.Value.MouseButton == newBinding.MouseButton)
                    existingAction = kvp.Key;
            }
        }

        // Step 2: Unbind it from the old action
        if (existingAction.HasValue)
        {
            bindings[existingAction.Value] = default; // or a "None" binding
        }

        // Step 3: Assign to the new action
        bindings[action] = newBinding;

        OnRebind?.Invoke(null);
    }


    // Get the current binding for display/UI
    public InputBinding GetBinding(GameAction action)
    {
        return bindings.TryGetValue(action, out var binding) ? binding : default;
    }
}
