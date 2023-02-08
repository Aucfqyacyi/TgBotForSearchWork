using System.Reflection;

namespace TgBotForSearchWorkApi.Extensions;

public static class TypeExtension
{
    /// <summary>
    /// Return all fields in this type, that have binding flags: Instance, NonPublic, Public, Static.
    /// </summary>
    /// <returns>An array of System.Reflection.FieldInfo objects representing all fields defined
    ///     for the current System.Type that match the specified binding constraints. -or-
    ///     An empty array of type System.Reflection.FieldInfo, if no fields are defined
    ///     for the current System.Type, or if none of the defined fields match the binding
    ///     constraints.</returns>
    public static FieldInfo[] GetAllFields(this Type type)
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
    }

    /// <summary>
    /// Return all types, that have one from base types or interfaces.
    /// </summary>
    /// <returns>A collection that contains all the types that are defined by one from those base types.</returns>
    public static IEnumerable<Type> GetTypesHaveOneBaseType(params string[] baseTypeNames)
    {
        return GetTypes(CreatePredicateOr(HasBaseTypeOrInterface, baseTypeNames));
    }

    /// <summary>
    /// Return all types, that have all base types or interfaces.
    /// </summary>
    /// <returns>A collection that contains all the types that are defined by those base types.</returns>
    public static IEnumerable<Type> GetTypesHaveAllBaseTypes(params string[] baseTypeNames)
    {
        return GetTypes(CreatePredicateAnd(HasBaseTypeOrInterface, baseTypeNames));
    }

    public static IEnumerable<Type> GetTypesHaveAllAttributes(params Type[] attributeTypes)
    {
        return GetTypes(CreatePredicateAnd(HasAttribute, attributeTypes));
    }

    public static IEnumerable<Type> GetTypesHaveOneAttribute(params Type[] attributeTypes)
    {
        return GetTypes(CreatePredicateOr(HasAttribute, attributeTypes));
    }

    /// <summary>
    /// Search a generic type in base types.
    /// </summary>
    /// <returns>True if base type has this generic type, otherwise false.</returns>
    public static bool HasBaseTypeGenericType(this Type? type, Type genericType)
    {
        while ((type = type?.BaseType) is not null)
        {
            if (type.GenericTypeArguments.Contains(genericType))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Сheck if hierarchy of type has base type or interface by its name.
    /// </summary>
    /// <returns>True if this type has base type or interface with this the name, otherwise false.</returns>
    public static bool HasBaseTypeOrInterface(this Type type, string baseTypeName)
    {
        if (HasInterface(type, baseTypeName))
        {
            return true;
        }
        Type? nullAbleType = type;
        do
        {
            if (HasBaseType(nullAbleType!, baseTypeName))
            {
                return true;
            }
        } while ((nullAbleType = nullAbleType?.BaseType) is not null);
        return false;
    }

    /// <summary>
    /// Сheck if hierarchy of type has base type or interface by their names.
    /// </summary>
    /// <returns> Collection, that contains the results by the check of hierarchy of type on availability baseTypeNames. </returns>
    public static IEnumerable<bool> HasBaseTypeOrInterface(this Type type, params string[] baseTypeNames)
    {
        return HasBaseTypeOrInterface(type, baseTypeNames);
    }

    public static IEnumerable<bool> HasBaseTypeOrInterface(this Type type, IEnumerable<string> baseTypeNames)
    {
        Type? nullAbleType = type;
        Dictionary<string, bool> isBaseTypeNames =
            baseTypeNames.Aggregate(new Dictionary<string, bool>(), (dictionary, baseTypeName) =>
            {
                dictionary.Add(baseTypeName, type.HasInterface(baseTypeName));
                return dictionary;
            });
        if (isBaseTypeNames.Values.Contains(false))
        {
            do
            {
                foreach (var baseTypeName in baseTypeNames)
                {
                    if (isBaseTypeNames[baseTypeName] is false && HasBaseType(nullAbleType!, baseTypeName))
                    {
                        isBaseTypeNames[baseTypeName] = true;
                    }
                }
            } while ((nullAbleType = nullAbleType?.BaseType) is not null);
        }
        return isBaseTypeNames.Values;
    }

    public static IEnumerable<bool> HasAttribute(this Type type, params Type[] attributeTypes)
    {
        return HasAttribute(type, attributeTypes);
    }

    public static IEnumerable<bool> HasAttribute(this Type type, IEnumerable<Type> attributeTypes)
    {
        return attributeTypes.Aggregate(new List<bool>(), (hasTypeAttribute, attributeType) =>
        {
            hasTypeAttribute.Add(type.HasAttribute(attributeType));
            return hasTypeAttribute;
        });        
    }

    public static bool HasBaseType(this Type type, string baseTypeName)
    {
        return type.BaseType?.Name.Contains(baseTypeName) ?? false;
    }

    public static bool HasInterface(this Type type, string interfaceName)
    {
        return type.GetInterface(interfaceName) is not null;
    }

    public static bool HasAttribute(this Type type, Type attributeType)
    {
        return type.GetCustomAttribute(attributeType) is not null;
    }

    public static IEnumerable<Type> GetTypes(Func<Type, bool> predicate)
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        return callingAssembly.GetTypes().Where(predicate);
    }

    private static Func<Type, bool> CreatePredicateOr<TValue>(Func<Type, IEnumerable<TValue>, IEnumerable<bool>> func, IEnumerable<TValue> values)
    {
        return CreatePredicate(func, Enumerable.Contains, true, values);
    }

    private static Func<Type, bool> CreatePredicateAnd<TValue>(Func<Type, IEnumerable<TValue>, IEnumerable<bool>> func, IEnumerable<TValue> values)
    {
        return CreatePredicate(func, IEnumerableExtension.NotContains, false, values);
    }

    private static Func<Type, bool> CreatePredicate<TValue>(Func<Type, IEnumerable<TValue>, IEnumerable<bool>> func, 
                                                            Func<IEnumerable<bool>, bool, bool> predicate, 
                                                            bool itemForPredicate, 
                                                            IEnumerable<TValue> values)
    {
        return (Type type) =>
        {
            if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsGenericTypeDefinition)
            {
                return false;
            }
            return predicate(func(type, values), itemForPredicate);
        };
    }
}
