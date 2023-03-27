using AutoDIInjector.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoDIInjector;


public static class IServiceCollectionExtension
{
    private static readonly Type _serviceAttributeType = typeof(ServiceAttribute);

    public static IServiceCollection AddServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            return services.RegisterTypes(Assembly.GetCallingAssembly().GetExportedTypes());
        else
            return services.RegisterTypes(assemblies.SelectMany(assem => assem.GetExportedTypes()));
    }

    public static IServiceCollection AddServices(this IServiceCollection services, Func<Type, bool> predicate, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            return services.RegisterTypes(Assembly.GetCallingAssembly().GetExportedTypes(), predicate);
        else
            return services.RegisterTypes(assemblies.SelectMany(assem => assem.GetExportedTypes()), predicate);
    }

    private static IServiceCollection RegisterTypes(this IServiceCollection services, IEnumerable<Type> types, Func<Type, bool>? predicate = null)
    {
        foreach (var type in types)
        {
            if (type.IsTypeToRegister() && (predicate?.Invoke(type) ?? true))
            {
                var attribute = type.GetCustomAttribute(_serviceAttributeType) as ServiceAttribute;
                Type interfaceType = attribute!.InterfaceType ?? type;
                ServiceDescriptor serviceDescriptor = new ServiceDescriptor(interfaceType, type, attribute!.ServiceLifetime);
                services.Add(serviceDescriptor);
            }
        }
        return services;
    }
}
