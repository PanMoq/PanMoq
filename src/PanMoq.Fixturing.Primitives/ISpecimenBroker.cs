namespace PanMoq.Fixturing.Primitives;

/// <summary>
/// Represents a fixture that can create specimens to use for testing and then execute delegates with them.
/// </summary>
public interface ISpecimenBroker
{
    /// <summary>
    /// Executes the provided <<paramref name="specimenAction"/> by providing it with an instance of <typeparamref name="TSpecimen"/>.
    /// </summary>
    /// <typeparam name="TSpecimen">The type of specimen to create and pass to the provided <paramref name="specimenAction"/>.</typeparam>
    /// <param name="specimenAction">The action to execute.</param>
    void UseSpecimen<TSpecimen>(Action<TSpecimen> specimenAction)
        where TSpecimen : notnull;

    /// <summary>
    /// Executes the provided <<paramref name="specimenAction"/> by providing it with an instance of <typeparamref name="TSpecimen"/>.
    /// </summary>
    /// <typeparam name="TSpecimen">The type of specimen to create and pass to the provided <paramref name="specimenAction"/>.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the <paramref name="specimenAction"/>.</typeparam>
    /// <param name="specimenAction">The action to execute.</param>
    /// <returns>An instance of <typeparamref name="TResult"/> that was returned by <paramref name="specimenAction"/>.</returns>
    TResult UseSpecimen<TSpecimen, TResult>(Func<TSpecimen, TResult> specimenAction)
        where TSpecimen : notnull;
}
