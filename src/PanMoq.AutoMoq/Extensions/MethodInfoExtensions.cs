using System.Reflection;

#nullable enable

namespace PanMoq.AutoMoq.Extensions;

internal static class MethodInfoExtensions
{
    internal static bool IsOverridable(this MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        /*
         * From MSDN (http://goo.gl/WvOgYq)
         *
         * To determine if a method is overridable, it is not sufficient to check that IsVirtual is true.
         * For a method to be overridable, IsVirtual must be true and IsFinal must be false.
         *
         * For example, interface implementations are marked as "virtual final".
         * Methods marked with "override sealed" are also marked as "virtual final".
         */

        return method.IsVirtual && !method.IsFinal;
    }

    internal static bool IsSealed(this MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        return !method.IsOverridable();
    }

    internal static bool IsVoid(this MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        return method.ReturnType == typeof(void);
    }

    internal static bool HasOutParameters(this MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        return method.GetParameters()
                     .Any(p => p.IsOut);
    }

    internal static bool HasRefParameters(this MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        // "out" parameters are also considered "byref", so we have to filter these out
        return method.GetParameters()
                     .Any(p => p.ParameterType.IsByRef && !p.IsOut);
    }
}
