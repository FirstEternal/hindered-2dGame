using Microsoft.Xna.Framework;

public class AAHalfPlane : HalfPlane
{
    protected AxisDirection _direction;

    public AAHalfPlane(AxisDirection theDirection, float theDistance)
    {
        _distance = theDistance;
        _direction = theDirection;

        switch (theDirection)
        {
            default:
            case AxisDirection.PositiveX:
                _normal = Vector2.UnitX;
                break;
            case AxisDirection.NegativeX:
                _normal = -Vector2.UnitX;
                break;
            case AxisDirection.PositiveY:
                _normal = Vector2.UnitY;
                break;
            case AxisDirection.NegativeY:
                _normal = -Vector2.UnitY;
                break;
        }
    }

    public AxisDirection Direction
    {
        get => _direction;
        set
        {
            switch (value)
            {
                default:
                case AxisDirection.PositiveX:
                    _normal = Vector2.UnitX;
                    break;
                case AxisDirection.NegativeX:
                    _normal = -Vector2.UnitX;
                    break;
                case AxisDirection.PositiveY:
                    _normal = Vector2.UnitY;
                    break;
                case AxisDirection.NegativeY:
                    _normal = -Vector2.UnitY;
                    break;
            }

        }
    }

    void SetNormal(Vector2 value)
    {
        if ((value.X == 0 && value.Y == 0) || (value.X != 0 && value.Y != 0))
        {
            throw new Exception("Axis aligned half plane requires an axis aligned normal");
        }

        Normal = value;
        if (value.X > 0)
        {
            _direction = AxisDirection.PositiveX;
        }
        else if (value.X < 0)
        {
            _direction = AxisDirection.NegativeX;
        }
        else if (value.Y > 0)
        {
            _direction = AxisDirection.PositiveY;
        }
        else if (value.Y < 0)
        {
            _direction = AxisDirection.NegativeY;
        }

    }
}