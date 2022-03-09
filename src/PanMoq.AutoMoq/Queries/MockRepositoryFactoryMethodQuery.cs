using System.Reflection;
using AutoFixture.Kernel;
using Moq;
using PanMoq.AutoMoq.Models;

#nullable enable

namespace PanMoq.AutoMoq.Queries;

public class MockRepositoryFactoryMethodQuery : MockQuery
{
    private readonly MockRepository _mockRepository;

    internal MockRepositoryFactoryMethodQuery(ConstructorPrioritization constructorPrioritization, MockRepository mockRepository)
        : base(constructorPrioritization)
    {
        _mockRepository = mockRepository;
    }

    protected override IMethod CreateMockForConstructorlessAbstraction(Type mockType)
    {
        if (mockType == null) throw new ArgumentNullException(nameof(mockType));

        return new MockRepositoryFactoryMethod(mockType, _mockRepository);
    }

    protected override IMethod CreateMockViaNonDefaultConstructor(Type mockType, ConstructorInfo mockedTypeConstructor)
    {
        if (mockType == null) throw new ArgumentNullException(nameof(mockType));
        if (mockedTypeConstructor == null) throw new ArgumentNullException(nameof(mockedTypeConstructor));

        return new MockRepositoryFactoryMethod(mockType, _mockRepository, mockedTypeConstructor);
    }
}
