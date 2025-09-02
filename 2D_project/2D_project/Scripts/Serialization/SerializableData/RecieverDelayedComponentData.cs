using GamePlatformer;
using System;
using System.Reflection;

internal class RecieverDelayedComponentData : IComponentTypeData
{
    public string Type { get; set; }
    public int DelayInSeconds { get; set; }
    public string PerformActionName { get; set; }
    public int TransmitterObjectID { get; set; }
    public string TransmitterComponentType { get; set; }
    public string EventHandler { get; set; }

    public static Reciever Deserialize(RecieverDelayedComponentData data, string PerformActionName = null)
    {
        // Basic validation
        if (PerformActionName is null && (data.PerformActionName is null || data.PerformActionName == "")
            || data.Type is null || data.Type == ""
            || data.TransmitterComponentType is null || data.TransmitterComponentType == ""
            || data.EventHandler is null || data.EventHandler == ""
            || data.DelayInSeconds <= 0)
        {
            throw new ArgumentException("Invalid JSON data for Reciever.");
        }

        // 1. find perform action
        string methodName = PerformActionName ?? data.PerformActionName;
        // Use reflection to find the method by name
        MethodInfo methodInfo = typeof(RecieverActions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

        // Create the delegate and bind it to the static method
        Reciever.PerformAction performAction = (Reciever.PerformAction)Delegate.CreateDelegate(typeof(Reciever.PerformAction), null, methodInfo);

        // Create the Reciever component
        //Reciever reciever = new Reciever(data.PerformAction);
        RecieverDelayed reciever = new RecieverDelayed(Game2DPlatformer.Instance, performAction, data.DelayInSeconds);

        reciever.TransmitterObjectID = data.TransmitterObjectID;
        reciever.EventHandlerName = data.EventHandler;


        // 2.
        // Find the Type from the string name
        switch (data.TransmitterComponentType)
        {
            case "Terrain_InvisibleCollider":
                reciever.TransmitterComponentType = typeof(Terrain_InvisibleCollider);
                break;
            case "Reciever":
                reciever.TransmitterComponentType = typeof(Reciever);
                break;
            case "MovePlayerWithPlatformLogic":
                reciever.TransmitterComponentType = typeof(MovePlayerWithPlatformLogic);
                break;
            case "MoveStopOnCollisionComponent":
                reciever.TransmitterComponentType = typeof(MovePlayerWithPlatformLogic);
                break;
            case "TerrainButtonBox":
                reciever.TransmitterComponentType = typeof(Terrain_ButtonBox);
                break;
            case "BossEnemy":
                reciever.TransmitterComponentType = typeof(BossEnemy);
                break;
            case "TeleportObject":
                reciever.TransmitterComponentType = typeof(TeleportObject);
                break;
            /*
        case "MoveOnCollisionPlatformComponent":
            return MoveOnCollisionPlatformComponentData.Deserialize(componentData as MoveOnCollisionPlatformComponentData);
            */

            /*
        case "TeleportObject":
            return TeleportObjectData.Deserialize(componentData as TeleportObjectData);*/
            // Add cases for other types if needed
            default:
                throw new ArgumentException($"Unknown type: {data.TransmitterComponentType}");
        }

        // Return the Reciever instance
        return reciever;
    }
}
