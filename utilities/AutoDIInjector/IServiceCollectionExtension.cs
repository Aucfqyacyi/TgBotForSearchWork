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
        if (types.Any() is true)
            return services.RegisterTypesAsync(types, predicate).GetAwaiter().GetResult();
        else
            return services;
    }

    private static async Task<IServiceCollection> RegisterTypesAsync(this IServiceCollection services, IEnumerable<Type> types, Func<Type, bool>? predicate = null)
    {
        await Parallel.ForEachAsync(types, (type, token) =>
        {
            services.RegisterType(type, predicate);
            return ValueTask.CompletedTask;
        });
        return services;
    }

    private static void RegisterType(this IServiceCollection services, Type type, Func<Type, bool>? predicate = null)
    {
        if (type.IsTypeToRegister() is false && (predicate?.Invoke(type) ?? false) is false)
            return;

        var attribute = type.GetCustomAttribute(_serviceAttributeType) as ServiceAttribute;
        lock (_locker)
        {
            Type implementationType = attribute?.ImplementationType ?? type;
            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(type, implementationType, attribute!.ServiceLifetime);
            services.Add(serviceDescriptor);
        }
    }

}
