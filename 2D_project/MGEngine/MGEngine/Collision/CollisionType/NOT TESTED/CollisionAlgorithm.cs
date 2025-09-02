using Microsoft.Xna.Framework;

public class CollisionAlgorithm<T1, T2>
{
    protected CollisionAlgorithm()
    {
    }

    public virtual void CollisionBetween(T1 item1, T2 item2)
    {
    }

    protected virtual bool DetectCollision(T1 item1, T2 item2)
    {
        return false;
    }

    protected virtual void ResolveCollision(T1 item1, T2 item2)
    {
    }

    /*
    protected bool ShouldResolveCollision(object item1, object item2)
    {
        CustomCollider customCollider1 = item1 as CustomCollider;
        CustomCollider customCollider2 = item2 as CustomCollider;
        bool result = true;
        if (customCollider1 is not null)
        {
            result &= customCollider1.CollidingWith(item2, true);
        }

        if (customCollider2 is not null)
        {
            result &= customCollider2.CollidingWith(item1, result);
        }

        return result;
        return false;
    }
    */

    protected void RelaxCollision(PhysicsComponent physicsComponent1, PhysicsComponent physicsComponent2, Vector2 relaxDistance)
    {
        float relaxPercentage1 = 0.5f;
        float relaxPercentage2 = 0.5f;
        if (physicsComponent1 is not null && physicsComponent2 is not null)
        {
            float mass1 = physicsComponent1.Mass;
            float mass2 = physicsComponent2.Mass;
            relaxPercentage1 = mass2 / (mass1 + mass2);
            relaxPercentage2 = mass1 / (mass1 + mass2);
        }
        else if (physicsComponent1 is not null)
        {
            relaxPercentage1 = 1;
            relaxPercentage2 = 0;
        }
        else if (physicsComponent2 is not null)
        {
            relaxPercentage1 = 0;
            relaxPercentage2 = 1;
        }
        /*
        else
        {
            if (itemWithPosition1 is not null && itemWithPosition2 is null)
            {
                relaxPercentage1 = 1;
                relaxPercentage2 = 0;
            }
            else if (itemWithPosition1 is null && itemWithPosition2 is not null)
            {
                relaxPercentage1 = 0;
                relaxPercentage2 = 1;
            }
        }

        if (itemWithPosition1 is not null)
        {
            itemWithPosition1.Position -= relaxDistance * relaxPercentage1;
        }

        if (itemWithPosition2 is not null)
        {
            itemWithPosition2.Position += relaxDistance * relaxPercentage2;
        }
        */
        if (physicsComponent1 is not null) physicsComponent1.gameObject.transform.globalPosition -= relaxDistance * relaxPercentage1;
        if (physicsComponent2 is not null) physicsComponent2.gameObject.transform.globalPosition -= relaxDistance * relaxPercentage2;
    }

    /*
    protected void ReportCollision(object item1, object item2)
    {
        CustomCollider customCollider1 = item1 as CustomCollider;
        CustomCollider customCollider2 = item2 as CustomCollider;
        
        customCollider1?.CollidedWith(item2);
        customCollider2?.CollidedWith(item1);
    }*/
    /*
    protected void ExchangeEnergy(object item1, object item2, Vector2 collisionNormal)
    {
        IVelocity itemWithVelocity1 = item1 as IVelocity;
        IVelocity itemWithVelocity2 = item2 as IVelocity;
        
        float speed1 = itemWithVelocity1 is not null ? Vector2.Dot(itemWithVelocity1.Velocity, collisionNormal) : 0;
        float speed2 = itemWithVelocity2 is not null ? Vector2.Dot(itemWithVelocity2.Velocity, collisionNormal) : 0;
        
        float speedDifference = speed1 - speed2;
        if (speedDifference < 0)
        {
            return;
        }

        float cor1 = item1 is ICoefficientOfRestitution restitution1 ? restitution1.CoefficientOfRestitution : 1;
        float cor2 = item2 is ICoefficientOfRestitution restitution2 ? restitution2.CoefficientOfRestitution : 1;
        float cor = cor1 * cor2;
        float mass1Inverse = item1 is IMass ? 1.0f / ((IMass)item1).Mass : 0;
        float mass2Inverse = item2 is IMass ? 1.0f / ((IMass)item2).Mass : 0;
        float impact = -(cor+1) * speedDifference / (mass1Inverse + mass2Inverse);
        if (mass1Inverse > 0 && itemWithVelocity1 is not null)
        {
            itemWithVelocity1.Velocity += collisionNormal * (impact * mass1Inverse);
        }

        if (mass2Inverse > 0 && itemWithVelocity2 is not null)
        {
            itemWithVelocity2.Velocity -= collisionNormal * (impact * mass2Inverse);
        }
    }
    
    protected void ExchangeEnergy(object item1, object item2, Vector2 collisionNormal, Vector2 pointOfImpact)
    {
        IPosition item1WithPosition = item1 as IPosition;
        IMovable movableItem1 = item1 as IMovable;
        IRotatable rotatableItem1 = item1 as IRotatable;
        IPosition item2WithPosition = item2 as IPosition;
        IMovable movableItem2 = item2 as IMovable;
        IRotatable rotatableItem2 = item2 as IRotatable;
        Vector2 velocity1 = movableItem1?.Velocity ?? Vector2.Zero;
        Vector2 velocity2 = movableItem2?.Velocity ?? Vector2.Zero;
        Vector2 lever1 = new();
        Vector2 lever2 = new();
        Vector2 tangentialDirection1 = new();
        Vector2 tangentialDirection2 = new();

        if (item1WithPosition is not null && rotatableItem1 is not null)
        {
            lever1 = pointOfImpact - item1WithPosition.Position;
            tangentialDirection1 = Vector2.Normalize(new Vector2(-lever1.Y, lever1.X));
            Vector2 rotationalVelocity = tangentialDirection1 * (lever1.Length() * rotatableItem1.AngularVelocity);
            velocity1 += rotationalVelocity;
        }

        if (item2WithPosition is not null && rotatableItem2 is not null)
        {
            lever2 = pointOfImpact - item2WithPosition.Position;
            tangentialDirection2 = Vector2.Normalize(new Vector2(-lever2.Y, lever2.X));
            Vector2 rotationalVelocity = tangentialDirection2 * (lever2.Length() * rotatableItem2.AngularVelocity);
            velocity2 += rotationalVelocity;
        }

        float speed1 = Vector2.Dot(velocity1, collisionNormal);
        float speed2 = Vector2.Dot(velocity2, collisionNormal);
        float speedDifference = speed1 - speed2;
        if (speedDifference < 0)
        {
            return;
        }

        float cor1 = item1 is ICoefficientOfRestitution
            ? ((ICoefficientOfRestitution)item1).CoefficientOfRestitution
            : 1;
        float cor2 = item2 is ICoefficientOfRestitution
            ? ((ICoefficientOfRestitution)item2).CoefficientOfRestitution
            : 1;
        float cor = cor1 * cor2;
        float mass1Inverse = item1 is IMass ? 1.0f / ((IMass)item1).Mass : 0;
        float mass2Inverse = item2 is IMass ? 1.0f / ((IMass)item2).Mass : 0;
        IAngularMass item1WithAngularMass = item1 as IAngularMass;
        IAngularMass item2WithAngularMass = item2 as IAngularMass;
        float angularMass1Inverse = item1WithAngularMass is not null
            ? MathF.Pow(Vector2.Dot(tangentialDirection1, collisionNormal) * lever1.Length(), 2) /
              item1WithAngularMass.AngularMass
            : 0;
        float angularMass2Inverse = item2WithAngularMass is not null
            ? MathF.Pow(Vector2.Dot(tangentialDirection2, collisionNormal) * lever2.Length(), 2) /
              item2WithAngularMass.AngularMass
            : 0;
        float impact = -(cor + 1) * speedDifference /
                       (mass1Inverse + mass2Inverse + angularMass1Inverse + angularMass2Inverse);

        if (mass1Inverse > 0 && movableItem1 is not null)
        {
            movableItem1.Velocity += (collisionNormal * (impact * mass1Inverse));
        }

        if (mass2Inverse > 0 && movableItem2 is not null)
        {
            movableItem2.Velocity -= (collisionNormal * (impact * mass2Inverse));
        }

        if (item1WithAngularMass is not null)
        {
            float tangentialForce = Vector2.Dot(tangentialDirection1, collisionNormal) * impact;
            float change = tangentialForce * lever1.Length() / item1WithAngularMass.AngularMass;
            rotatableItem1.AngularVelocity += change;
        }

        if (item2WithAngularMass is not null)
        {
            float tangentialForce = Vector2.Dot(tangentialDirection2, collisionNormal) * -impact;
            float change = tangentialForce * lever2.Length() / item2WithAngularMass.AngularMass;
            rotatableItem2.AngularVelocity += change;
        }
    }*/
}