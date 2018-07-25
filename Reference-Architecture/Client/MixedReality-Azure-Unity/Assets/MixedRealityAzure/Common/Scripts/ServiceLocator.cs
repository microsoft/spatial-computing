using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static IDictionary<object, object> InternalServices = new Dictionary<object, object>();

    public static void RegisterService<TInterface, TImplementation>(Action<IDictionary<object, object>> registrationCallback)
    {
        registrationCallback(InternalServices);
    }

    public static T GetService<T>()
    {
        try
        {
            return (T)InternalServices[typeof(T)];
        }
        catch (KeyNotFoundException)
        {
            throw new ApplicationException("The requested service is not registered");
        }
    }
}
