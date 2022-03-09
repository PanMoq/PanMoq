using AutoFixture;
using Moq;

namespace PanMoq.AutoMoq.Extensions;

public static class CrosswiringExtensions
{
    public static void GetDefaultReturnValuesFrom(this MockRepository mockRepository, Fixture autofixtureFixture) =>
        (mockRepository ?? throw new ArgumentNullException(nameof(mockRepository))).DefaultValueProvider = new AutoFixtureMoqDefaultValueProvider(autofixtureFixture ?? throw new ArgumentNullException(nameof(autofixtureFixture)));

    public static void CustomizeWithAutomaticMocksFrom(this Fixture autofixtureFixture, MockRepository mockRepository)
    {
        if (autofixtureFixture == null) throw new ArgumentNullException(nameof(autofixtureFixture));
        if (mockRepository == null) throw new ArgumentNullException(nameof(mockRepository));

        autofixtureFixture.CustomizeWithAutomaticMocks(new AutoMoqCustomization(mockRepository) { ConfigureMembers = true, GenerateDelegates = true });
    }

    public static void CustomizeWithAutomaticMocks(this Fixture autofixtureFixture)
    {
        if (autofixtureFixture == null) throw new ArgumentNullException(nameof(autofixtureFixture));

        autofixtureFixture.CustomizeWithAutomaticMocks(new AutoMoqCustomization() { ConfigureMembers = true, GenerateDelegates = true });
    }

    public static void CustomizeWithAutomaticMocks(this Fixture autofixtureFixture, AutoMoqCustomization customization)
    {
        if (autofixtureFixture == null) throw new ArgumentNullException(nameof(autofixtureFixture));
        if (customization == null) throw new ArgumentNullException(nameof(customization));

        autofixtureFixture.Customize(customization);
    }
}
