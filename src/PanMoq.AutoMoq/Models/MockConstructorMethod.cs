using System.Reflection;
using AutoFixture.Kernel;

#nullable enable

namespace PanMoq.AutoMoq.Models;

public class MockConstructorMethod : IMethod
{
    private readonly ConstructorInfo _ctor;

    internal MockConstructorMethod(ConstructorInfo ctor)
    {
        _ctor = ctor ?? throw new ArgumentNullException(nameof(ctor));
    }

    public IEnumerable<ParameterInfo> Parameters => _ctor.GetParameters();

    public object Invoke(IEnumerable<object> parameters)
    {
        if (parameters is not object[])
        {
            parameters = parameters.ToArray();
        }

        var paramsArray = new object[] { parameters };
        return _ctor.Invoke(paramsArray);
    }
}
