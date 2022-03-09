using AutoFixture;
using Moq;
using PanMoq.AutoMoq.Models;

namespace PanMoq.AutoMoq;

public class AutoFixtureMoqDefaultValueProvider : DefaultValueProvider
{
    private readonly Fixture _autofixtureFixture;

    internal AutoFixtureMoqDefaultValueProvider(Fixture autofixtureFixture)
    {
        _autofixtureFixture = autofixtureFixture ?? throw new ArgumentNullException(nameof(autofixtureFixture));
    }

    protected override object GetDefaultValue(Type type, Mock mock)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (mock == null) throw new ArgumentNullException(nameof(mock));

        var autofixtureSpecimenContext = new MockOriginatedSpecimenContext(_autofixtureFixture, mock);
        var autofixtureRequest = new AutoFixture.Kernel.SeededRequest(type, default);
        return autofixtureSpecimenContext.Resolve(autofixtureRequest);
    }
}
