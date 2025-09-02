using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class RespawnPointSystem
{
    public static RespawnPointSystem Instance;
    public Dictionary<Scene, List<RespawnPoint>> stageRespawns = new Dictionary<Scene, List<RespawnPoint>>();
    private int activeRespawnPointIndex = 0;
    private Scene activeScene;

    public RespawnPointSystem(/*Dictionary<Scene, List<RespawnPoint>> stageRespawns*/)
    {
        // TODO find a way to assign respawn points
        if (Instance is not null) return;
        Instance = this;

        //this.stageRespawns = stageRespawns;
        SceneManager.Instance.OnSceneChange += (object sender, EventArgs e) =>
        {
            // remove respawn point from current scene
            activeScene = SceneManager.Instance.activeScene;
            if (stageRespawns.ContainsKey(activeScene)) stageRespawns[activeScene][activeRespawnPointIndex].RespawnPointStatusChanged(conditionIsMet: false);

            /*
            // adjust to the new scene
            activeRespawnPointIndex = 0; // start as first respawn point in the list
            AssignStartingRespawnPoint(scene: activeScene, startingRespawnPointIndex: activeRespawnPointIndex); */
        };
    }

    public void AssignStartingRespawnPoint(Scene scene, List<RespawnPoint> respawnPoints, int startingRespawnPointIndex)
    {
        if (!stageRespawns.ContainsKey(scene)) stageRespawns[scene] = respawnPoints; // assign respawn points, if it they do not already exist

        //stageRespawns[scene][activeRespawnPointIndex].RespawnPointStatusChanged(conditionIsMet: false); // remove current respawn point
        if (activeRespawnPointIndex >= stageRespawns[scene].Count)
        {
            Debug.WriteLine("starting respawn point index should not be higher than the number of respawn points");
            activeRespawnPointIndex = stageRespawns[scene].Count;
        }

        activeRespawnPointIndex = startingRespawnPointIndex;
        stageRespawns[scene][activeRespawnPointIndex].RespawnPointStatusChanged(conditionIsMet: true); // add respawn point
        Player.Instance.ResetPlayer(respawnPoints[startingRespawnPointIndex].spawnLocation);
    }

    public void RespawnPointReached(int newRespawnPointIndex)
    {
        stageRespawns[activeScene][activeRespawnPointIndex].RespawnPointStatusChanged(conditionIsMet: false); // remove current respawn point
        activeRespawnPointIndex = newRespawnPointIndex;
        if (activeRespawnPointIndex == stageRespawns[activeScene].Count) activeRespawnPointIndex = 0;
        stageRespawns[activeScene][activeRespawnPointIndex].RespawnPointStatusChanged(conditionIsMet: true); // add respawn point
    }

    public void NextRespawnPointReached(object sender, EventArgs e)
    {
        stageRespawns[activeScene][activeRespawnPointIndex++].RespawnPointStatusChanged(conditionIsMet: false); // remove current respawn point
        if (activeRespawnPointIndex == stageRespawns[activeScene].Count) activeRespawnPointIndex = 0;
        stageRespawns[activeScene][activeRespawnPointIndex].RespawnPointStatusChanged(conditionIsMet: true); // add respawn point
    }

    public class RespawnPoint(Vector2 spawnLocation)
    {
        public Vector2 spawnLocation { get; private set; } = spawnLocation;
        public void RespawnPointStatusChanged(bool conditionIsMet)
        {
            // unsubscribe -> 1.) to make sure no duplicate subscription + 2.) unsubscribe if condition is not met
            Player.Instance.onDeath -= RespawnPlayer;
            if (conditionIsMet) Player.Instance.onDeath += RespawnPlayer;
        }

        public void RespawnPlayer(object sender, EventArgs e)
        {
            Player.Instance.ResetPlayer(spawnLocation);
        }
    }
}

internal class RespawnPointReachedRecieverComponentData : RecieverComponentData
{
    public Vector2 spawnLocation { get; set; }
    public static Reciever Deserialize(RespawnPointReachedRecieverComponentData data)
    {
        // Basic validation
        if (data.Type is null || data.Type == ""
            || data.TransmitterComponentType is null || data.TransmitterComponentType == ""
            || data.EventHandler is null || data.EventHandler == "")
        {
            throw new ArgumentException("Invalid JSON data for Reciever.");
        }


        // PerformActionName = RespawnPointReached
        return Deserialize(data: data, PerformActionName: "RespawnPointReached");
    }
}

internal class RespawnPointIndex(int index) : ObjectComponent
{
    public int index { get; private set; } = index;
}
internal class RespawnPointIndexData : IComponentTypeData
{
    public int Index;
    public string Type { get; set; }

    public static RespawnPointIndex Deserialize(RespawnPointIndexData data)
    {
        return new RespawnPointIndex(data.Index);
    }
}
