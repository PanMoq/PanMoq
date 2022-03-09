using Moq;

#nullable enable

namespace PanMoq.AutoMoq.Models;

internal class MockOriginatedSpecimenContext
    : AutoFixture.Kernel.SpecimenContext
{
    public MockOriginatedSpecimenContext(AutoFixture.Fixture fixture, Mock mock)
        : base(fixture ?? throw new ArgumentNullException(nameof(fixture)))
    {
        Mock = mock ?? throw new ArgumentNullException(nameof(mock));
    }

    public Mock Mock { get; }
}
