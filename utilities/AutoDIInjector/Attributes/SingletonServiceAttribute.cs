using Microsoft.Extensions.DependencyInjection;

namespace AutoDIInjector.Attributes;

public class SingletonServiceAttribute : ServiceAttribute
{
    public SingletonServiceAttribute(Type? implementationType = null) : base(ServiceLifetime.Singleton, implementationType)
    {
    }
}
