using Microsoft.Xna.Framework;

public class OBBRectangleCollider(float width, float height, bool isAftermath, bool isRelaxPosition = true, bool isEnergyExchange = false) : AARectangleCollider(width, height, isAftermath)
{
    /*
    public static List<Vector2> GetTransformedOBBVertices(OBBRectangleCollider obb)
    {
        Vector2 offset = obb.gameObject.transform.globalPosition;
        float angle = obb.gameObject.transform.globalRotationAngle;
        Matrix transformMatrix = Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(offset.X, offset.Y, 0);

        List<Vector2> vertices = new List<Vector2>(4);
        // OBB vertices (4 vertices forming a rotated rectangle)
        Vector2 topLeft = new Vector2(-obb.Width / 2, -obb.Height / 2);
        Vector2 topRight = new Vector2(obb.Width / 2, -obb.Height / 2);
        Vector2 bottomLeft = new Vector2(-obb.Width / 2, obb.Height / 2);
        Vector2 bottomRight = new Vector2(obb.Width / 2, obb.Height / 2);

        vertices.Add(Vector2.Transform(topLeft, transformMatrix));
        vertices.Add(Vector2.Transform(topRight, transformMatrix));
        vertices.Add(Vector2.Transform(bottomLeft, transformMatrix));
        vertices.Add(Vector2.Transform(bottomRight, transformMatrix));

        return vertices;
    }*/

    public static Vector2[] GetRotatedRectangleVertices(OBBRectangleCollider rect)
    {
        Vector2 center = rect.gameObject.transform.globalPosition;
        float angle = rect.gameObject.transform.globalRotationAngle;
        float halfWidth = rect.Width / 2;
        float halfHeight = rect.Height / 2;

        Vector2[] localVertices = new Vector2[]
        {
            new Vector2(-halfWidth, -halfHeight),
            new Vector2(halfWidth, -halfHeight),
            new Vector2(halfWidth, halfHeight),
            new Vector2(-halfWidth, halfHeight)
        };

        Matrix rotationMatrix = Matrix.CreateRotationZ(angle);
        for (int i = 0; i < localVertices.Length; i++)
        {
            localVertices[i] = Vector2.Transform(localVertices[i], rotationMatrix) + center;
        }

        return localVertices;
    }
}
