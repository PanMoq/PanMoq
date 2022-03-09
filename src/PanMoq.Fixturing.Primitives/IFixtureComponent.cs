namespace PanMoq.Fixturing.Primitives;

public interface IFixtureComponent<out TInitial>
{
    TInitial CreateInitial();
}

public interface IFixtureComponent<TInitial, out TFinal>
    : IFixtureComponent<TInitial>
{
    TFinal BuildFinal(TInitial initial);
}
