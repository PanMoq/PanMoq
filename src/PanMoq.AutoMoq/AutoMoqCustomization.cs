using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using PanMoq.AutoMoq.Commands;
using PanMoq.AutoMoq.Queries;

#nullable enable

namespace PanMoq.AutoMoq;

/// <summary>
/// Enables auto-mocking with Moq.
/// </summary>
/// <remarks>
/// NOTICE! You can assign the customization properties to tweak the features you would like to enable. See example.
/// <br />
/// <code>new AutoMoqCustomization { ConfigureMembers = true }</code>
/// </remarks>
public record AutoMoqCustomization : ICustomization
{
    private readonly MockRepository? _mockRepository;
    private readonly ISpecimenBuilder _defaultRelay;
    private readonly ISpecimenBuilder _relayForDelegates;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMoqCustomization"/> class.
    /// <para>
    /// NOTICE! You can assign the customization properties to tweak the features you would like to enable. Example:
    /// <br />
    /// <code>new AutoMoqCustomization { ConfigureMembers = true }</code>
    /// </para>
    /// </summary>
    public AutoMoqCustomization(MockRepository? mockRepository = null)
    {
        _mockRepository = mockRepository;
        _defaultRelay = MockRelayFactory.CreateForClassesOrInterfaces();
        _relayForDelegates = MockRelayFactory.CreateForDelegates();
    }

    /// <summary>
    /// Specifies whether members of a mock will be automatically setup to retrieve the return values from a fixture.
    /// </summary>
    public bool ConfigureMembers { get; init; } = true;

    /// <summary>
    /// If value is <c>true</c>, delegate requests are intercepted and created by Moq.
    /// Otherwise, if value is <c>false</c>, delegates are created by the AutoFixture kernel.
    /// </summary>
    public bool GenerateDelegates { get; init; } = true;

    /// <summary>
    /// Gets or sets whether or not constructor selection is greedy. Default is not greedy.
    /// </summary>
    public ConstructorPrioritization ConstructorPrioritization { get; init; } = ConstructorPrioritization.Parsimonious;

        /// <summary>
    /// Customizes an <see cref="IFixture"/> to enable auto-mocking with Moq.
    /// </summary>
    /// <param name="fixture">The fixture upon which to enable auto-mocking.</param>
    public void Customize(IFixture fixture)
    {
        if (fixture == null) throw new ArgumentNullException(nameof(fixture));

        IMethodQuery mockCreationMethodQuery =
            _mockRepository == null
                ? new MockConstructorQuery(ConstructorPrioritization)
                : new MockRepositoryFactoryMethodQuery(ConstructorPrioritization, _mockRepository);

        ISpecimenBuilder mockBuilder = new MockPostprocessor(new MethodInvoker(mockCreationMethodQuery));

        // If members should be automatically configured, wrap the builder with members setup postprocessor.
        if (ConfigureMembers)
        {
            mockBuilder = new Postprocessor(
                builder: mockBuilder,
                command: new CompositeSpecimenCommand(
                            new StubPropertiesCommand(),
                            new MockVirtualMethodsCommand(),
                            new AutoMockPropertiesCommand()));
        }

        fixture.Customizations.Insert(0, mockBuilder);
        fixture.ResidueCollectors.Insert(0, _defaultRelay);

        if (GenerateDelegates)
        {
            fixture.Customizations.Insert(1, _relayForDelegates);
        }
    }
}
