using Microsoft.Xna.Framework;
using System;

internal class Movement_BackForth : IBossMethod
{
    // Parameters for the movement
    private Vector2 startPosition;
    private Vector2 endDestination;
    private float speedForth;
    private float speedBack;
    private int backForthCount;

    // Internal state variables
    private int phaseSubstep = 0;
    private int currentBackForthCount = 0;

    private readonly Entity entity; // Reference to the game object

    public Movement_BackForth(Entity entity)
    {
        this.entity = entity;
    }

    public void ResetSubsteps()
    {
        phaseSubstep = 0;
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 5) throw new ArgumentException("Expected 5 parameters.");
        startPosition = (Vector2)parameters[0];
        endDestination = (Vector2)parameters[1];
        speedForth = (float)parameters[2];
        speedBack = (float)parameters[3];
        backForthCount = (int)parameters[4];

        phaseSubstep = 0;
        currentBackForthCount = 0;
    }

    public void Execute(float? deltaTime = null)
    {
        if (currentBackForthCount >= backForthCount) return;

        int tolerationDistance = 50;

        switch (phaseSubstep)
        {
            case 0:
                MGEngine.Physics.Movement.AssignVelocity(finalPosition: endDestination, entity, speedForth);

                if (MGEngine.Physics.Movement.IsInPosition(endDestination, entity.gameObject.transform, tolerationDistance))
                {
                    phaseSubstep++;
                    entity.gameObject.transform.globalPosition = endDestination;
                }
                break;

            case 1:
                MGEngine.Physics.Movement.AssignVelocity(finalPosition: startPosition, entity, speedBack);

                if (MGEngine.Physics.Movement.IsInPosition(startPosition, entity.gameObject.transform, tolerationDistance))
                {
                    phaseSubstep--;
                    entity.gameObject.transform.globalPosition = startPosition;
                    currentBackForthCount++;
                }
                break;
        }
    }
}
