using System.Reflection;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Extensions;


public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        Type serviceType = typeof(ServiceAttribute);
        IEnumerable<Type> typesToRegister = TypeExtension.GetTypesHaveOneAttribute(serviceType);
        foreach (var type in typesToRegister)
        {
            var attribute = type.GetCustomAttribute(serviceType) as ServiceAttribute;
            services.AddService(type, attribute!.ServiceLifetime, attribute.InterfaceType);
        }
        return services;
    }

    public static void AddService(this IServiceCollection services, Type serviceType, ServiceLifetime lifetime, Type? implementationType = null)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                if(implementationType is null) services.AddSingleton(serviceType);
                else services.AddSingleton(serviceType, implementationType);
                break;
            case ServiceLifetime.Scoped:
                if (implementationType is null) services.AddScoped(serviceType);
                else services.AddScoped(serviceType, implementationType);
                break;
            case ServiceLifetime.Transient:
                if (implementationType is null) services.AddTransient(serviceType);
                else services.AddTransient(serviceType, implementationType);
                break;
        }
    }
}
