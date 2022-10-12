namespace PanMoq.Fixturing.Primitives;

/// <summary>
/// Represents a fixture that can create specimens to use for testing.
/// </summary>
public interface ISpecimenProvider
{
    /// <summary>
    /// Creates a specimen for use with testing.
    /// </summary>
    /// <typeparam name="TSpecimen">The type of specimen to create.</typeparam>
    /// <returns>An instance of <typeparamref name="TSpecimen"/>.</returns>
    TSpecimen CreateSpecimen<TSpecimen>()
        where TSpecimen : notnull;
}
