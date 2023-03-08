using Microsoft.Extensions.DependencyInjection;

namespace AutoDIInjector.Attributes;

public class ScopedServiceAttribute : ServiceAttribute
{
    public ScopedServiceAttribute(Type? implementationType = null) : base(ServiceLifetime.Scoped, implementationType)
    {
    }
}
