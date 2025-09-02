using Microsoft.Xna.Framework;
using System;

internal class Movement_DownBounceLeftRightReturnUp : IBossMethod
{
    // Parameters for the movement
    private Vector2 startPosition;
    private Vector2 downDestination;
    private Vector2 leftDestination;
    private Vector2 rightDestination;
    private float speedDown;
    private float speedLeft;
    private float speedRight;
    private int leftRightBounceCount;
    private bool isReverseOrder;
    private float dmgReduction;

    // Internal state variables
    private int phaseSubstep = 0;
    private int bounceCount = 0;

    private readonly BossEnemy bossEnemy; // Reference to the game object (assumed to be injected or set externally)

    public Movement_DownBounceLeftRightReturnUp(BossEnemy bossEnemy)
    {
        this.bossEnemy = bossEnemy;
    }

    public void ResetSubsteps()
    {
        phaseSubstep = 0;
    }


    /// <summary>
    /// Sets the parameters for the recovery movement attack.
    /// </summary>
    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 10) throw new ArgumentException("Expected 8 parameters.");
        startPosition = (Vector2)parameters[0];
        downDestination = (Vector2)parameters[1];
        leftDestination = (Vector2)parameters[2];
        rightDestination = (Vector2)parameters[3];
        speedDown = (float)parameters[4];
        speedLeft = (float)parameters[5];
        speedRight = (float)parameters[6];
        leftRightBounceCount = (int)parameters[7];
        isReverseOrder = (bool)parameters[8];
        dmgReduction = (float)parameters[9];

        phaseSubstep = 0;
    }

    /// <summary>
    /// Executes the recovery movement attack logic.
    /// </summary>
    public void Execute(float? deltaTime = null)
    {
        int tolerationDistance = 50;

        switch (phaseSubstep)
        {
            case 0:
                // Move down to downDestination
                bossEnemy.Velocity = new Vector2(0, speedDown) * (isReverseOrder ? -1 : 1);

                if (MGEngine.Physics.Movement.IsInPosition(downDestination, bossEnemy.gameObject.transform, tolerationDistance))
                {
                    bossEnemy.Velocity = new Vector2(-speedLeft, 0);
                    bossEnemy.dmgReduction = dmgReduction; // Make immune to damage
                    phaseSubstep++;

                    bossEnemy.gameObject.transform.globalPosition = downDestination;
                }
                break;

            case 1:
                // Move Bounce left and right
                if (bounceCount == leftRightBounceCount)
                {
                    if ((bossEnemy.Velocity.X < 0 && bossEnemy.gameObject.transform.globalPosition.X < downDestination.X) ||
                        (bossEnemy.Velocity.X > 0 && bossEnemy.gameObject.transform.globalPosition.X > downDestination.X))
                    {
                        bossEnemy.dmgReduction = 0; // Remove damage immunity
                        phaseSubstep++;

                        bossEnemy.gameObject.transform.globalPosition = downDestination;
                        bossEnemy.Velocity = new Vector2(0, -speedDown) * (isReverseOrder ? -1 : 1);
                    }
                }
                else
                {
                    if (bossEnemy.Velocity.X < 0 && MGEngine.Physics.Movement.IsInPosition(leftDestination, bossEnemy.gameObject.transform, tolerationDistance))
                    {
                        bossEnemy.Velocity = new Vector2(speedRight, 0);
                        bossEnemy.gameObject.transform.globalPosition = leftDestination;
                        bounceCount++;
                    }
                    else if (bossEnemy.Velocity.X > 0 && MGEngine.Physics.Movement.IsInPosition(rightDestination, bossEnemy.gameObject.transform, tolerationDistance))
                    {
                        bossEnemy.Velocity = new Vector2(-speedLeft, 0);
                        bossEnemy.gameObject.transform.globalPosition = rightDestination;
                        bounceCount++;
                    }
                }
                break;

            case 2:
                // Move back to startPosition
                if (MGEngine.Physics.Movement.IsInPosition(startPosition, bossEnemy.gameObject.transform, tolerationDistance))
                {
                    bossEnemy.Velocity = Vector2.Zero;
                    phaseSubstep = 0;

                    bossEnemy.gameObject.transform.globalPosition = startPosition;
                    bounceCount = 0;

                    // Begin recovering
                    bossEnemy.BeginRecovering();
                }
                break;
        }
    }
}