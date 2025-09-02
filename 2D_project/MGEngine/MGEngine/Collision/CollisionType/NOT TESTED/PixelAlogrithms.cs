using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class PixelAlogrithms
{
    // Get the pixel data from a Texture2D
    public static Color[] GetTexturePixels(Texture2D texture)
    {
        Color[] pixelData = new Color[texture.Width * texture.Height];
        texture.GetData(pixelData);  // Extract pixel data from the texture

        return pixelData;
    }

    /*
    public static bool PixelPerfectCollision(SpriteCollider spriteColliderA, SpriteCollider spriteColliderB)
    {


        // find all visible pixels in image check if there is collision between visible pixels
        Transform t = spriteColliderA.gameObject.transform;
        Sprite sprite = spriteColliderA.gameObject.GetComponent<Sprite>();
        Color[] pixelData = new Color[sprite.texture2D?.Width ?? 0 * sprite.texture2D?.Height ?? 0];
      
        foreach (Vector2 worldPointA in spriteColliderA.visibleWorldPointPixels)
        {
            foreach (Vector2 worldPointB in spriteColliderB.visibleWorldPointPixels)
            {
                if (Vector2.Distance(worldPointA, worldPointB) < 5)
                {
                    // collision detected
                    return true;
                }
            }
            /*
            // Calculate the 1D index for the pixel
            int index = (int)(worldPointA.Y * sprite.texture2D.Width + worldPointA.X);

            // Change the color of the specific pixel
            pixelData[index] = Color.Red; // Color it red

            // Update the texture with the modified pixel data
            sprite.texture2D.SetData(pixelData);
            if (t.position.X >= 0 && t.position.X < t.scale.X * sprite.texture2D.Width && t.position.Y >= 0 && t.position.Y < t.scale.Y * sprite.texture2D.Height)
            {
                
                Debug.WriteLine("weird");
                // Calculate the 1D index for the pixel
                int index = (int)(worldPointA.Y * sprite.texture2D.Width + worldPointA.X);

                // Change the color of the specific pixel
                pixelData[index] = Color.Red; // Color it red

                // Update the texture with the modified pixel data
                sprite.texture2D.SetData(pixelData);
                
                foreach (Vector2 worldPointB in spriteColliderB.visibleWorldPointPixels)
                {
                    if(worldPointA == worldPointB)
                    {
                        // collision detected
                        return true;
                    }
                }
            }*//*

        }
        // no collision detected
        return false;
    }*/




    public class MarchingSquares
    {
        // Helper function to check if a pixel is transparent
        private static bool IsTransparent(Color pixel)
        {
            return pixel.A == 0;
        }

        // Method to apply Marching Squares and detect edge pixels
        public static List<Point> DetectEdges(Texture2D texture)
        {
            List<Point> edgePixels = new List<Point>();

            // Get the pixel data from the texture
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);

            // Iterate through each 2x2 block in the texture
            for (int x = 0; x < texture.Width - 1; x++)
            {
                for (int y = 0; y < texture.Height - 1; y++)
                {
                    // Get the four pixels in the 2x2 block
                    Color topLeft = pixels[x + y * texture.Width];
                    Color topRight = pixels[(x + 1) + y * texture.Width];
                    Color bottomLeft = pixels[x + (y + 1) * texture.Width];
                    Color bottomRight = pixels[(x + 1) + (y + 1) * texture.Width];

                    // Determine the index for the current 2x2 block based on transparency
                    int blockType = 0;
                    if (!IsTransparent(topLeft)) blockType |= 1;
                    if (!IsTransparent(topRight)) blockType |= 2;
                    if (!IsTransparent(bottomLeft)) blockType |= 4;
                    if (!IsTransparent(bottomRight)) blockType |= 8;

                    // If the block has both inside and outside pixels, it's an edge block
                    if (blockType > 0 && blockType < 15)
                    {
                        // Store the top-left pixel of the block as an edge pixel
                        edgePixels.Add(new Point(x, y));
                    }
                }
            }

            return edgePixels;
        }

        // Method to find the closest edge pixel to a given point
        public static Point FindClosestEdge(Point point, List<Point> edgePixels)
        {
            Point closestEdge = edgePixels[0];
            float minDistance = Vector2.Distance(new Vector2(point.X, point.Y), new Vector2(closestEdge.X, closestEdge.Y));

            foreach (Point edgePixel in edgePixels)
            {
                float distance = Vector2.Distance(new Vector2(point.X, point.Y), new Vector2(edgePixel.X, edgePixel.Y));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEdge = edgePixel;
                }
            }

            return closestEdge;
        }
    }


}

