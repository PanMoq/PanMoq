using System.Linq.Expressions;
using System.Reflection;
using AutoFixture.Kernel;
using Moq;
using PanMoq.AutoMoq.Extensions;

#nullable enable

namespace PanMoq.AutoMoq.Commands;

/// <summary>
/// Sets up a mocked object's methods so that the return values will be retrieved from a fixture,
/// instead of being created directly by Moq.
///
/// This will setup any virtual methods that are either non-void or have "out" parameters.
/// </summary>
/// <remarks>
/// This will setup any virtual methods that are either non-void or have "out" parameters.
/// This includes:
///  - interface's methods/property getters;
///  - class's abstract/virtual/overridden/non-sealed methods/property getters.
///
/// Notes:
/// - Due to a limitation in Moq, methods with "ref" parameters are skipped.
/// - Automatic mocking of generic methods isn't feasible either - we'd have to anticipate any type parameters that this method could be called with.
/// - To not interfere with other post-processors in the chain we skip getters for properties that have both getters and setters.
/// </remarks>
public class MockVirtualMethodsCommand : ISpecimenCommand
{
    private static readonly DelegateSpecification DelegateSpecification = new();

    internal MockVirtualMethodsCommand()
    {
    }

    /// <summary>
    /// Sets up a mocked object's methods so that the return values will be retrieved from a fixture,
    /// instead of being created directly by Moq.
    /// </summary>
    /// <param name="specimen">The mock to setup.</param>
    /// <param name="context">The context of the mock.</param>
    public void Execute(object specimen, ISpecimenContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (specimen is not Mock mock)
            return;

        var mockType = mock.GetType();
        var mockedType = mockType.GetMockedType();
        var methods = GetConfigurableMethods(mockedType);

        foreach (var method in methods)
        {
            var returnType = method.ReturnType;
            var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method, context);

            if (methodInvocationLambda != null)
            {
                if (method.IsVoid())
                {
                    GetType()
                        .GetMethod(nameof(SetupVoidMethod), BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(mockedType)
                        .Invoke(this, new object[] { mock, methodInvocationLambda });
                }
                else
                {
                    GetType()
                        .GetMethod(nameof(SetupMethod)!, BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(mockedType, returnType)
                        .Invoke(this, new object[] { mock, methodInvocationLambda, context });
                }
            }
        }
    }

    /// <summary>
    /// Sets up a void method.
    /// </summary>
    /// <typeparam name="TMock">The type of the object being mocked.</typeparam>
    /// <param name="mock">The mock being set up.</param>
    /// <param name="methodCallExpression">An expression representing a call to the method being set up.</param>
    private static void SetupVoidMethod<TMock>(Mock<TMock> mock, Expression<Action<TMock>> methodCallExpression)
        where TMock : class
    {
        if (mock == null) throw new ArgumentNullException(nameof(mock));
        if (methodCallExpression == null) throw new ArgumentNullException(nameof(methodCallExpression));

        mock.Setup(methodCallExpression);
    }

    /// <summary>
    /// Sets up a non-void method.
    /// </summary>
    /// <typeparam name="TMock">The type of the object being mocked.</typeparam>
    /// <typeparam name="TResult">The return type of the method being set up.</typeparam>
    /// <param name="mock">The mock being set up.</param>
    /// <param name="methodCallExpression">An expression representing a call to the method being set up.</param>
    /// <param name="context">The context that will be used to resolve the method's return value.</param>
    private static void SetupMethod<TMock, TResult>(Mock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression, ISpecimenContext context)
        where TMock : class
    {
        if (mock == null) throw new ArgumentNullException(nameof(mock));
        if (methodCallExpression == null) throw new ArgumentNullException(nameof(methodCallExpression));
        if (context == null) throw new ArgumentNullException(nameof(context));

        mock.Setup(methodCallExpression)
            .ReturnsUsingContext(context);
    }

    /// <summary>
    /// Gets a list of methods to configure.
    /// </summary>
    /// <param name="type">The type being mocked and whose methods need to be configured.</param>
    private static IEnumerable<MethodInfo> GetConfigurableMethods(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        // If "type" is a delegate, return "Invoke" method only and skip the rest of the methods.
        var methods = DelegateSpecification.IsSatisfiedBy(type)
            ? new[] { type.GetTypeInfo().GetMethod("Invoke")! }
            : type.GetAllMethods();

        // Skip properties that have both getters and setters to not interfere
        // with other post-processors in the chain that initialize them later.
        methods = SkipWritablePropertyGetters(type, methods);

        return methods.Where(CanBeConfigured);
    }

    private static IEnumerable<MethodInfo> SkipWritablePropertyGetters(Type type, IEnumerable<MethodInfo> methods)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (methods == null) throw new ArgumentNullException(nameof(methods));

        var getterMethods = type.GetAllProperties()
            .Where(p => p.GetGetMethod() != null &&
                        p.GetSetMethod() != null)
            .Select(p => p.GetGetMethod()!);

        return methods.Except(getterMethods);
    }

    /// <summary>
    /// Determines whether a method can be mocked.
    /// </summary>
    /// <param name="method">The candidate method.</param>
    /// <returns>Whether <paramref name="method"/> can be configured.</returns>
    private static bool CanBeConfigured(MethodInfo method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        return method.IsOverridable() &&
               !method.IsGenericMethod &&
               !method.HasRefParameters() &&
               (!method.IsVoid() || method.HasOutParameters());
    }

    /// <summary>
    /// Returns a lambda expression which represents an invocation of a mocked type's method.
    /// E.g.,. <![CDATA[ x => x.Method(It.IsAny<string>(), out parameter) ]]>
    /// </summary>
    private static Expression? MakeMethodInvocationLambda(Type mockedType, MethodInfo method, ISpecimenContext context)
    {
        if (mockedType == null) throw new ArgumentNullException(nameof(mockedType));
        if (method == null) throw new ArgumentNullException(nameof(method));
        if (context == null) throw new ArgumentNullException(nameof(context));

        var lambdaParam = Expression.Parameter(mockedType, "x");

        var methodCallParams = method.GetParameters()
                                     .Select(param => MakeParameterExpression(param, context))
                                     .ToList();

        if (methodCallParams.Any(exp => exp == null))
            return null;

        Expression methodCall;
        if (DelegateSpecification.IsSatisfiedBy(mockedType))
        {
            // e.g. "x(It.IsAny<string>(), out parameter)"
            methodCall = Expression.Invoke(lambdaParam, methodCallParams!);
        }
        else
        {
            // e.g. "x.Method(It.IsAny<string>(), out parameter)"
            methodCall = Expression.Call(lambdaParam, method, methodCallParams!);
        }

        // e.g. "x => x.Method(It.IsAny<string>(), out parameter)"
        // or "x => x(It.IsAny<string>(), out parameter)"
        return Expression.Lambda(methodCall, lambdaParam);
    }

    private static Expression? MakeParameterExpression(ParameterInfo parameter, ISpecimenContext context)
    {
        if (parameter == null) throw new ArgumentNullException(nameof(parameter));
        if (context == null) throw new ArgumentNullException(nameof(context));

        // check if parameter is an "out" parameter
        if (parameter.IsOut)
        {
            // gets the type corresponding to this "byref" type
            // e.g., the underlying type of "System.String&" is "System.String"
            var underlyingType = parameter.ParameterType.GetElementType()!;

            // resolve the "out" param from the context
            var variable = context.Resolve(underlyingType);

            return
                variable is OmitSpecimen
                    ? null
                    : Expression.Constant(variable, underlyingType);
        }
        else
        {
            // for any non-out parameter, invoke "It.IsAny<T>()"
            var isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny))!.MakeGenericMethod(parameter.ParameterType);

            return Expression.Call(isAnyMethod);
        }
    }
}
