using Microsoft.Xna.Framework;
using System;

internal class Movement_Spiral : IBossMethod
{
    private Vector2 center;
    private float angularSpeed;    // Radians per frame
    private float radialSpeed;     // Units per frame
    private float angle;           // Current angle in radians
    private float radius;          // Current radius from the center
    private float maxRadius;       // Stop expanding after this
    private readonly BossEnemy bossEnemy;

    public Movement_Spiral(BossEnemy bossEnemy)
    {
        this.bossEnemy = bossEnemy;
    }

    public void ResetSubsteps()
    {
        angle = 0;
        radius = 0;
    }

    public void ChangeSpiralCenter(Vector2 newCenter)
    {
        center = newCenter;
    }
    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 5) throw new ArgumentException("Expected 5 parameters.");
        center = (Vector2)parameters[0];
        angularSpeed = (float)parameters[1];
        radialSpeed = (float)parameters[2];
        maxRadius = (float)parameters[3];
        angle = (float)parameters[4];

        radius = 0;
    }

    /*
    public void Execute(float? deltaTime = null)
    {
        if (radius >= maxRadius) return;

        // Increment angle and radius
        angle += angularSpeed;
        radius += radialSpeed;

        // Convert polar to Cartesian
        float x = center.X + (float)(Math.Cos(angle) * radius);
        float y = center.Y + (float)(Math.Sin(angle) * radius);

        Vector2 targetPosition = new Vector2(x, y);
        bossEnemy.gameObject.transform.globalPosition = targetPosition;
    }*/

    public void Execute(float? deltaTime = null)
    {
        if (radius >= maxRadius) return;

        float dt = deltaTime ?? (1f / 60f); // Assume 60 FPS if deltaTime is not provided

        // Increment angle and radius with delta time
        angle += angularSpeed * dt;
        radius += radialSpeed * dt;

        // Convert polar to Cartesian
        float x = center.X + MathF.Cos(angle) * radius;
        float y = center.Y + MathF.Sin(angle) * radius;

        bossEnemy.gameObject.transform.globalPosition = new Vector2(x, y);
    }
}
