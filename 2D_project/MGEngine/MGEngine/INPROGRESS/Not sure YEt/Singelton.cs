public abstract class Singleton<T> where T : class, new()
{
    // The singleton instance
    private static T instance;

    // Lock object for thread safety
    private static readonly object lockObject = new object();

    // Protected constructor to prevent direct instantiation from outside
    protected Singleton() { }

    // Public property to access the singleton instance
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Lock to ensure only one thread can enter this block
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        // Use reflection to invoke the private constructor
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }

    public void Initialize()
    {
        // Initialization code here
    }
}


