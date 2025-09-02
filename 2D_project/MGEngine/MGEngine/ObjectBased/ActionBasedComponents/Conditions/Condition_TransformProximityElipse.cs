using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

public class Condition_TransformProximityElipse(bool isWithinRange, float x, float y, Transform centerTransformA, Transform transformB) : ICondition
{
    private readonly Transform centerTransformA = centerTransformA;
    private readonly Transform transformB = transformB;
    private readonly float withinRangeX = x;
    private readonly float withinRangeY = y;
    public event EventHandler? OnConditionMet;

    public void Update(GameTime gameTime)
    {
        bool isInsideElipse;
        // check if any transform is rotated
        if (centerTransformA.globalRotationAngle != 0)
        {
            isInsideElipse = IsWithinRotatedEllipse(centerTransformA, withinRangeX, withinRangeY, centerTransformA.globalRotationAngle, transformB);
        }
        else
        {
            isInsideElipse = IsWithingElipse(centerTransformA, withinRangeX, withinRangeY, transformB);
        }

        if ((isWithinRange && isInsideElipse) || (!isWithinRange && !isInsideElipse))
        {
            OnConditionMet?.Invoke(this, EventArgs.Empty);
        }
    }

    public static bool IsWithingElipse(Transform centerTransform, float radiusX, float radiusY, Transform pointTransform)
    {
        float normalizedX = (pointTransform.globalPosition.X - centerTransform.globalPosition.X) / radiusX;
        float normalizedY = (pointTransform.globalPosition.Y - centerTransform.globalPosition.Y) / radiusY;

        // Check if the point lies within the ellipse
        return (normalizedX * normalizedX + normalizedY * normalizedY) <= 1;
    }

    bool IsWithinRotatedEllipse(Transform centerTransform, float radiusX, float radiusY, float rotationAngle, Transform pointTransform)
    {
        // Get positions
        float px = pointTransform.globalPosition.X;
        float py = pointTransform.globalPosition.Y;

        float cx = centerTransform.globalPosition.X;
        float cy = centerTransform.globalPosition.Y;

        // Translate the point to the ellipse's local space
        float dx = px - cx;
        float dy = py - cy;

        // Calculate the rotated coordinates
        float cosTheta = MathF.Cos(rotationAngle);
        float sinTheta = MathF.Sin(rotationAngle);

        float rotatedX = dx * cosTheta + dy * sinTheta;
        float rotatedY = dx * sinTheta - dy * cosTheta;

        // Check if the rotated point is within the ellipse
        return (rotatedX * rotatedX) / (radiusX * radiusX) + (rotatedY * rotatedY) / (radiusY * radiusY) <= 1;
    }
}
