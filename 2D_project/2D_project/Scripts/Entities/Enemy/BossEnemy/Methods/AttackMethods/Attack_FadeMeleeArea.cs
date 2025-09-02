using Microsoft.Xna.Framework;
using System;

internal class Attack_FadeMeleeArea : IBossMethod
{
    public Attack_FadeMeleeArea(BossEnemy enemy)
    {
        this.enemy = enemy;
    }

    Entity enemy;

    Vector2[] attackLocations;
    float fadeDuration;
    int width;
    int height;

    int dmg;
    Vector2[] knockDirections;
    float pushPower;

    public void ResetSubsteps()
    {
    }

    public void SetParameters(params object[] parameters)
    {
        if (parameters.Length != 7)
            throw new ArgumentException("Invalid number of parameters for fade melee Area");

        // create new fadeIndicator
        attackLocations = (Vector2[])parameters[0];
        knockDirections = (Vector2[])parameters[1];
        width = (int)parameters[2];
        height = (int)parameters[3];
        fadeDuration = (float)parameters[4];
        pushPower = (float)parameters[5];
        dmg = (int)parameters[6];
    }

    public void Execute(float? deltaTime = null)
    {
        for (int i = 0; i < attackLocations.Length; i++)
        {
            AttackObjectPoolingSystem.GetSpawnedMeele(
                fadeDuration,
                attackLocations[i],
                enemy,
                width,
                height,
                dmg,
                knockDirections[i],
                pushPower
            );
        }

    }
}
