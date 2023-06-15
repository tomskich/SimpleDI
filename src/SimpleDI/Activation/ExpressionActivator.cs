using System.Linq.Expressions;
using System.Reflection;

namespace SimpleDI.Activation;

public class ExpressionActivator : IActivator
{
    private const string ScopeParameterName = "scope";
    private const string GetServiceMethodName = nameof(IServiceProvider.GetService);

    private static readonly ParameterExpression ScopeParameter = Expression.Parameter(typeof(IServiceScope), ScopeParameterName);
    private static readonly MethodInfo GetServiceMethod = typeof(IServiceProvider).GetMethod(GetServiceMethodName)!;

    private readonly IConstructorFinder _constructorFinder;

    public ExpressionActivator(IConstructorFinder constructorFinder)
    {
        _constructorFinder = constructorFinder;
    }

    public Func<IServiceScope, object?> CreateActivation(Type service)
    {
        var constructor = _constructorFinder.Find(service);

        var parameters = constructor.GetParameters();

        var args = parameters.Select(parameter => Expression.Convert(
            Expression.Call(ScopeParameter, GetServiceMethod,
                Expression.Constant(parameter.ParameterType)),
            parameter.ParameterType));

        return Expression
            .Lambda<Func<IServiceScope, object>>(Expression
                .New(constructor, args), ScopeParameter)
            .Compile();
    }
}