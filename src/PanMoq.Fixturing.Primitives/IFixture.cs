namespace PanMoq.Fixturing.Primitives;

public interface IFixture<out TSelf, TComponent, out TComponentInitial>
    : IConfiguratorChain<TSelf, TComponentInitial>
    where TSelf : IFixture<TSelf, TComponent, TComponentInitial>
    where TComponent : IFixtureComponent<TComponentInitial>, new()
{
}

public interface IConfiguratorChain<out TSelf, out TConfiguratorParameter>
    where TSelf : IConfiguratorChain<TSelf, TConfiguratorParameter>
{
    TSelf Configure(Action<TConfiguratorParameter> configurator);
}
