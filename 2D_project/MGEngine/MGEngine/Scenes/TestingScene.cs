using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class TestingScene(Game game, Camera? mainCamera = null) : Scene(game, mainCamera)
//public class TestingScene(Game game, Camera? mainCamera = null) : GridScene(game, mainCamera)
{
    public override void Update(GameTime gameTime)
    {
        //Debug.WriteLine($"gameObject-count: {gameObjects.Count + overlayGameObjects.Count}" );
        // controller to change renderers

        if (SceneManager.Instance is null || InputController.Instance is null) return;

        bool isLeftShiftDown = InputController.Instance.IsKeyHeld(Keys.LeftShift);
        bool isLeftAltDown = InputController.Instance.IsKeyHeld(Keys.LeftAlt);
        bool isRDown = InputController.Instance.IsKeyHeld(Keys.R);
        bool isDDown = InputController.Instance.IsKeyHeld(Keys.D);
        bool isODown = InputController.Instance.IsKeyHeld(Keys.O);
        bool isPlusDown = InputController.Instance.IsKeyHeld(Keys.Add);
        bool isMinusDown = InputController.Instance.IsKeyHeld(Keys.OemMinus);
        bool isIDown = InputController.Instance.IsKeyHeld(Keys.I);
        bool isPDown = InputController.Instance.IsKeyHeld(Keys.P);
        bool isEPressed = InputController.Instance.IsKeyPressed(Keys.E);
        bool isCPressed = InputController.Instance.IsKeyPressed(Keys.C);
        bool isDPressed = InputController.Instance.IsKeyPressed(Keys.D);

        // camera keybinds
        if (isLeftShiftDown && isPlusDown)
        {
            mainCamera.Zoom += 0.1f;
        }
        else if (isLeftShiftDown && isMinusDown)
        {
            mainCamera.Zoom -= 0.1f;
        }
        else if (isLeftShiftDown && isRDown && isCPressed)
        {
            mainCamera.Zoom = 1.75f;
        }

        Renderer? renderer = SceneManager.Instance?.activeRenderer;
        DebugRenderer? debugRenderer = SceneManager.Instance?.activeDebugRenderer;
        OverlayRenderer? overlayRenderer = SceneManager.Instance?.activeOverlayRenderer;

        // debug renderer show gameobject ids
        if (debugRenderer is not null && isLeftShiftDown && isPDown && isDPressed)
        {
            if (!debugRenderer.Enabled)
            {
                Debug.WriteLine("Debug renderer not enabled");
            }
            else
            {
                debugRenderer.shouldShowGParentameObjectIDs = !debugRenderer.shouldShowGParentameObjectIDs;
                Debug.WriteLine($"Showing gameObject ids: {debugRenderer.shouldShowGParentameObjectIDs}");
            }
        }

        // debug renderer show parent gameobject ids
        if (debugRenderer is not null && isLeftShiftDown && isIDown && isDPressed)
        {
            if (!debugRenderer.Enabled)
            {
                Debug.WriteLine("Debug renderer not enabled");
            }
            else
            {
                debugRenderer.shouldShowGameObjectIDs = !debugRenderer.shouldShowGameObjectIDs;
                Debug.WriteLine($"Showing gameObject ids: {debugRenderer.shouldShowGameObjectIDs}");
            }
        }

        // renderer keybinds
        if (renderer is not null && isLeftShiftDown && isRDown && isEPressed)
        {
            renderer.Enabled = !renderer.Enabled;
            Debug.WriteLine("\n  Renderer is enabled: " + renderer.Enabled);
            Debug.WriteLine("\n______________________________________________________________\n");
        }
        else if (debugRenderer is not null && isLeftShiftDown && isDDown && isEPressed)
        {
            debugRenderer.Enabled = !debugRenderer.Enabled;
            Debug.WriteLine("\n  DebugRenderer is enabled: " + debugRenderer.Enabled);
            Debug.WriteLine("\n______________________________________________________________\n");
        }
        else if (overlayRenderer is not null && isLeftShiftDown && isODown && isEPressed)
        {
            overlayRenderer.Enabled = !overlayRenderer.Enabled;
            Debug.WriteLine("\n  OverlayRenderer is enabled: " + overlayRenderer.Enabled);
            Debug.WriteLine("\n______________________________________________________________\n");
        }
        base.Update(gameTime);
    }
}

