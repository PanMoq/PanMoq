using System.Reflection;
using AutoFixture.Kernel;
using PanMoq.AutoMoq.Models;

#nullable enable

namespace PanMoq.AutoMoq.Queries;

public class MockConstructorQuery : MockQuery
{
    public MockConstructorQuery(ConstructorPrioritization constructorPrioritization)
        : base(constructorPrioritization)
    {
    }

    protected override IMethod CreateMockForConstructorlessAbstraction(Type mockType)
    {
        if (mockType == null) throw new ArgumentNullException(nameof(mockType));

        return new ConstructorMethod(mockType.GetDefaultConstructor());
    }

    protected override IMethod CreateMockViaNonDefaultConstructor(Type mockType, ConstructorInfo mockedTypeConstructor)
    {
        if (mockType == null) throw new ArgumentNullException(nameof(mockType));
        if (mockedTypeConstructor == null) throw new ArgumentNullException(nameof(mockedTypeConstructor));

        return new MockConstructorMethod(mockType.GetParamsConstructor()!);
    }
}
