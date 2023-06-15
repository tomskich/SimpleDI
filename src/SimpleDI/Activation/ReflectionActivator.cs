namespace SimpleDI.Activation;

public class ReflectionActivator : IActivator
{
    private readonly IConstructorFinder _constructorFinder;

    public ReflectionActivator(IConstructorFinder constructorFinder)
    {
        _constructorFinder = constructorFinder;
    }

    public Func<IServiceScope, object?> CreateActivation(Type service)
    {
        var ctor = _constructorFinder.Find(service);

        return ctor == null
            ? _ => null
            : scope =>
            {
                var parameters = ctor.GetParameters();
                var args = new object?[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                    args[i] = scope.GetService(parameters[i].ParameterType);

                return ctor.Invoke(args);
            };
    }
}