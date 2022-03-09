namespace PanMoq.Fixturing.Primitives;

public interface ISpecimenBroker
{
    void UseSpecimen<TSpecimen>(Action<TSpecimen> specimenAction)
        where TSpecimen : notnull;

    TSpecimen UseSpecimen<TSpecimen, TResult>(Func<TSpecimen, TResult> specimenAction)
        where TSpecimen : notnull;
}
