using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

internal class SpriteColliderGenerator
{
    public static SpriteColliderGenerator Instance;
    public SpriteColliderGenerator()
    {
        Instance = this;
    }

    public void CreateCollider(Texture2D sprite)
    {
        string colliderType = DetermineBestFitCollider(sprite);
        //Debug.WriteLine("Best-fit collider type: " + colliderType);
    }

    private string DetermineBestFitCollider(Texture2D sprite)
    {
        Rectangle bounds = GetBoundingBox(sprite);

        // Calculate fit errors for each collider type
        float circleError = CalculateCircleFitError(sprite, bounds);
        float rectangleError = CalculateRectangleFitError(sprite, bounds);
        float polygonError = CalculateConvexPolygonFitError(sprite, bounds, 5);

        // Normalize and weight errors
        float normalizedCircleError = circleError / (bounds.Width / 2f); // Normalize by radius
        float normalizedRectangleError = rectangleError; // Rectangle error is proportional
        float normalizedPolygonError = polygonError * 1.5f; // Weight polygon slightly higher

        // Add bias for rectangles when the bounding box is nearly square
        bool isNearlySquare = Math.Abs(bounds.Width - bounds.Height) < bounds.Width * 0.1f;
        if (isNearlySquare && normalizedRectangleError < normalizedCircleError * 1.2f)
        {
            return "Rectangle Collider"; // Prioritize rectangle for nearly square shapes
        }

        // Determine the best fit
        if (normalizedCircleError <= normalizedRectangleError && normalizedCircleError <= normalizedPolygonError)
            return "Circle Collider";
        else if (normalizedRectangleError <= normalizedPolygonError)
            return "Rectangle Collider";
        else
            return "Convex Polygon Collider";
    }


    private float CalculateCircleFitError(Texture2D sprite, Rectangle bounds)
    {
        int centerX = bounds.X + bounds.Width / 2;
        int centerY = bounds.Y + bounds.Height / 2;
        float radius = bounds.Width / 2f;

        Color[] pixels = new Color[sprite.Width * sprite.Height];
        sprite.GetData(pixels);

        float totalError = 0f;
        int totalChecked = 0;
        float squaredErrorSum = 0f;

        // Check pixel distances from the center
        for (int angle = 0; angle < 360; angle += 5) // Check every 5 degrees
        {
            float radians = MathHelper.ToRadians(angle);
            int x = (int)(centerX + radius * Math.Cos(radians));
            int y = (int)(centerY + radius * Math.Sin(radians));

            if (x >= 0 && y >= 0 && x < sprite.Width && y < sprite.Height)
            {
                bool isFilled = pixels[x + y * sprite.Width].A > 0;
                if (!isFilled) totalError += 1; // Error for missing pixel

                float actualDistance = GetDistanceToEdge(sprite, centerX, centerY, angle, bounds);
                float distanceError = Math.Abs(actualDistance - radius);
                squaredErrorSum += distanceError * distanceError;

                totalChecked++;
            }
        }

        float variance = squaredErrorSum / totalChecked;
        totalError += variance; // Add variance to total error
        return totalError / totalChecked;
    }

    // Helper to find the distance to the edge at a given angle
    private float GetDistanceToEdge(Texture2D sprite, int centerX, int centerY, float angle, Rectangle bounds)
    {
        Color[] pixels = new Color[sprite.Width * sprite.Height];
        sprite.GetData(pixels);

        float radians = MathHelper.ToRadians(angle);
        float dx = MathF.Cos(radians);
        float dy = MathF.Sin(radians);

        float distance = 0f;
        int x = centerX, y = centerY;
        while (x >= 0 && y >= 0 && x < sprite.Width && y < sprite.Height)
        {
            if (pixels[x + y * sprite.Width].A == 0) break;
            x = (int)(centerX + dx * distance);
            y = (int)(centerY + dy * distance);
            distance++;
        }

        return distance;
    }


    private float CalculateRectangleFitError(Texture2D sprite, Rectangle bounds)
    {
        Color[] pixels = new Color[sprite.Width * sprite.Height];
        sprite.GetData(pixels);

        int totalPixels = bounds.Width * bounds.Height;
        int filledPixels = 0;

        for (int y = bounds.Y; y < bounds.Y + bounds.Height; y++)
        {
            for (int x = bounds.X; x < bounds.X + bounds.Width; x++)
            {
                if (pixels[x + y * sprite.Width].A > 0) // Non-transparent pixel
                {
                    filledPixels++;
                }
            }
        }

        // Error is based on how much of the rectangle is not filled
        return 1f - ((float)filledPixels / totalPixels);
    }

    private float CalculateConvexPolygonFitError(Texture2D sprite, Rectangle bounds, int maxVertices)
    {
        ConvexPolygon polygon = ExtractConvexPolygon(sprite, bounds, maxVertices);

        // Approximation error: check how well the sprite's outline matches the polygon
        // (In a real implementation, calculate the deviation of the sprite's edges from the polygon edges)
        return polygon.Vertices.Count > maxVertices ? float.MaxValue : 0.2f; // Simplified placeholder
    }

    private Rectangle GetBoundingBox(Texture2D sprite)
    {
        Color[] pixels = new Color[sprite.Width * sprite.Height];
        sprite.GetData(pixels);

        int minX = sprite.Width, maxX = 0, minY = sprite.Height, maxY = 0;

        for (int y = 0; y < sprite.Height; y++)
        {
            for (int x = 0; x < sprite.Width; x++)
            {
                if (pixels[x + y * sprite.Width].A > 0) // Non-transparent pixel
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    private ConvexPolygon ExtractConvexPolygon(Texture2D sprite, Rectangle bounds, int maxVertices)
    {
        // Placeholder implementation for convex polygon extraction
        // In a real scenario, you could use a convex hull algorithm (e.g., Graham's scan)
        ConvexPolygon polygon = new ConvexPolygon([
            new Vector2(bounds.X, bounds.Y),
            new Vector2(bounds.X + bounds.Width, bounds.Y),
            new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height)
        ]);

        /*
        List<Vector2> polygon = new List<Vector2>
        {
            new Vector2(bounds.X, bounds.Y),
            new Vector2(bounds.X + bounds.Width, bounds.Y),
            new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height)
        };*/

        return polygon;
    }
}
