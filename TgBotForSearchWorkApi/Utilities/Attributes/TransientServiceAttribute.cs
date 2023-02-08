namespace TgBotForSearchWorkApi.Utilities.Attributes;

public class TransientServiceAttribute : ServiceAttribute
{
    public TransientServiceAttribute(Type? interfaceType = null) : base(ServiceLifetime.Transient, interfaceType)
    {
    }

}
