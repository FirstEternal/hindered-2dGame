using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputController : GameComponent
{
    public static InputController? Instance { get; private set; }

    public InputController(Game game) : base(game)
    {
        if (Instance is null) Instance = this;

        prevKeyboardState = Keyboard.GetState();
        prevMouseState = Mouse.GetState();
    }

    // Fields to track the state of the keyboard and mouse
    KeyboardState prevKeyboardState;
    KeyboardState currKeyboardState;
    MouseState prevMouseState;
    MouseState currMouseState;

    // New flags for tracking mouse press and release
    private bool isMousePressed = false;
    public bool MouseReleasedAfterPress { get; private set; }

    public override void Update(GameTime gameTime)
    {
        prevKeyboardState = currKeyboardState;
        prevMouseState = currMouseState;

        currKeyboardState = Keyboard.GetState();
        currMouseState = Mouse.GetState();

        // Mouse press/release logic
        if (currMouseState.LeftButton == ButtonState.Pressed)
        {
            isMousePressed = true;
            MouseReleasedAfterPress = false;  // Reset the release flag when the button is pressed
        }
        else if (isMousePressed && currMouseState.LeftButton == ButtonState.Released)
        {
            MouseReleasedAfterPress = true;
            isMousePressed = false;  // Reset press state after release
        }
    }

    // Check if a key was pressed (transition from Up to Down)
    public bool IsKeyPressed(Keys key)
    {
        return prevKeyboardState.IsKeyUp(key) && currKeyboardState.IsKeyDown(key);
    }

    // Check if a key is held down (currently being pressed)
    public bool IsKeyHeld(Keys key)
    {
        return currKeyboardState.IsKeyDown(key);
    }

    public enum MouseKey
    {
        LeftButton,
        RightButton,
        MiddleButton
    }

    // Check if a specific mouse key was pressed (transition from Up to Down)
    public bool IsMouseKeyPressed(MouseKey key)
    {
        switch (key)
        {
            case MouseKey.LeftButton:
                return prevMouseState.LeftButton == ButtonState.Released && currMouseState.LeftButton == ButtonState.Pressed;
            case MouseKey.RightButton:
                return prevMouseState.RightButton == ButtonState.Released && currMouseState.RightButton == ButtonState.Pressed;
            case MouseKey.MiddleButton:
                return prevMouseState.MiddleButton == ButtonState.Released && currMouseState.MiddleButton == ButtonState.Pressed;
            default:
                return false;
        }
    }

    // Check if a specific mouse key is being held down
    public bool IsMouseKeyHeld(MouseKey key)
    {
        switch (key)
        {
            case MouseKey.LeftButton:
                return currMouseState.LeftButton == ButtonState.Pressed;
            case MouseKey.RightButton:
                return currMouseState.RightButton == ButtonState.Pressed;
            case MouseKey.MiddleButton:
                return currMouseState.MiddleButton == ButtonState.Pressed;
            default:
                return false;
        }
    }

}
