using AutoDIInjector.Attributes;
using System.Reflection;

namespace AutoDIInjector;

internal static class TypeExtension
{
    public static bool IsTypeToRegister(this Type type)
    {
        if (type.IsInterface || type.IsAbstract || type.IsNested || type.IsGenericType || type.IsGenericTypeDefinition)
        {
            return false;
        }
        return type.IsDefined(typeof(ServiceAttribute));
    }
}
