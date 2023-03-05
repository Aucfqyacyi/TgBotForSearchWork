using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoDIInjector.Attributes;

public class ScopedServiceAttribute : ServiceAttribute
{
    public ScopedServiceAttribute(Type? implementationType = null) : base(ServiceLifetime.Scoped, implementationType)
    {
    }
}
