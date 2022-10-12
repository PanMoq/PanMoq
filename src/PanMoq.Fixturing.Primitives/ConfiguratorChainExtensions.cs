namespace PanMoq.Fixturing.Primitives;

public static class ConfiguratorChainExtensions
{
    public static TSelf Configure<TSelf, TParameter1, TParameter2>(this IConfiguratorChain<TSelf, ValueTuple<TParameter1, TParameter2>> self, Action<TParameter1, TParameter2> configurator)
        where TSelf : IConfiguratorChain<TSelf, ValueTuple<TParameter1, TParameter2>> =>
        (self ?? throw new ArgumentNullException(nameof(self))).Configure(t => configurator(t.Item1, t.Item2));

    public static TSelf Configure<TSelf, TParameter1, TParameter2, TParameter3>(this IConfiguratorChain<TSelf, ValueTuple<TParameter1, TParameter2, TParameter3>> self, Action<TParameter1, TParameter2, TParameter3> configurator)
        where TSelf : IConfiguratorChain<TSelf, ValueTuple<TParameter1, TParameter2, TParameter3>> =>
        (self ?? throw new ArgumentNullException(nameof(self))).Configure(t => configurator(t.Item1, t.Item2, t.Item3));
}
