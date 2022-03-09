using AutoFixture.Kernel;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using System.Globalization;
using System.Reflection;

#nullable enable

namespace PanMoq.AutoMoq;

/// <summary>
/// Contains extension methods to manipulate/setup instances of <see cref="Mock{T}"/>.
/// </summary>
internal static class MockType
{
    internal static bool IsMock(this Type? type) =>
        type != null
        && type.GetTypeInfo().IsGenericType
        && typeof(Mock<>).IsAssignableFrom(type.GetGenericTypeDefinition())
        && !type.GetMockedType().IsGenericParameter;

    internal static ConstructorInfo? GetDefaultConstructor(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        return type.GetConstructor(Type.EmptyTypes);
    }

    internal static IEnumerable<ConstructorInfo> GetPublicAndProtectedConstructors(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(ctor => !ctor.IsPrivate);
    }

    internal static ConstructorInfo? GetParamsConstructor(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        return type.GetConstructor(new[] { typeof(object[]) });
    }

    internal static Type GetMockedType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        return type.GetTypeInfo().GetGenericArguments().Single();
    }

    internal static IReturnsResult<TMock> ReturnsUsingContext<TMock, TResult>(this IReturns<TMock, TResult> setup, ISpecimenContext context)
        where TMock : class
    {
        if (setup == null) throw new ArgumentNullException(nameof(setup));
        if (context == null) throw new ArgumentNullException(nameof(context));

        return setup.Returns(() =>
        {
            var specimen = context.Resolve(typeof(TResult));

            // check if specimen is null but member is non-nullable value type
            if (specimen == null && (default(TResult) != null))
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Tried to setup a member with a return type of {0}, but null was found instead.",
                        typeof(TResult)));
            }

            // check if specimen can be safely converted to TResult
            if (specimen != null && specimen is not TResult)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Tried to setup a member with a return type of {0}, but an instance of {1} was found instead.",
                        typeof(TResult),
                        specimen.GetType()));
            }

            var result = (TResult)specimen!;

            // "cache" value for future invocations
            setup.Returns(result!);
            return result!;
        });
    }
}
