using System.Reflection;

#nullable enable

namespace PanMoq.AutoMoq.Extensions;

internal static class TypeExtensions
{
    /// <summary>
    /// Gets a collection of all methods declared by the type or any of its base interfaces.
    /// </summary>
    internal static IEnumerable<MethodInfo> GetAllMethods(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        IEnumerable<MethodInfo> result = type.GetMethods();

        // If "type" is an interface, "GetMethods" does not return methods declared on other interfaces extended by "type".
        if (type.GetTypeInfo().IsInterface)
            result = result.Concat(type.GetInterfaces().SelectMany(x => x.GetMethods()));

        return result;
    }

    /// <summary>
    /// Gets a collection of all properties declared by the type or any of its base interfaces.
    /// </summary>
    internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        IEnumerable<PropertyInfo> result = type.GetProperties();

        // If "type" is an interface, "GetProperties" does not return methods declared on other interfaces extended by "type".
        if (type.GetTypeInfo().IsInterface)
            result = result.Concat(type.GetInterfaces().SelectMany(x => x.GetProperties()));

        return result;
    }

    /// <summary>
    /// Returns whether or not a type represents a delegate.
    /// </summary>
    internal static bool IsDelegate(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        return typeof(MulticastDelegate).IsAssignableFrom(type.GetTypeInfo().BaseType);
    }
}
