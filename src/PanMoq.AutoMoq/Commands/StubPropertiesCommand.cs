using AutoFixture.Kernel;
using Moq;

#nullable enable

namespace PanMoq.AutoMoq.Commands;

/// <summary>
/// Stubs a mocked object's properties, giving them "property behavior".
/// Setting a property's value will cause it to be saved and later returned when the property is accessed.
/// </summary>
public class StubPropertiesCommand : ISpecimenCommand
{
    internal StubPropertiesCommand()
    {
    }

    /// <summary>
    /// Stubs a mocked object's properties, giving them "property behavior".
    /// Setting a property's value will cause it to be saved and later returned when the property is accessed.
    /// </summary>
    /// <param name="specimen">The mock to setup.</param>
    /// <param name="context">The context of the mock.</param>
    public void Execute(object specimen, ISpecimenContext context)
    {
        if (specimen is not Mock mock)
            return;

        // Disable generation of default values (if enabled), otherwise SetupAllProperties will hang if there's a circular dependency
        var mockDefaultValueSetting = mock.DefaultValue;
        mock.DefaultValue = DefaultValue.Empty;

        // stub properties
        mock.GetType()
            .GetMethod("SetupAllProperties")!
            .Invoke(mock, Array.Empty<object>());

        // restore setting
        mock.DefaultValue = mockDefaultValueSetting;
    }
}
