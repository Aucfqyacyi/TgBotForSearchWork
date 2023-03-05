using Microsoft.Extensions.DependencyInjection;

namespace AutoDIInjector.Attributes;

public class TransientServiceAttribute : ServiceAttribute
{
    public TransientServiceAttribute(Type? implementationType = null) : base(ServiceLifetime.Transient, implementationType)
    {
    }

}
