using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

internal class Phase2_DrillMode_BossEnemy_DragonicGrasser : IPhase
{
    // TODO -> add actual logic for the boss fight / right now only sprites
    private GameObject _gameObject;
    private IBossMethod movementMethod;

    private BossEnemy bossEnemy;

    SpriteAnimated animatedSprite;

    PhaseColliderObject _phaseColliderObject;
    PhaseColliderObject IPhase.phaseColliderObject { get => _phaseColliderObject; set => _phaseColliderObject = value; }

    public EventHandler OnVisualChange { get; set; }

    public GameObject GetVisualGameObject()
    {
        return _gameObject;
    }

    void IPhase.CreateVisuals(GameObject parent)
    {
        // 1.) create GameObject that will hold visuals
        _gameObject = new GameObject();
        _gameObject.CreateTransform();

        parent.AddChild(_gameObject);

        // 2.) assign animated sprite
        int frameCount = 5;
        Rectangle[] sourceRectangles = JSON_Manager.GetEnemiesSourceRectangles("Dragonic Grasser_Drill", frameCount);
        float[] frameTimers = [0.4f, 0.15f, 0.15f, 0.15f, 0.15f];
        animatedSprite = new SpriteAnimated(
                texture2D: JSON_Manager.enemiesSpriteSheet,
                sourceRectangles: sourceRectangles,
                frameTimers: frameTimers,
                colorTints: [Color.White, Color.White, Color.White, Color.White, Color.White],
                origins: JSON_Manager.GetEnemiesOrigin("Dragonic Grasser_Drill", frameCount, parent.transform.globalScale)
        );

        _gameObject.AddComponent(animatedSprite);


        _phaseColliderObject = new GrasserPhase3Colliders(parent: _gameObject, spriteAnimated: animatedSprite);

        _gameObject.SetActive(false);
    }

    void IPhase.BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods)
    {
        this.bossEnemy = bossEnemy;
        bossEnemy.moveSpeed = 0;

        movementMethod = movementMethods[0];
        Movement_DrillMode movement_DrillMode = (Movement_DrillMode)movementMethod;

        movementMethod.ResetSubsteps();
        Vector2 spawnPosition = bossEnemy.ArrivalPosition;

        List<Vector2> teleportLocations = new()
        {
            new Vector2(spawnPosition.X - 1450, spawnPosition.Y + 200),
            new Vector2(spawnPosition.X - 750,  spawnPosition.Y + 500),
            new Vector2(spawnPosition.X + 750,  spawnPosition.Y + 500),
            new Vector2(spawnPosition.X + 1450, spawnPosition.Y + 200)
        };

        Vector2 playerPos = Player.Instance.gameObject.transform.globalPosition;

        // Find closest teleport location to the player
        Vector2 closest = teleportLocations[0];
        float minDistance = Vector2.Distance(playerPos, closest);

        foreach (var location in teleportLocations)
        {
            float distance = Vector2.Distance(playerPos, location);
            if (distance < minDistance)
            {
                closest = location;
                minDistance = distance;
            }
        }

        // Teleport boss to closest location
        bossEnemy.gameObject.transform.globalPosition = closest;

        Vector2 start = closest;

        // Choose the teleport point furthest in the direction of the player
        // Compute direction from start to player
        Vector2 toPlayer = Vector2.Normalize(playerPos - start);

        // Filter teleport points that are in general direction toward player (dot > 0), and not the same as start
        List<Vector2> candidates = teleportLocations
            .Where(point => point != start && Vector2.Dot(Vector2.Normalize(point - start), toPlayer) > 0)
            .ToList();

        // Fallback: if no points are in the direction (rare), include all except start
        if (candidates.Count == 0)
        {
            candidates = teleportLocations.Where(point => point != start).ToList();
        }

        // Pick one randomly
        Vector2 end = candidates[Game2DPlatformer.Instance.random.Next(candidates.Count)];

        /*
        Vector2 best = teleportLocations[0];
        float bestDot = Vector2.Dot(Vector2.Normalize(playerPos - start), Vector2.Normalize(best - start));

        foreach (var point in teleportLocations)
        {
            float dot = Vector2.Dot(Vector2.Normalize(playerPos - start), Vector2.Normalize(point - start));
            if (dot > bestDot)
            {
                bestDot = dot;
                best = point;
            }
        }

        Vector2 end = best;*/

        float waveAmplitude = 600f;

        float totalDistance = Vector2.Distance(start, end);
        float targetSegmentLength = 187.5f; // example, tune as needed

        int segmentCount = Math.Max(1, (int)MathF.Round(totalDistance / targetSegmentLength));

        // Random maxMovementCount between 1->3 inclusive | 1->2 if it is a longer distance | 1 if it is the longest distance
        int maxMovementCount;

        if (totalDistance > 2300f)
        {
            maxMovementCount = 1;
        }
        else if (totalDistance > 1500f)
        {
            maxMovementCount = Game2DPlatformer.Instance.random.Next(1, 3); // 1 or 2
        }
        else
        {
            maxMovementCount = Game2DPlatformer.Instance.random.Next(1, 4); // 1 to 3
        }

        // Random reverseDirectionOnEnd as true or false
        bool reverseDirectionOnEnd = Game2DPlatformer.Instance.random.Next(0, 2) == 0;

        movementMethod.SetParameters(
            start,
            end,
            waveAmplitude,
            segmentCount,
            maxMovementCount,
            reverseDirectionOnEnd
        );

        // 2.) setup attack pattern -> there is no attack method
        // 

        // reset animation
        animatedSprite.currMinFrameIndex = 0;
        animatedSprite.SetFrame(0);
        animatedSprite.ResumeAnimation();

        // show visuals
        _gameObject.SetActive(true);
    }

    void IPhase.UpdatePhase(GameTime gameTime)
    {
        // update sprite effects
        animatedSprite.spriteEffects = (bossEnemy.Velocity.X <= 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        movementMethod?.Execute((float)gameTime.ElapsedGameTime.TotalSeconds);

        if (animatedSprite.currFrameIndex == 2)
        {
            animatedSprite.currMinFrameIndex = 2; // after reaching this point keep the minimum at 2(drill sprite start index)
        }

        _phaseColliderObject.UpdateColliders();
    }

    void IPhase.Reload(GameTime gameTime) { return; } // no attacking -> no reloading requiered
}