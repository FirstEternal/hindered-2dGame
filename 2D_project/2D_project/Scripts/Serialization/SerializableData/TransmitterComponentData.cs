using System;
using System.Reflection;

internal class TransmitterComponentData : IComponentTypeData, IGameObjectData
{
    public string Type { get; set; }
    public int ID { get; set; }
    public string Tag { get; set; } // not really necessary, but might be good to have in the future

    public string PerformActionName { get; set; }
    public bool isActive { get; set; } // irrelevant in this perticular scenario

    public static Reciever Deserialize(RecieverComponentData data)
    {
        // Basic validation
        if (data.PerformActionName is null || data.PerformActionName == "")
        {
            throw new ArgumentException("Invalid JSON data for Reciever.");
        }

        string methodName = data.PerformActionName;
        // Use reflection to find the method by name
        MethodInfo methodInfo = typeof(RecieverActions).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

        // Create the delegate and bind it to the static method
        Reciever.PerformAction performAction = (Reciever.PerformAction)Delegate.CreateDelegate(typeof(Reciever.PerformAction), null, methodInfo);

        // Create the Reciever component
        //Reciever reciever = new Reciever(data.PerformAction);
        Reciever reciever = new Reciever(performAction);

        // Return the Reciever instance
        return reciever;
    }
}
