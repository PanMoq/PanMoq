using Moq;
using PanMoq.Fixturing.Primitives;

namespace PanMoq.Fixturing.Moq;

public record Moq : IFixtureComponent<MockRepository, MockRepository>
{
    public virtual MockRepository CreateInitial() =>
        new(MockBehavior.Default);

    public virtual MockRepository BuildFinal(MockRepository initial) =>
        initial;
}
