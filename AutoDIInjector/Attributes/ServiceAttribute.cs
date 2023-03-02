using Microsoft.Extensions.DependencyInjection;

namespace AutoDIInjector.Attributes;


[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime ServiceLifetime { get; }
    public Type? ImplementationType { get; set; }

    public ServiceAttribute(ServiceLifetime serviceLifetime, Type? implementationType = null)
    {
        ServiceLifetime = serviceLifetime;
        ImplementationType = implementationType;
    }
}
