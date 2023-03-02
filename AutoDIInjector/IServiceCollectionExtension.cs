using AutoDIInjector.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoDIInjector;


public static class IServiceCollectionExtension
{
    private static readonly Type _serviceAttributeType = typeof(ServiceAttribute);
    private static readonly object _locker = new object();

    public static IServiceCollection AddServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddServices(null, assemblies);
    }

    public static IServiceCollection AddServices(this IServiceCollection services, Func<Type, bool>? predicate, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            return services.RegisterTypes(Assembly.GetCallingAssembly().GetExportedTypes(), predicate);
        else
            return services.RegisterTypes(assemblies.SelectMany(assem => assem.GetExportedTypes()), predicate);
    }

    private static IServiceCollection RegisterTypes(this IServiceCollection services, IEnumerable<Type> types, Func<Type, bool>? predicate)
    {
        Parallel.ForEach(types, type =>
        {
            if (type.IsTypeToRegister() && (predicate?.Invoke(type) ?? true))
            {
                var attribute = type.GetCustomAttribute(_serviceAttributeType) as ServiceAttribute;
                lock(_locker)
                    services.Add(new ServiceDescriptor(type, attribute?.ImplementationType ?? type, attribute!.ServiceLifetime));
            }
        });
        return services;
    }
}
