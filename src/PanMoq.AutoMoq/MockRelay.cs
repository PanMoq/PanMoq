using AutoFixture.Kernel;
using Moq;

#nullable disable

namespace PanMoq.AutoMoq;

/// <summary>
/// Relays a request for an interface or an abstract class to a request for a
/// <see cref="Mock{T}"/> of that class.
/// </summary>
public class MockRelay : ISpecimenBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockRelay"/> class with a specification
    /// that determines whether a type should be mocked.
    /// </summary>
    /// <param name="mockableSpecification">
    /// A specification that determines whether a type should be mocked or not.
    /// </param>
    internal MockRelay(IRequestSpecification mockableSpecification)
    {
        MockableSpecification = mockableSpecification ?? throw new ArgumentNullException(nameof(mockableSpecification));
    }

    /// <summary>
    /// Gets a specification that determines whether a given request should
    /// be mocked.
    /// </summary>
    /// <value>The specification.</value>
    /// <remarks>
    /// <para>
    /// This specification determines whether a given type should be
    /// relayed as a request for a mock of the same type. By default it
    /// only returns <see langword="true"/> for interfaces and abstract
    /// classes, but a different specification can be supplied by using the
    /// overloaded constructor that takes an
    /// <see cref="IRequestSpecification" /> as input. In that case, this
    /// property returns the specification supplied to the constructor.
    /// </para>
    /// </remarks>
    /// <seealso cref="MockRelay(IRequestSpecification)" />
    public IRequestSpecification MockableSpecification { get; }

    /// <summary>
    /// Creates a new specimen based on a request.
    /// </summary>
    /// <param name="request">The request that describes what to create.</param>
    /// <param name="context">A context that can be used to create other specimens.</param>
    /// <returns>
    /// A dynamic mock instance of the requested interface or abstract class if possible;
    /// otherwise a <see cref="NoSpecimen"/> instance.
    /// </returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (!MockableSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        if (request is not Type t)
            return new NoSpecimen();

        var result = ResolveMock(t, context);

        // Note: null is a valid specimen (e.g., returned by NullRecursionHandler)
        if (result is NoSpecimen or OmitSpecimen or null)
            return result;

        if (result is not Mock m)
            return new NoSpecimen();

        return m.Object;
    }

    private static object ResolveMock(Type t, ISpecimenContext context)
    {
        var mockType = typeof(Mock<>).MakeGenericType(t);
        return context.Resolve(mockType);
    }
}
