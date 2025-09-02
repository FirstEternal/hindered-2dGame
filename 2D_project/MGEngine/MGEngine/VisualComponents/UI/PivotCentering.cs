using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;

public class PivotCentering
{
    public enum Enum_Pivot
    {
        TopLeft,
        TopCenter,
        TopRight,
        Center,
        CenterLeft,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static void UpdatePivot(IResizableVisualComponent parentSprite, IResizableVisualComponent child, Transform childTransform, Enum_Pivot pivotPosition, Vector2? offSet = null)
    {
        Vector2 newLocalPosition = Vector2.Zero;
        switch (pivotPosition)
        {
            case Enum_Pivot.TopLeft:
                newLocalPosition = new Vector2(child.width / 2 - parentSprite.width / 2, child.height / 2 - parentSprite.height / 2);
                break;

            case Enum_Pivot.TopCenter:
                newLocalPosition = new Vector2(0, child.height / 2 - parentSprite.height / 2);
                break;
            case Enum_Pivot.TopRight:
                newLocalPosition = new Vector2(-child.width / 2 + parentSprite.width / 2, child.height / 2 - parentSprite.height / 2);
                break;
            case Enum_Pivot.CenterLeft:
                newLocalPosition = new Vector2(child.width / 2 - parentSprite.width / 2, 0);
                break;
            case Enum_Pivot.Center:
                newLocalPosition = Vector2.Zero;
                break;
            case Enum_Pivot.CenterRight:
                newLocalPosition = new Vector2(-child.width / 2 + parentSprite.width / 2, 0);
                break;
            case Enum_Pivot.BottomLeft:
                newLocalPosition = new Vector2(child.width / 2 - parentSprite.width / 2, -child.height / 2 + parentSprite.height / 2);
                break;
            case Enum_Pivot.BottomCenter:
                newLocalPosition = new Vector2(0, -child.height / 2 + parentSprite.height / 2);
                break;
            case Enum_Pivot.BottomRight:
                newLocalPosition = new Vector2(-child.width / 2 + parentSprite.width / 2, -child.height / 2 + parentSprite.height / 2);
                break;
        }

        // impossible possition
        childTransform.localPosition = newLocalPosition + offSet ?? Vector2.Zero;
    }

    internal static void UpdatePivot(Panel parentPanel, bool v, Transform transform, Enum_Pivot buttonPivot, Vector2 vector2)
    {
        throw new NotImplementedException();
    }
}