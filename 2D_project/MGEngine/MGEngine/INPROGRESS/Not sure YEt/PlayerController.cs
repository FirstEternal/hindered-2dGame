using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

internal class PlayerController(Game game) : GameComponent(game)
{
    // TODO !!!!!!!!!!!!!!!!!!!!!!!!!
















    public KeyboardState keyboardState { get; private set; }
    public MouseState mouseState { get; private set; }
    bool isInput;

    //public Dictionary<Keys, EventHandler> currentPressedKeys = new Dictionary<Keys, EventHandler>();
    private List<Keys> gameKeys = new List<Keys>();

    public EventHandler onKeyPressed;
    public EventHandler onKeyHeld;

    public class KeyEventArgs : EventArgs
    {
        Keys Key;
    }

    /// <summary>
    /// assign events to all keys that will be used during a game
    /// </summary>
    /*
    public void SetUpGameKeysDictionary(Dictionary<Keys, EventHandler> currentPressedKeys)
    {
        this.currentPressedKeys = currentPressedKeys;
        
        foreach (Keys key in keys)
        {
            // state of being pressed or not
            currentPressedKeys[key]?.Invoke(this, EventArgs.Empty); 
        }
    }*/
    /*
    public void SetUpGameKeys(List<Keys> gameKeys)
    {
        this.gameKeys = gameKeys;   
    }

    private void UpdateGameKeyStates()
    {
        KeyboardState state = Keyboard.GetState();
        Keys[] pressedKeys = state.GetPressedKeys();

        foreach (Keys key in currentPressedKeys.Keys)
        {
            // update state
            currentPressedKeys[key] = state.IsKeyDown(key);
        }
    }

    public override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();

        //translationVector = Vector2.Zero; 
        MouseState mouseState = Mouse.GetState();

        // Check if the left mouse button is clicked
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
        }

        // Get movement input from the player
        KeyboardState keyboardState = Keyboard.GetState();

        Vector2 translationVector = Vector2.Zero;

        // player movement
        if (IsKeyHeld(Keys.Left) || IsKeyHeld(Keys.A))
        {
        }
        else if (IsKeyHeld(Keys.Right) || IsKeyHeld(Keys.D))
        {
            isMoving = true;
            translationVector.X = 1;
            isSpriteFlipped = false;
        }
        // TESTING ONLY
        else if (IsKeyHeld(Keys.Up) || IsKeyHeld(Keys.W))
        {
            isMoving = true;
            translationVector.Y = -1;
        }
        else if (IsKeyHeld(Keys.Down) || IsKeyHeld(Keys.S))
        {
            isMoving = true;
            translationVector.Y = 1;

        }
        // END TESTING
        if (IsKeyPressed(Keys.Z))
        {
            healthBar.currHealth -= 10;
            Debug.WriteLine("health loss");
        }
        if (IsKeyPressed(Keys.U))
        {
            healthBar.currShield -= 10;
            Debug.WriteLine("shield loss");
        }


        // translation
        if (translationVector.X != 0) isInput = true;

        Movement.MoveInLine(gameTime, translationVector, moveSpeed, this.gameObject.transform);

        // jump logic
        if (IsKeyPressed(Keys.Space))
        {
            isInput = true;
            Jump();
        }

        // swap weapons: Bow -> Blade -> Bow
        if (IsKeyPressed(Keys.Q))
        {
            SwapWeapon();
        }

        // swap elements -> next element in dictionary
        if (IsKeyPressed(Keys.E))
        {
            SwapElements();
        }

        if (!isInput)
        {
            untilIdleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (untilIdleTime <= 0)
            {
                isIdle = true;
            }
        }
        else
        {
            ResetIdleTimer();
        }

        if (isIdle)
        {
            //PerformAnimation("Idle");
        }

        previousKeyboardState = currentKeyboardState;


        // move camera
        SceneManager.Instance.activeScene.mainCamera.Follow(gameObject);
    }

    private bool IsKeyPressed(Keys key)
    {
        return previousKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key);
    }

    // Function to detect if a key is held down
    private bool IsKeyHeld(Keys key)
    {
        return currentKeyboardState.IsKeyDown(key);
    }*/
}
