using AutoFixture;
using PanMoq.Fixturing.Primitives;

namespace PanMoq.Fixturing.AutoFixture;

public record AutoFixture : IFixtureComponent<Fixture, Fixture>
{
    public virtual Fixture CreateInitial() =>
        new();

    public virtual Fixture BuildFinal(Fixture initial) =>
        initial;
}
