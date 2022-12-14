using AutoFixture.Kernel;
using Moq;
using System.Diagnostics.CodeAnalysis;
using PanMoq.AutoMoq.Extensions;

#nullable disable

namespace PanMoq.AutoMoq;

/// <summary>
/// Post-processes a <see cref="Mock{T}"/> instance by setting appropriate default behavioral
/// values.
/// </summary>
public class MockPostprocessor : ISpecimenBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockPostprocessor"/> class with the
    /// supplied <see cref="ISpecimenBuilder"/>.
    /// </summary>
    /// <param name="builder">
    /// The builder which is expected to create <see cref="Mock{T}"/> instances.
    /// </param>
    internal MockPostprocessor(ISpecimenBuilder builder)
    {
        Builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Gets the builder decorated by this instance.
    /// </summary>
    public ISpecimenBuilder Builder { get; }

    /// <summary>
    /// Modifies a <see cref="Mock{T}"/> instance created by <see cref="Builder"/>.
    /// </summary>
    /// <param name="request">The request that describes what to create.</param>
    /// <param name="context">A context that can be used to create other specimens.</param>
    /// <returns>
    /// The specimen created by <see cref="Builder"/>. If the instance is a correct
    /// <see cref="Mock{T}"/> instance, this instance modifies it before returning it.
    /// </returns>
    public object Create(object request, ISpecimenContext context)
    {
        var t = request as Type;
        if (!t.IsMock())
        {
            return new NoSpecimen();
        }

        var specimen = Builder.Create(request, context);
        if (specimen is NoSpecimen or OmitSpecimen or null)
            return specimen;

        if (specimen is not Mock m)
        {
            return new NoSpecimen();
        }

        var mockType = t.GetMockedType();
        if (m.GetType().GetMockedType() != mockType)
        {
            return new NoSpecimen();
        }

        var configurator = (IMockConfigurator)Activator.CreateInstance(typeof(MockConfigurator<>).MakeGenericType(mockType))!;
        configurator.Configure(m);

        return m;
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "It's activated via reflection.")]
    private class MockConfigurator<T> : IMockConfigurator
        where T : class
    {
        public void Configure(Mock mock) => Configure((Mock<T>)mock);

        private static void Configure(Mock<T> mock)
        {
            mock.CallBase = ShouldUseCallBase(typeof(T));
            mock.DefaultValue = DefaultValue.Mock;
        }

        private static bool ShouldUseCallBase(Type type) => !type.IsDelegate();
    }

    private interface IMockConfigurator
    {
        void Configure(Mock mock);
    }
}
