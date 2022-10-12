using System.Collections.Immutable;
using AutoFixture;
using Moq;
using PanMoq.AutoMoq;
using PanMoq.AutoMoq.Extensions;
using PanMoq.Fixturing.Primitives;

namespace PanMoq;

/// <summary>
/// An auto-mocking container for creating test specimens.
/// </summary>
public record AutoMoqFixture
    : IFixture<AutoMoqFixture, PanMoq.Fixturing.Moq.Moq, MockRepository>,
      IFixture<AutoMoqFixture, PanMoq.Fixturing.AutoFixture.AutoFixture, Fixture>,
      IFixture<AutoMoqFixture, ComponentCombo<PanMoq.Fixturing.Moq.Moq, PanMoq.Fixturing.AutoFixture.AutoFixture, MockRepository, Fixture, MockRepository, Fixture>, ValueTuple<MockRepository, Fixture>>,
      ISpecimenProvider,
      ISpecimenBroker
{
    private ImmutableList<Action<AutoMoqFixtureComponentsState>> Configurators { get; init; }
    private Lazy<AutoMoqFixtureComponentsState> LazyFinal { get; init; }

    /// <summary>
    /// Instantiates a new instance of <see cref="AutoMoqFixture"/> with default configuration.
    /// </summary>
    public AutoMoqFixture()
    {
        Configurators = ImmutableList<Action<AutoMoqFixtureComponentsState>>.Empty;
        LazyFinal = BuildLazy(Configurators);
    }

    /// <summary>
    /// Creates and returns a new instance of <see cref="AutoMoqFixture"/> which has the configuration of the current instance combined with the provided <paramref name="configurator"/>.
    /// </summary>
    /// <param name="configurator">A delegate that applies configuration to the <see cref="MockRepository"/> component of the fixture.</param>
    /// <returns>An instance of <see cref="AutoMoqFixture"/> with amended configuration.</returns>
    public AutoMoqFixture Configure(Action<MockRepository> configurator)
    {
        void AdaptConfigurator(AutoMoqFixtureComponentsState componentInitials) =>
            configurator(componentInitials.MockRepository);

        return Configure(AdaptConfigurator);
    }

    /// <summary>
    /// Creates and returns a new instance of <see cref="AutoMoqFixture"/> which has the configuration of the current instance combined with the provided <paramref name="configurator"/>.
    /// </summary>
    /// <param name="configurator">A delegate that applies configuration to the <see cref="Fixture"/> component of the fixture.</param>
    /// <returns>An instance of <see cref="AutoMoqFixture"/> with amended configuration.</returns>
    public AutoMoqFixture Configure(Action<Fixture> configurator)
    {
        void AdaptConfigurator(AutoMoqFixtureComponentsState componentInitials) =>
            configurator(componentInitials.Fixture);

        return Configure(AdaptConfigurator);
    }

    /// <summary>
    /// Creates and returns a new instance of <see cref="AutoMoqFixture"/> which has the configuration of the current instance combined with the provided <paramref name="configurator"/>.
    /// </summary>
    /// <param name="configurator">A delegate that applies configuration to the components of the fixture.</param>
    /// <returns>An instance of <see cref="AutoMoqFixture"/> with amended configuration.</returns>
    public AutoMoqFixture Configure(Action<AutoMoqFixtureComponentsState> configurator)
    {
        var newConfigurators = Configurators.Add(configurator);
        return
            this with
            {
                Configurators = newConfigurators,
                LazyFinal = BuildLazy(newConfigurators)
            };
    }

    protected virtual void BuildBeforeFirstConfigurator(AutoMoqFixtureComponentsState componentStates)
    {
        var (mockRepository, autofixtureFixture) = componentStates;
        mockRepository.GetDefaultReturnValuesFrom(autofixtureFixture);
        autofixtureFixture.CustomizeWithAutomaticMocks(CreateAutoMoqCustomization(mockRepository));
        autofixtureFixture.Register(() => mockRepository);
    }

    protected virtual void BuildAfterFinalConfigurator(AutoMoqFixtureComponentsState componentStates)
    {
    }

    private AutoMoqFixtureComponentsState Build(ImmutableList<Action<AutoMoqFixtureComponentsState>> configurators)
    {
        var components = CreateComponents();
        var componentStates = CreateInitialComponentsState(components);

        BuildBeforeFirstConfigurator(componentStates);
        foreach (var configurator in configurators)
        {
            configurator(componentStates);
        }
        BuildAfterFinalConfigurator(componentStates);

        return BuildFinalComponentsState(components, componentStates);
    }

    private Lazy<AutoMoqFixtureComponentsState> BuildLazy(ImmutableList<Action<AutoMoqFixtureComponentsState>> configurators) =>
        new(Build(configurators));

    protected virtual AutoMoqFixtureComponentsState BuildFinalComponentsState(AutoMoqFixtureComponents components, AutoMoqFixtureComponentsState componentsState)
    {
        var (moqComponent, autoFixtureComponent) = components;
        return new(moqComponent.BuildFinal(componentsState.MockRepository), autoFixtureComponent.BuildFinal(componentsState.Fixture));
    }

    protected virtual AutoMoqFixtureComponentsState CreateInitialComponentsState(AutoMoqFixtureComponents components)
    {
        var (moqComponent, autoFixtureComponent) = components;
        return new(moqComponent.CreateInitial(), autoFixtureComponent.CreateInitial());
    }

    protected virtual AutoMoqFixtureComponents CreateComponents() =>
        new(new(), new());

    protected virtual AutoMoqCustomization CreateAutoMoqCustomization(MockRepository mockRepository) =>
        new(mockRepository) { ConfigureMembers = true, GenerateDelegates = true };

    AutoMoqFixture IConfiguratorChain<AutoMoqFixture, (MockRepository, Fixture)>.Configure(Action<(MockRepository, Fixture)> configurator) =>
        Configure(componentsState => configurator((componentsState.MockRepository, componentsState.Fixture)));

    /// <inheritdoc/>
    public TSpecimen CreateSpecimen<TSpecimen>() where TSpecimen : notnull =>
        LazyFinal.Value.Fixture.Create<TSpecimen>();

    /// <inheritdoc/>
    public void UseSpecimen<TSpecimen>(Action<TSpecimen> specimenAction) where TSpecimen : notnull =>
        (specimenAction ?? throw new ArgumentNullException(nameof(specimenAction))).Invoke(CreateSpecimen<TSpecimen>());

    /// <inheritdoc/>
    public TResult UseSpecimen<TSpecimen, TResult>(Func<TSpecimen, TResult> specimenAction) where TSpecimen : notnull =>
        (specimenAction ?? throw new ArgumentNullException(nameof(specimenAction))).Invoke(CreateSpecimen<TSpecimen>());
}
