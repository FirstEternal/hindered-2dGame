
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class ResetComponent : ObjectComponent
{
    private List<string> componentTypesToReset;

    // Reset GameObject
    private bool isActive;

    // Transform
    object[] transformValues;

    // Physical Component
    object[] physicalComponentValues;

    public ResetComponent(List<string> componentTypesToReset, bool isActive, object[] transformValues, object[] physicalComponentValues)
    {
        this.componentTypesToReset = componentTypesToReset;
        this.isActive = isActive;
        this.transformValues = transformValues;
        this.physicalComponentValues = physicalComponentValues;
    }

    public void ResetAllValues()
    {
        // Reset GameObject
        gameObject.SetActive(isActive);

        // Transform
        if (transformValues is not null)
        {
            Vector2? GlobalPosition = (Vector2?)transformValues[0];
            float? LocalRotation = (float?)transformValues[1];
            Vector2? LocalScale = (Vector2?)transformValues[2];

            if (GlobalPosition is not null) gameObject.transform.globalPosition = GlobalPosition ?? Vector2.Zero;
            if (LocalRotation is not null) gameObject.transform.localRotationAngle = LocalRotation ?? 0;
            if (LocalScale is not null) gameObject.transform.globalScale = LocalScale ?? Vector2.Zero;
        }

        if (physicalComponentValues is not null)
        {
            var physicsComponent = gameObject.GetComponent<PhysicsComponent>();
            if (physicsComponent != null)
            {
                float? Mass = (float?)physicalComponentValues[0];
                bool? IsMovable = (bool?)physicalComponentValues[1];
                bool? IsGravity = (bool?)physicalComponentValues[2];
                Vector2? Velocity = (Vector2?)physicalComponentValues[3];
                float? AngularVelocity = (float?)physicalComponentValues[4];

                if (Mass is not null) physicsComponent.Mass = Mass ?? 1;
                if (IsMovable is not null) physicsComponent.isMovable = IsMovable ?? false;
                if (IsGravity is not null) physicsComponent.isGravity = IsGravity ?? true;
                if (Velocity is not null) physicsComponent.Velocity = Velocity ?? Vector2.Zero;
                if (AngularVelocity is not null) physicsComponent.AngularVelocity = AngularVelocity ?? 0;
            }
        }

        foreach (string componentType in componentTypesToReset)
        {
            Type? type = Type.GetType(componentType);

            if (type != null)
            {
                ObjectComponent? component = gameObject.GetComponent(type) as ObjectComponent;
                if (component is IResettable resettable)
                {
                    resettable.Reset();
                }
                else
                {
                    Debug.WriteLine($"Component {componentType} does not implement IResettable");
                }
            }
            else
            {
                Debug.WriteLine($"Type not found: {componentType}");
            }
        }
    }

}