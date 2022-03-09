namespace PanMoq.Fixturing.Primitives;

public sealed record ComponentCombo<TComponent1, TComponent2, TComponent1Initial, TComponent2Initial, TComponent1Final, TComponent2Final>()
    : IFixtureComponent<ValueTuple<TComponent1Initial, TComponent2Initial>, ValueTuple<TComponent1Final, TComponent2Final>>
    where TComponent1 : IFixtureComponent<TComponent1Initial, TComponent1Final>, new()
    where TComponent2 : IFixtureComponent<TComponent2Initial, TComponent2Final>, new()
{
    private TComponent1 Component1 { get; init; } = new TComponent1();
    private TComponent2 Component2 { get; init; } = new TComponent2();

    public (TComponent1Initial, TComponent2Initial) CreateInitial()
    {
        return (Component1.CreateInitial(), Component2.CreateInitial());
    }

    public (TComponent1Final, TComponent2Final) BuildFinal((TComponent1Initial, TComponent2Initial) initial)
    {
        return (Component1.BuildFinal(initial.Item1), Component2.BuildFinal(initial.Item2));
    }
}

public sealed record ComponentCombo<TComponent1, TComponent2, TComponent3, TComponent1Initial, TComponent2Initial, TComponent3Initial, TComponent1Final, TComponent2Final, TComponent3Final>()
    : IFixtureComponent<ValueTuple<TComponent1Initial, TComponent2Initial, TComponent3Initial>, ValueTuple<TComponent1Final, TComponent2Final, TComponent3Final>>
    where TComponent1 : IFixtureComponent<TComponent1Initial, TComponent1Final>, new()
    where TComponent2 : IFixtureComponent<TComponent2Initial, TComponent2Final>, new()
    where TComponent3 : IFixtureComponent<TComponent3Initial, TComponent3Final>, new()
{
    private TComponent1 Component1 { get; init; } = new TComponent1();
    private TComponent2 Component2 { get; init; } = new TComponent2();
    private TComponent3 Component3 { get; init; } = new TComponent3();

    public (TComponent1Initial, TComponent2Initial, TComponent3Initial) CreateInitial()
    {
        return (Component1.CreateInitial(), Component2.CreateInitial(), Component3.CreateInitial());
    }

    public (TComponent1Final, TComponent2Final, TComponent3Final) BuildFinal((TComponent1Initial, TComponent2Initial, TComponent3Initial) initial)
    {
        return (Component1.BuildFinal(initial.Item1), Component2.BuildFinal(initial.Item2), Component3.BuildFinal(initial.Item3));
    }
}
