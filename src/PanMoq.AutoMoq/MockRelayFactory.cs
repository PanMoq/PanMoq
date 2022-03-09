using System.Reflection;
using AutoFixture.Kernel;

namespace PanMoq.AutoMoq;

internal static class MockRelayFactory
{
    internal static MockRelay CreateForClassesOrInterfaces() =>
        new(new IsMockableSpecification());

    internal static MockRelay CreateForDelegates() =>
        new(new DelegateSpecification());

    private class IsMockableSpecification : IRequestSpecification
    {
        public bool IsSatisfiedBy(object request)
        {
            if (request is not Type t)
                return false;

            return t.GetTypeInfo().IsAbstract || t.GetTypeInfo().IsInterface;
        }
    }
}
