namespace PanMoq.Fixturing.Primitives;

public interface ISpecimenProvider
{
    TSpecimen CreateSpecimen<TSpecimen>()
        where TSpecimen : notnull;
}
