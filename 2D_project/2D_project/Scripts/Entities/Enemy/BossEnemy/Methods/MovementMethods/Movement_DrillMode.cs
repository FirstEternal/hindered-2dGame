using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

internal class Movement_DrillMode : IBossMethod
{
    private readonly BossEnemy bossEnemy;

    private float waveAmplitude;
    private float waveLength;
    private Vector2 direction;
    private Vector2 startPosition;
    private Vector2 endPosition;

    private float totalDistance;
    private float horizontalDistanceTraveled = 0f;

    //private int previousSegmentIndex = -1;
    //private bool? wasGoingUp = null;
    private float speed;

    int currMovementCount;
    int maxMovementCount;
    bool reverseDirectionOnEnd;

    public Movement_DrillMode(BossEnemy bossEnemy)
    {
        this.bossEnemy = bossEnemy;
    }

    public void ResetSubsteps()
    {
        //previousSegmentIndex = -1;
        //wasGoingUp = null;
        horizontalDistanceTraveled = 0f;
        //currMovementCount = 0;

        direction = Vector2.Normalize(endPosition - startPosition);
        totalDistance = Vector2.Distance(startPosition, endPosition);

        bossEnemy.gameObject.transform.globalPosition = startPosition;
        bossEnemy.Velocity = Vector2.Zero;
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 6)
            throw new ArgumentException("Expected 4 parameters: startPosition, endPosition, amplitude, segmentCount");

        startPosition = (Vector2)parameters[0];
        endPosition = (Vector2)parameters[1];
        waveAmplitude = (float)parameters[2];
        int segmentCount = (int)parameters[3];

        if (segmentCount <= 0 || segmentCount % 2 != 0)
            throw new ArgumentException("Segment count must be a positive even number.");

        direction = Vector2.Normalize(endPosition - startPosition);
        totalDistance = Vector2.Distance(startPosition, endPosition);
        waveLength = totalDistance / segmentCount;

        // Calculate speed so each wave segment takes fixed time (e.g. 0.2 seconds)
        float segmentDuration = 1f;
        speed = MathF.Min(waveLength / segmentDuration, 150);

        currMovementCount = 0;
        maxMovementCount = (int)parameters[4];
        reverseDirectionOnEnd = (bool)parameters[5];
        if (maxMovementCount > 1) speed *= 1.4f; // increase speed slightly

        bossEnemy.gameObject.transform.globalPosition = startPosition;
        bossEnemy.Velocity = Vector2.Zero;

        ResetSubsteps();
    }

    public void Execute(float? deltaTime = null)
    {
        Vector2 basePosition = bossEnemy.gameObject.transform.globalPosition;

        float distanceAlongX;

        if (deltaTime.HasValue)
        {
            float safeDeltaTime = MathF.Min(deltaTime.Value, 1f / 30f);
            float horizontalStep = bossEnemy.Velocity.X * safeDeltaTime;
            horizontalDistanceTraveled += Math.Abs(horizontalStep);
            distanceAlongX = horizontalDistanceTraveled;
        }
        else
        {
            Debug.WriteLine("deltaTime = null, might result in unwanted behaviour");
            distanceAlongX = Vector2.Distance(startPosition, basePosition);
        }

        // Clamp to max travel range (avoid overshooting end)
        distanceAlongX = MathF.Min(distanceAlongX, totalDistance);

        // Check if close enough to snap
        float remainingDistance = totalDistance - distanceAlongX;
        if (remainingDistance <= 10f)
        {
            currMovementCount++;
            if (currMovementCount >= maxMovementCount)
            {
                bossEnemy.gameObject.transform.globalPosition = endPosition;
                bossEnemy.Velocity = Vector2.Zero;
                bossEnemy.TriggerSpecialMoveEnd();
            }
            else
            {
                if (reverseDirectionOnEnd) RepeatMovementInReverseDirection();
                else RepeatMovement();
            }

            return;
        }

        int currentSegment = (int)(distanceAlongX / waveLength);
        float segmentProgress = (distanceAlongX % waveLength) / waveLength;

        bool isGoingUp = (currentSegment % 2 == 0);

        float offsetY = isGoingUp
            ? -segmentProgress * waveAmplitude
            : (-waveAmplitude + segmentProgress * waveAmplitude);

        Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
        if (direction.X < 0)
            perpendicular = -perpendicular;

        Vector2 verticalOffset = perpendicular * offsetY;
        Vector2 forward = direction * distanceAlongX;
        Vector2 newPosition = startPosition + forward + verticalOffset;

        bossEnemy.gameObject.transform.globalPosition = newPosition;

        // Maintain forward velocity
        MGEngine.Physics.Movement.AssignVelocity(endPosition, bossEnemy, speed);

        // Compute tangent direction and rotation
        float slope = isGoingUp ? 1f : -1f;
        if (direction.X < 0)
            slope = -slope;


        Vector2 tangent = Vector2.Normalize(direction * waveLength + perpendicular * slope * waveAmplitude);
        float rotationAngle = MathF.Atan2(tangent.Y, tangent.X);

        bool isFlipped = bossEnemy.Velocity.X <= 0;

        // Mirror rotation horizontally if flipped (facing left)
        if (isFlipped)
        {
            rotationAngle = MathF.PI - rotationAngle;
        }

        // Calculate rotation adjustment based on zigzag direction (up or down)
        float baseAdjustment = 3 * MathF.PI / 4; // NO IDEA WHY I NEED 135° but oh well it works
        float rotationAdjustment = isGoingUp ? baseAdjustment : -baseAdjustment;

        // Apply adjustment direction depending on flip state
        rotationAdjustment *= isFlipped ? 1f : -1f;

        // Final rotation applied to the boss sprite
        bossEnemy.gameObject.transform.localRotationAngle = rotationAngle + rotationAdjustment;
    }

    private void RepeatMovement()
    {
        Debug.WriteLine("movement repeated");
        ResetSubsteps();
    }

    private void RepeatMovementInReverseDirection()
    {
        Debug.WriteLine("movement repeated in reverse");

        Vector2 newStartPosition = new Vector2(endPosition.X, startPosition.Y);
        Vector2 newEndPosition = new Vector2(startPosition.X, endPosition.Y);

        startPosition = newStartPosition;
        endPosition = newEndPosition;

        ResetSubsteps();
    }
}
