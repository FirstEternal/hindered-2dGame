using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class SpriteTiled : Sprite
{
    public Rectangle baseSourceRect;
    public Rectangle midSourceRect;
    public Rectangle tipSourceRect;

    // Arrays for configuration per part (no animation, just static assignment)
    protected Color[]? colorTints;
    protected Vector2[] origins;
    protected float[] layerDepths;

    public float targetLength; // total length the sprite should cover
    public float maxLength = GameWindow.Instance.windowWidth * 0.8f;

    /// <summary>
    /// sourceRectangles are as follow 0: base, 1: extendable middle, 2: tip
    /// </summary>
    public SpriteTiled(
        Texture2D texture,
        Rectangle[] sourceRectangles,
        Color[]? colorTints = null,
        Vector2[]? origins = null,
        float[]? layerDepths = null
    ) : base(texture, Color.White)
    {
        baseSourceRect = sourceRectangles[0];
        midSourceRect = sourceRectangles[1];
        tipSourceRect = sourceRectangles[2];

        // Assign arrays or defaults
        this.colorTints = colorTints ?? new Color[] { Color.White, Color.White, Color.White };
        this.origins = origins ?? new Vector2[]
        {
            new Vector2(0, baseSourceRect.Height / 2f), // left-center by default
            new Vector2(0, midSourceRect.Height / 2f),
            new Vector2(0, tipSourceRect.Height / 2f)
        };
        this.layerDepths = layerDepths ?? new float[] { 0.1f, 0.1f, 0.1f };
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Transform? transform = gameObject?.transform;
        if (transform is null) return;

        Vector2 scale = GetScale();
        float length = Math.Min(targetLength, maxLength);

        // final angle: include transform global + local rotation (if any)
        float angle = transform.globalRotationAngle;

        // direction along which we advance tiles (based on final angle)
        Vector2 dir = new((float)Math.Cos(angle), (float)Math.Sin(angle));

        // Precompute adjusted origins (they're used both for drawing and for spacing)
        Vector2 adjBaseOrigin = GetAdjustedOrigin(baseSourceRect, 0);
        Vector2 adjMidOrigin = GetAdjustedOrigin(midSourceRect, 1);
        Vector2 adjTipOrigin = GetAdjustedOrigin(tipSourceRect, 2);

        // Compute forward distances (from origin to right edge) in world units (scaled)
        float baseForward = (baseSourceRect.Width - adjBaseOrigin.X) * scale.X;
        float midForward = (midSourceRect.Width - adjMidOrigin.X) * scale.X;
        float tipForward = (tipSourceRect.Width - adjTipOrigin.X) * scale.X;

        // safety in case of weird origins
        if (baseForward < 0f) baseForward = baseSourceRect.Width * scale.X - (adjBaseOrigin.X * scale.X);
        if (midForward < 0f) midForward = midSourceRect.Width * scale.X - (adjMidOrigin.X * scale.X);
        if (tipForward < 0f) tipForward = tipSourceRect.Width * scale.X - (adjTipOrigin.X * scale.X);

        // how many mids fit between base and tip given desired length?
        // we measure length from base origin outward to the tip's right-edge
        float minNeeded = baseForward + tipForward;
        int numMids = 0;
        if (minNeeded < length)
        {
            // if midForward <= 0 fallback to dividing by mid width
            float midStep = midForward > 0 ? midForward : midSourceRect.Width * scale.X;
            numMids = (int)Math.Floor((length - minNeeded) / midStep);
            if (numMids < 0) numMids = 0;
        }

        // starting draw position (base origin world position)
        Vector2 pos = transform.globalPosition;

        // --- draw base at pos ---
        DrawSegment(spriteBatch, baseSourceRect, pos, angle, 0, adjBaseOrigin);

        // compute the world position of the right edge of the base
        Vector2 prevRight = pos + dir * baseForward;

        // place and draw mids: we want the left-edge of the mid to equal prevRight.
        // the mid is drawn at p_mid such that:
        // left_edge_world_of_mid = p_mid + dir * (-adjMidOrigin.X * scale.X)  == prevRight
        // => p_mid = prevRight + dir * (adjMidOrigin.X * scale.X)
        for (int i = 0; i < numMids; i++)
        {
            Vector2 pMid = prevRight + dir * (adjMidOrigin.X * scale.X);
            DrawSegment(spriteBatch, midSourceRect, pMid, angle, 1, adjMidOrigin);

            // update prevRight to the right edge of this mid
            prevRight = pMid + dir * (midSourceRect.Width - adjMidOrigin.X) * scale.X;
        }

        // finally, place tip so its left edge matches prevRight
        Vector2 pTip = prevRight + dir * (adjTipOrigin.X * scale.X);
        DrawSegment(spriteBatch, tipSourceRect, pTip, angle, 2, adjTipOrigin);
    }

    // compute the adjusted origin used when drawing (account for spriteEffects flips)
    private Vector2 GetAdjustedOrigin(Rectangle src, int index)
    {
        Vector2 o = origins[index];

        if (spriteEffects.HasFlag(SpriteEffects.FlipHorizontally))
            o.X = src.Width - o.X;
        if (spriteEffects.HasFlag(SpriteEffects.FlipVertically))
            o.Y = src.Height - o.Y;

        return o;
    }

    // DrawSegment that accepts an already computed adjusted origin
    private void DrawSegment(SpriteBatch spriteBatch, Rectangle src, Vector2 pos, float angle, int index, Vector2 adjustedOrigin)
    {
        Color tint = colorTints != null ? colorTints[index] : Color.White;
        float layer = layerDepths[index];

        spriteBatch.Draw(
            texture2D,
            pos,
            src,
            tint,
            angle,
            adjustedOrigin,
            GetScale(),
            spriteEffects,
            layer
        );
    }
}
