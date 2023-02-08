namespace TgBotForSearchWorkApi.Utilities.Attributes;

public class ScopedServiceAttribute : ServiceAttribute
{
    public ScopedServiceAttribute(Type? interfaceType = null) : base(ServiceLifetime.Scoped, interfaceType)
    {
    }
}
