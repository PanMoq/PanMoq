using System.Reflection;
using AutoFixture.Kernel;
using Moq;

#nullable enable

namespace PanMoq.AutoMoq.Models;

public class MockRepositoryFactoryMethod : IMethod
{
    private readonly MethodInfo _method;
    private readonly MockRepository _mockRepository;
    private readonly ConstructorInfo? _mockedTypeConstructor;

    private static readonly MethodInfo FactoryMethodGenericDefinition =
        typeof(MockRepository)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(mi => string.Equals(mi.Name, nameof(MockRepository.Create), StringComparison.Ordinal))
            .Where(mi => mi.GetParameters().Length == 1)
            .Single(mi => typeof(object[]).IsAssignableTo(mi.GetParameters()[0].ParameterType));

    internal MockRepositoryFactoryMethod(Type mockType, MockRepository mockRepository, ConstructorInfo? mockedTypeConstructor = null)
    {
        if (mockType == null) throw new ArgumentNullException(nameof(mockType));
        var mockedType = mockType.GetMockedType();

        _method = FactoryMethodGenericDefinition.MakeGenericMethod(mockedType);
        _mockRepository = mockRepository ?? throw new ArgumentNullException(nameof(mockRepository));
        _mockedTypeConstructor = mockedTypeConstructor;
    }


    public object? Invoke(IEnumerable<object> parameters)
    {
        if (parameters is not object[])
        {
            parameters = parameters.ToArray();
        }

        return _method.Invoke(_mockRepository, new object[] { parameters });
    }

    public IEnumerable<ParameterInfo> Parameters =>
        _mockedTypeConstructor?.GetParameters() ?? Array.Empty<ParameterInfo>();
}
