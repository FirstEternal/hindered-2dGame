using MGEngine.ObjectBased;
using System.Diagnostics;
using System.Reflection;

public class Reciever(Reciever.PerformAction performAction) : ObjectComponent
{
    public delegate void PerformAction(params object[] parameters);
    public PerformAction performAction = performAction;
    public EventHandler? OnActionPerformed;

    // JSON ASSIGNMENT
    public int TransmitterObjectID { get; set; }
    public Type TransmitterComponentType { get; set; }
    public string EventHandlerName { get; set; }

    public void AssignTransmitter(List<GameObject> gameObjects)
    {
        GameObject transmitter = null;

        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.id == TransmitterObjectID)
            {
                transmitter = gameObject;
                break; // Break early when found
            }
        }

        if (transmitter == null)
        {
            Debug.WriteLine("Transmitter object not found.");
            return;
        }

        if (TransmitterComponentType == null)
        {
            Debug.WriteLine("TransmitterComponentType is not set.");
            return;
        }

        ObjectComponent component = transmitter.GetComponent(TransmitterComponentType);
        if (component == null)
        {
            Debug.WriteLine($"Component '{TransmitterComponentType.Name}' not found on the transmitter.");
            return;
        }

        EventInfo eventInfo = TransmitterComponentType.GetEvent(EventHandlerName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly);
        if (eventInfo == null)
        {
            Debug.WriteLine($"Event '{EventHandlerName}' not found in '{TransmitterComponentType.Name}'.");
            return;
        }

        MethodInfo methodInfo = GetType().GetMethod(nameof(OnRecieve), BindingFlags.Public | BindingFlags.Instance);
        if (methodInfo == null)
        {
            Debug.WriteLine("OnRecieve method not found.");
            return;
        }

        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);

        // 👇 Unsubscribe the handler first (no-op if it was never subscribed)
        eventInfo.RemoveEventHandler(component, handler);

        // 👇 Subscribe the handler
        eventInfo.AddEventHandler(component, handler);

        Debug.WriteLine($"Successfully re-subscribed to event '{EventHandlerName}' in '{TransmitterComponentType.Name}'.");
    }

    public virtual void OnRecieve(object? sender, EventArgs e)
    {
        if (gameObject is not null)
        {
            performAction?.Invoke(gameObject);
            OnActionPerformed?.Invoke(this, EventArgs.Empty);
        }
    }
}
