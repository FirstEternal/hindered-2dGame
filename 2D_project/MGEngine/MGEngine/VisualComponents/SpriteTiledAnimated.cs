using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteTiledAnimated: SpriteTiled
{
    public void DisplayNextFrame() { }

    public void PauseAnimation() { }

    public void ResumeAnimation() { }

    public void ResetAnimation() { }

    public void SetExtentionLength(int frameIndex) { }

    //virtual void AssignFrame() { }


    public GameObject gameObjectAnimated { get => gameObject; private set => gameObject = value; }
    public int currFrameIndex => 0;

    public bool isAnimationPaused => false;

    public bool animationEnabled { get => true; set => animationEnabled = false; }

    private bool _loopEnabled;

    public bool loopEnabled { get => _loopEnabled; set => _loopEnabled = value; }
    public int currMinFrameIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    public SpriteTiledAnimated(Texture2D texture, Rectangle[] sourceRectangles, Color[]? colorTints = null, Vector2[]? origins = null, float[]? layerDepths = null) : base(texture, sourceRectangles, colorTints, origins, layerDepths)
    {
    }
}
