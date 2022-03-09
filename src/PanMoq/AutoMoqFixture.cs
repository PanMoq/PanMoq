using System.Collections.Immutable;
using AutoFixture;
using Moq;
using PanMoq.AutoMoq;
using PanMoq.AutoMoq.Extensions;
using PanMoq.Fixturing.Primitives;

namespace PanMoq;

public record AutoMoqFixture
    : IFixture<AutoMoqFixture, PanMoq.Fixturing.Moq.Moq, MockRepository>,
      IFixture<AutoMoqFixture, PanMoq.Fixturing.AutoFixture.AutoFixture, Fixture>,
      IFixture<AutoMoqFixture, ComponentCombo<PanMoq.Fixturing.Moq.Moq, PanMoq.Fixturing.AutoFixture.AutoFixture, MockRepository, Fixture, MockRepository, Fixture>, ValueTuple<MockRepository, Fixture>>,
      ISpecimenProvider
{
    private ImmutableList<Action<AutoMoqFixtureComponentsState>> Configurators { get; init; }
    private Lazy<AutoMoqFixtureComponentsState> LazyFinal { get; init; }

    public AutoMoqFixture()
    {
        Configurators = ImmutableList<Action<AutoMoqFixtureComponentsState>>.Empty;
        LazyFinal = BuildLazy(Configurators);
    }

    public AutoMoqFixture Configure(Action<MockRepository> configurator)
    {
        void AdaptConfigurator(AutoMoqFixtureComponentsState componentInitials) =>
            configurator(componentInitials.MockRepository);

        return Configure(AdaptConfigurator);
    }

    public AutoMoqFixture Configure(Action<Fixture> configurator)
    {
        void AdaptConfigurator(AutoMoqFixtureComponentsState componentInitials) =>
            configurator(componentInitials.Fixture);

        return Configure(AdaptConfigurator);
    }

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

    public TSpecimen CreateSpecimen<TSpecimen>() where TSpecimen : notnull =>
        LazyFinal.Value.Fixture.Create<TSpecimen>();

    AutoMoqFixture IConfiguratorChain<AutoMoqFixture, (MockRepository, Fixture)>.Configure(Action<(MockRepository, Fixture)> configurator) =>
        Configure(componentsState => configurator((componentsState.MockRepository, componentsState.Fixture)));
}
