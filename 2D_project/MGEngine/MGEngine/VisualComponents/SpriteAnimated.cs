using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteAnimated : Sprite
{
    protected Color[]? colorTints;
    protected Rectangle[] sourceRectangles;
    protected Vector2[] origins;
    protected float[] layerDepths;
    public float[] frameTimers;
    float currFrameTimer = 0;
    public int currFrameIndex { get; private set; } = 0;

    private int _currMinFrameIndex = 0;

    public int currMinFrameIndex
    {
        get
        {
            return _currMinFrameIndex;
        }
        set
        {
            if (value < 0)
            {
                _currMinFrameIndex = 0;
            }
            else if (value >= sourceRectangles.Length)
            {
                _currMinFrameIndex = sourceRectangles.Length - 1;
            }
            else
            {
                _currMinFrameIndex = value;
            }
        }
    }
    public bool isAnimationPaused { get; private set; }

    private bool _animationEnabled;
    public bool loopEnabled = true;


    public EventHandler animationEnded;

    public EventHandler onFrameChange;
    public bool animationEnabled
    {
        get
        {
            return _animationEnabled;
        }
        set
        {
            _animationEnabled = value;
            ResetAnimation(); // reset animation
            if (_animationEnabled) ResumeAnimation(); // resume animation in case it was paused
        }
    }

    public void DisplayNextFrame()
    {
        currFrameTimer = 0;
        currFrameIndex++;
        if (currFrameIndex >= frameTimers.Length)
        {
            if (!loopEnabled)
            {
                currFrameIndex--;
                PauseAnimation();
                animationEnded?.Invoke(this, EventArgs.Empty);
                return;
            }

            currFrameIndex = currMinFrameIndex;
        }


        AssignFrame();
    }
    public void PauseAnimation()
    {
        isAnimationPaused = true;
    }
    public void ResumeAnimation()
    {
        isAnimationPaused = false;
    }

    private void ResetAnimation()
    {
        // reset animation to first frame 
        currFrameIndex = currMinFrameIndex;
        // reset timer
        currFrameTimer = 0;
        AssignFrame();
    }

    public SpriteAnimated(Texture2D texture2D, Rectangle[] sourceRectangles, float[] frameTimers, Vector2[]? origins = null, Color[]? colorTints = null, bool isAnimationDisabled = false, float[]? layerDepths = null) : base(texture2D, colorTint: Color.White)
    {
        this.texture2D = texture2D;
        this.sourceRectangles = sourceRectangles;
        if (colorTints is null)
        {
            colorTints = colorTints?.Length == frameTimers.Length ? colorTints : new Color[frameTimers.Length];
            Array.Fill(colorTints, Color.White);
        }
        else
        {
            this.colorTints = colorTints;
        }

        this.frameTimers = frameTimers;

        // set first texture as current
        currFrameIndex = currMinFrameIndex;

        if (origins is null)
        {
            this.origins = new Vector2[frameTimers.Length];
            // create origins
            for (int i = 0; i < sourceRectangles.Length; i++)
            {
                Rectangle sourceRect = sourceRectangles[i];
                // set origin to middle of the rectangle
                this.origins[i] = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            }
        }
        else
        {
            this.origins = origins;
        }

        if (layerDepths is null)
        {
            this.layerDepths = new float[frameTimers.Length];
            // layer depths
            for (int i = 0; i < this.layerDepths.Length; i++)
            {
                this.layerDepths[i] = 0;
            }
        }
        else
        {
            this.layerDepths = layerDepths;
        }

        animationEnabled = !isAnimationDisabled;
        AssignFrame();
    }

    public void SetFrame(int frameIndex)
    {
        currFrameIndex = frameIndex;

        currFrameTimer = 0;
        if (currFrameIndex >= frameTimers.Length)
        {
            currFrameIndex = currMinFrameIndex;
        }

        AssignFrame();
    }

    private void AssignFrame()
    {
        sourceRectangle = sourceRectangles[currFrameIndex];
        colorTint = colorTints is not null ? colorTints[currFrameIndex] : Color.White;
        origin = origins[currFrameIndex];
        layerDepth = layerDepths[currFrameIndex];

        onFrameChange?.Invoke(this, EventArgs.Empty);
    }

    public override void Update(GameTime gameTime)
    {
        if (!animationEnabled || isAnimationPaused) return;

        // update idle/movement sprite
        currFrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // If enough time has passed, advance to the next frame
        if (currFrameTimer >= frameTimers[currFrameIndex])
        {
            DisplayNextFrame();
        }
    }
}
