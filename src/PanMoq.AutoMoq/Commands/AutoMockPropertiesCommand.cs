using System.Reflection;
using AutoFixture.Kernel;
using Moq;

#nullable disable

namespace PanMoq.AutoMoq.Commands;

/// <summary>
/// A command that populates all public writable properties/fields of a mock object with anonymous values.
/// </summary>
public class AutoMockPropertiesCommand : ISpecimenCommand
{
    private readonly ISpecimenCommand _autoPropertiesCommand =
        new AutoPropertiesCommand(new IgnoreProxyMembersSpecification());

    internal AutoMockPropertiesCommand()
    {
    }

    /// <summary>
    /// Populates all public writable properties/fields of a mock object with anonymous values.
    /// </summary>
    /// <param name="specimen">The mock object whose properties/fields will be populated.</param>
    /// <param name="context">The context that is used to create anonymous values.</param>
    public void Execute(object specimen, ISpecimenContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (specimen is not Mock mock)
            return;

        _autoPropertiesCommand.Execute(mock.Object, context);
    }

    /// <summary>
    /// Evaluates whether a request to populate a member is valid.
    /// The request is valid if the member is a property or a field,
    /// except for fields generated by Castle's DynamicProxy.
    /// </summary>
    private class IgnoreProxyMembersSpecification : IRequestSpecification
    {
        public bool IsSatisfiedBy(object request) =>
            request switch
            {
                FieldInfo fi => !IsProxyMember(fi),
                PropertyInfo pi => !IsProxyMember(pi),
                _ => false
            };

        private static bool IsProxyMember(FieldInfo fi) =>
            string.Equals(fi.Name, "__interceptors", StringComparison.Ordinal) ||
            string.Equals(fi.Name, "__target", StringComparison.Ordinal);

        private static bool IsProxyMember(PropertyInfo pi) =>
            string.Equals(pi.Name, nameof(IMocked.Mock), StringComparison.Ordinal) ||
            string.Equals(pi.Name, "Interceptors", StringComparison.Ordinal);
    }
}
