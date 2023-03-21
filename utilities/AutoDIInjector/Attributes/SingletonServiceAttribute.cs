using Microsoft.Extensions.DependencyInjection;

namespace AutoDIInjector.Attributes;

public class SingletonServiceAttribute : ServiceAttribute
{
    public SingletonServiceAttribute(Type? interfaceType = null) : base(ServiceLifetime.Singleton, interfaceType)
    {
    }
}
