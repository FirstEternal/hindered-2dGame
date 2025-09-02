using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class RotatedRectangle
{
    public Vector2 Position { get; set; }
    public Vector2 Scale { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Rotation { get; set; } // Rotation in radians

    public RotatedRectangle(Vector2 position, Vector2 scale, float width, float height, float rotation)
    {
        Position = position;
        Scale = scale;
        Width = width;
        Height = height;
        Rotation = rotation;
    }

    public static void GetBoundingBox(float width, float height, Vector2 scale, float rotation, Vector2 position, out double left, out double right, out double top, out double bottom)
    {
        // Precompute sine and cosine
        double cosTheta = Math.Cos(rotation);
        double sinTheta = Math.Sin(rotation);

        // Define the rectangle's corners
        double[] dx = { -width / 2, width / 2, width / 2, -width / 2 };
        double[] dy = { -height / 2, -height / 2, height / 2, height / 2 };

        // Rotate the corners
        double[] rotatedX = new double[4];
        double[] rotatedY = new double[4];
        for (int i = 0; i < 4; i++)
        {
            rotatedX[i] = position.X + dx[i] * cosTheta - dy[i] * sinTheta;
            rotatedY[i] = position.Y + dx[i] * sinTheta + dy[i] * cosTheta;
        }

        // Find the bounding box
        left = Math.Min(Math.Min(rotatedX[0], rotatedX[1]), Math.Min(rotatedX[2], rotatedX[3]));
        right = Math.Max(Math.Max(rotatedX[0], rotatedX[1]), Math.Max(rotatedX[2], rotatedX[3]));
        top = Math.Min(Math.Min(rotatedY[0], rotatedY[1]), Math.Min(rotatedY[2], rotatedY[3]));
        bottom = Math.Max(Math.Max(rotatedY[0], rotatedY[1]), Math.Max(rotatedY[2], rotatedY[3]));


    }

    // Calculate the four corners of the rotated rectangle
    public Vector2[] GetCorners()
    {
        Vector2[] corners = new Vector2[4];

        // Half width and height
        float halfWidth = Width * Scale.X / 2;
        float halfHeight = Height * Scale.Y / 2;

        // Calculate corners relative to the center
        corners[0] = new Vector2(-halfWidth, -halfHeight);
        corners[1] = new Vector2(halfWidth, -halfHeight);
        corners[2] = new Vector2(halfWidth, halfHeight);
        corners[3] = new Vector2(-halfWidth, halfHeight);

        // Rotate and translate corners
        for (int i = 0; i < 4; i++)
        {
            corners[i] = RotatePoint(corners[i], Rotation) + Position;
        }

        return corners;
    }

    // Rotate a point around the origin by a given angle
    private Vector2 RotatePoint(Vector2 point, float angle)
    {
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);

        return new Vector2(
            point.X * cos - point.Y * sin,
            point.X * sin + point.Y * cos
        );
    }

    // Check if two rotated rectangles collide using Separating Axis Theorem (SAT)
    public static bool RotatedRectangleCollision(RotatedRectangle rect1, RotatedRectangle rect2)
    {
        Vector2[] rect1Corners = rect1.GetCorners();
        Vector2[] rect2Corners = rect2.GetCorners();

        // Check each axis (edge) of rect1 and rect2
        Vector2[] axes = GetAxes(rect1Corners, rect2Corners);

        foreach (var axis in axes)
        {
            if (!IsProjectionOverlap(rect1Corners, rect2Corners, axis))
            {
                // If we find an axis where projections don't overlap, the rectangles are not colliding
                return false;
            }
        }

        // All projections overlap, the rectangles are colliding
        return true;
    }

    private static Vector2[] GetAxes(Vector2[] rect1Corners, Vector2[] rect2Corners)
    {
        // Get perpendicular axes to the edges of both rectangles
        Vector2[] axes = new Vector2[4];

        // First two axes from rect1, next two from rect2
        axes[0] = Perpendicular(rect1Corners[1] - rect1Corners[0]);
        axes[1] = Perpendicular(rect1Corners[3] - rect1Corners[0]);
        axes[2] = Perpendicular(rect2Corners[1] - rect2Corners[0]);
        axes[3] = Perpendicular(rect2Corners[3] - rect2Corners[0]);

        return axes;
    }

    private static Vector2 Perpendicular(Vector2 edge)
    {
        return new Vector2(-edge.Y, edge.X);
    }

    private static bool IsProjectionOverlap(Vector2[] rect1Corners, Vector2[] rect2Corners, Vector2 axis)
    {
        // Project both rectangles onto the axis
        float rect1Min, rect1Max, rect2Min, rect2Max;

        ProjectRectangle(rect1Corners, axis, out rect1Min, out rect1Max);
        ProjectRectangle(rect2Corners, axis, out rect2Min, out rect2Max);

        // Check if projections overlap
        return !(rect1Max < rect2Min || rect2Max < rect1Min);
    }

    private static void ProjectRectangle(Vector2[] corners, Vector2 axis, out float min, out float max)
    {
        min = Vector2.Dot(corners[0], axis);
        max = min;

        for (int i = 1; i < corners.Length; i++)
        {
            float projection = Vector2.Dot(corners[i], axis);

            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }
    public void DrawOutlinePixels(SpriteBatch spriteBatch, Texture2D pixelTexture, Color color)
    {
        // Get the corners of the rotated rectangle
        Vector2[] corners = GetCorners();

        // Loop through each pair of corners to draw the edges
        for (int i = 0; i < corners.Length; i++)
        {
            // Get the start and end points for the current edge
            Vector2 start = corners[i];
            Vector2 end = corners[(i + 1) % corners.Length]; // Wrap around for the last edge

            // Draw pixels along the line between start and end
            DrawLinePixels(spriteBatch, pixelTexture, start, end, color);
        }
    }

    // Helper method to draw pixels along a line between two points
    private void DrawLinePixels(SpriteBatch spriteBatch, Texture2D pixelTexture, Vector2 start, Vector2 end, Color color)
    {
        // Calculate differences and steps for Bresenham's algorithm
        int x0 = (int)start.X;
        int y0 = (int)start.Y;
        int x1 = (int)end.X;
        int y1 = (int)end.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Draw pixel at the current position
            spriteBatch.Draw(pixelTexture, new Vector2(x0, y0), color);

            // Check if we've reached the end point
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }




    /*
    // PIXEL PERFECT COLLISION
    // Get the pixel data from a Texture2D
    public static Color[] GetTexturePixels(Texture2D texture)
    {
        Color[] pixelData = new Color[texture.Width * texture.Height];
        texture.GetData(pixelData);  // Extract pixel data from the texture
        
        return pixelData;
    }

    // Transform a point from texture space to world space
    public static Vector2 TransformPoint(Vector2 localPoint, Vector2 position, float rotation, Vector2 scale, Vector2 origin)
    {
        // Step 1: Translate the point to the origin (e.g., the center of the sprite)
        Vector2 pointRelativeToOrigin = localPoint - origin;

        // Step 2: Apply scaling to the point
        Vector2 scaledPoint = pointRelativeToOrigin * scale;

        // Step 3: Apply rotation around the origin
        float cos = (float)Math.Cos(rotation);
        float sin = (float)Math.Sin(rotation);

        Vector2 rotatedPoint = new Vector2(
            scaledPoint.X * cos - scaledPoint.Y * sin,
            scaledPoint.X * sin + scaledPoint.Y * cos
        );

        // Step 4: Translate back and add the sprite's position in world space
        return rotatedPoint + position;
    }

    public static bool PixelPerfectCollision(
    Texture2D texture1, Vector2 position1, float rotation1, Vector2 scale1, Vector2 origin1,
    Texture2D texture2, Vector2 position2, float rotation2, Vector2 scale2, Vector2 origin2)
    {
        // Get pixel data for both textures
        Color[] pixels1 = GetTexturePixels(texture1);
        Color[] pixels2 = GetTexturePixels(texture2);

        // Loop through the pixels of the first texture
        for (int x1 = 0; x1 < texture1.Width; x1++)
        {
            for (int y1 = 0; y1 < texture1.Height; y1++)
            {
                // Get the local texture point in texture1
                Vector2 localPoint1 = new Vector2(x1, y1);

                // Transform the point to world space for texture1
                Vector2 worldPoint1 = TransformPoint(localPoint1, position1, rotation1, scale1, origin1);

                if (IsPointInsideTexture(worldPoint1, texture2, position2, rotation2, scale2, out Vector2 localPoint2))
                {
                    // Get the pixel color in the first and second texture
                    Color color1 = pixels1[x1 + y1 * texture1.Width];
                    Color color2 = pixels2[(int)localPoint2.X + (int)localPoint2.Y * texture2.Width];

                    // If both pixels are not transparent, we have a collision
                    if (color1.A > 0 && color2.A > 0)
                    {
                        return true; // Collision found
                    }
                }
                /*
                // Loop through pixels of the second texture
                for (int x2 = 0; x2 < texture2.Width; x2++)
                {
                    for (int y2 = 0; y2 < texture2.Height; y2++)
                    {
                        // Get the local texture point in texture2
                        Vector2 localPoint2 = new Vector2(x2, y2);

                        // Transform the point to world space for texture2
                        Vector2 worldPoint2 = TransformPoint(localPoint2, position2, rotation2, scale2, origin2);

                        // Check if the two world points are very close (i.e., they "collide" in world space)
                        if (Vector2.Distance(worldPoint1, worldPoint2) < 1.0f) // Pixel-perfect precision
                        {
                            // Get the pixel color in the first and second texture
                            Color color1 = pixels1[x1 + y1 * texture1.Width];
                            Color color2 = pixels2[x2 + y2 * texture2.Width];

                            // If both pixels are not transparent, we have a collision
                            if (color1.A > 0 && color2.A > 0)
                            {
                                return true;
                            }
                        }
                    }
                }*/
    /*
            }
        }

        // No collision detected
        return false;
    }


    // Check if a world point lies inside the bounds of texture2
    public static bool IsPointInsideTexture(Vector2 worldPoint, Texture2D texture, Vector2 position, float rotation, Vector2 scale, out Vector2 localPoint)
    {
        // Translate the world point back to texture space by applying the inverse transformations
        Vector2 relativePoint = worldPoint - position;

        // Inverse rotation
        float cos = (float)Math.Cos(-rotation);
        float sin = (float)Math.Sin(-rotation);

        Vector2 rotatedPoint = new Vector2(
            relativePoint.X * cos - relativePoint.Y * sin,
            relativePoint.X * sin + relativePoint.Y * cos
        );

        // Inverse scale
        localPoint = rotatedPoint / scale;

        // Check if the point is within the texture's bounds
        return localPoint.X >= 0 && localPoint.X < texture.Width && localPoint.Y >= 0 && localPoint.Y < texture.Height;
    }
    
    /*
    public static void DrawPixels(
   Texture2D texture1, Vector2 position1, float rotation1, Vector2 scale1)
    {
        // Get pixel data for both textures
        Color[] pixels1 = GetTexturePixels(texture1);

        // Loop through the pixels of the first texture
        for (int x1 = 0; x1 < texture1.Width; x1++)
        {
            for (int y1 = 0; y1 < texture1.Height; y1++)
            {
                // Get the local texture point in texture1
                Vector2 localPoint1 = new Vector2(x1, y1);

                // Transform the point to world space
                Vector2 worldPoint1 = TransformPoint(localPoint1, position1, rotation1, scale1, Game2DPlatformer.Instance.pixelOrigin);

                // Get the pixel color in the first and second texture
                Color color1 = pixels1[x1 + y1 * texture1.Width];
                if (color1.A > 0)
                {
                    Game2DPlatformer.Instance.spriteBatch.Draw(Game2DPlatformer.Instance.pixelTexture, worldPoint1, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                }
                else
                {
                    Game2DPlatformer.Instance.spriteBatch.Draw(Game2DPlatformer.Instance.pixelTexture, worldPoint1, null, Color.Blue, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                }
            }
        }
        
        // No collision detected
        //return false;
    }*/

    /*
    // MARCHING SQUARES
    public static bool PixelPerfectCollision1(
   Texture2D texture1, Vector2 position1, float rotation1, Vector2 scale1, Vector2 origin1,
   Texture2D texture2, Vector2 position2, float rotation2, Vector2 scale2, Vector2 origin2)
    {

        // Step 1: Detect edge pixels using Marching Squares
        List<Point> edgePixels1 = MarchingSquares.DetectEdges(texture1);
        List<Point> edgePixels2 = MarchingSquares.DetectEdges(texture2);

        // Step 2: Transform edge pixels to world coordinates
        for (int i = 0; i < edgePixels1.Count; i++)
        {
            // Get the local texture point in texture1
            Vector2 localPoint1 = new Vector2(edgePixels1[i].X, edgePixels1[i].Y);
            Vector2 worldPoint1 = TransformPoint(localPoint1, position1, rotation1, scale1, origin1);

            edgePixels1[i] = new Point((int)Math.Round(worldPoint1.X), (int)Math.Round(worldPoint1.Y));
        }

        for (int i = 0; i < edgePixels2.Count; i++)
        {
            // Get the local texture point in texture2
            Vector2 localPoint2 = new Vector2(edgePixels2[i].X, edgePixels2[i].Y);
            Vector2 worldPoint2 = TransformPoint(localPoint2, position2, rotation2, scale2, origin2);

            edgePixels2[i] = new Point((int)Math.Round(worldPoint2.X), (int)Math.Round(worldPoint2.Y));
        }

        for (int i = 0; i < edgePixels1.Count; i++)
        {
            Point closestEdge = MarchingSquares.FindClosestEdge(edgePixels1[i], edgePixels2);
        }

        // Step 3: Check for collisions
        foreach (Point edgePixel1 in edgePixels1)
        {
            // Check if the edge pixel is within the bounds of texture2
            if (edgePixel1.X >= 0 && edgePixel1.X < texture2.Width && edgePixel1.Y >= 0 && edgePixel1.Y < texture2.Height)
            {
                // Get the color of the pixel in texture2 at the transformed edge pixel position
                Color[] pixels2 = new Color[texture2.Width * texture2.Height];
                texture2.GetData(pixels2);
                Color color2 = pixels2[edgePixel1.X + edgePixel1.Y * texture2.Width];

                // Check if the pixel in texture2 is not transparent
                if (color2.A > 0)
                {
                    // Collision detected
                    return true;
                }
            }
        }

        // No collision detected
        return false;
    }
    
    internal static void DrawPixelEdges(
  Texture2D texture1, Vector2 position1, float rotation1, Vector2 scale1, Vector2 origin1)
    {

        // Step 1: Detect edge pixels using Marching Squares
        List<Point> edgePixels1 = MarchingSquares.DetectEdges(texture1);

        Color[] pixels1 = GetTexturePixels(texture1);

        // Step 2: Transform edge pixels to world coordinates
        Vector2 localPoint1;
        Vector2 worldPoint1;
        Color color1;

        Texture2D pixelTexture;

        // Create a 1x1 pixel texture
        pixelTexture = new Texture2D(Game2DPlatformer.Instance.GraphicsDevice, 1, 1);

        // Set the color of the pixel (e.g., white)
        pixelTexture.SetData(new[] { Color.White });

        for (int i = 0; i < edgePixels1.Count; i++)
        {
            // Get the local texture point in texture1
            localPoint1 = new Vector2(edgePixels1[i].X, edgePixels1[i].Y);
            worldPoint1 = TransformPoint(localPoint1, position1, rotation1, scale1, origin1);

            edgePixels1[i] = new Point((int)Math.Round(worldPoint1.X), (int)Math.Round(worldPoint1.Y));

            color1 = pixels1[(int) (localPoint1.X + localPoint1.Y * texture1.Width)];

            if (color1.A > 0)
            {
                Game2DPlatformer.Instance.spriteBatch.Draw(pixelTexture, worldPoint1, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }*/

}
