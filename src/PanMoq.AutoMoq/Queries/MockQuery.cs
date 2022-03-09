using System.Reflection;
using AutoFixture.Kernel;

namespace PanMoq.AutoMoq.Queries;

/// <summary>
/// Selects appropriate constructors to create <see cref="Moq.Mock{T}"/> instances.
/// </summary>
public abstract class MockQuery : IMethodQuery
{
    private static readonly DelegateSpecification DelegateSpecification = new();

    public ConstructorPrioritization ConstructorPrioritization { get; }

    internal MockQuery(ConstructorPrioritization constructorPrioritization)
    {
        ConstructorPrioritization = constructorPrioritization;
    }

    /// <summary>
    /// Selects constructors for the supplied <see cref="Moq.Mock{T}"/> type.
    /// </summary>
    /// <param name="mockType">The mock type.</param>
    /// <returns>
    /// Constructors for <paramref name="mockType"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method only returns constructors if <paramref name="mockType"/> is a
    /// <see cref="Moq.Mock{T}"/> type. If not, an empty sequence is returned.
    /// </para>
    /// <para>
    /// If the type is the type of a constructed <see cref="Moq.Mock{T}"/>, constructors are
    /// returned according to the generic type argument's constructors. If the type is an
    /// interface, the <see cref="Moq.Mock{T}()"/> default constructor is returned. If the type
    /// is a class, constructors are returned according to all the public and protected
    /// constructors of the underlying type. In this case, the
    /// <see cref="Moq.Mock{T}(object[])"/> constructor that takes a params array is returned
    /// for each underlying constructor, with information about the appropriate parameters for
    /// each constructor.
    /// </para>
    /// </remarks>
    public IEnumerable<IMethod> SelectMethods(Type mockType)
    {
        if (mockType == null) throw new ArgumentNullException(nameof(mockType));

        if (!mockType.IsMock())
        {
            return Enumerable.Empty<IMethod>();
        }

        var mockedType = mockType.GetMockedType();
        if (mockedType.GetTypeInfo().IsInterface || DelegateSpecification.IsSatisfiedBy(mockedType))
        {
            return new[] { CreateMockForConstructorlessAbstraction(mockType) };
        }

        return ConstructorPrioritization switch
        {
            ConstructorPrioritization.Parsimonious =>
                from ci in mockedType.GetPublicAndProtectedConstructors()
                let parameterCount = ci.GetParameters().Length
                orderby parameterCount ascending
                select CreateMockViaNonDefaultConstructor(mockType, ci),
            ConstructorPrioritization.Greedy =>
                from ci in mockedType.GetPublicAndProtectedConstructors()
                let parameterCount = ci.GetParameters().Length
                orderby parameterCount descending
                select CreateMockViaNonDefaultConstructor(mockType,  ci),
            _ => throw new InvalidOperationException()
        };
    }

    protected abstract IMethod CreateMockForConstructorlessAbstraction(Type mockType);

    protected abstract IMethod CreateMockViaNonDefaultConstructor(Type mockType, ConstructorInfo mockedTypeConstructor);
}
