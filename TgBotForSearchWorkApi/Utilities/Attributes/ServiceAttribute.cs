namespace TgBotForSearchWorkApi.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime ServiceLifetime { get; }
    public Type? InterfaceType { get; set; }

    public ServiceAttribute(ServiceLifetime serviceLifetime, Type? interfaceType = null)
    {
        ServiceLifetime = serviceLifetime;
        InterfaceType = interfaceType;
    }
}
