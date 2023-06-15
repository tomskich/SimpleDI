namespace SimpleDI.Activation;

public interface IActivator
{
    Func<IServiceScope, object?> CreateActivation(Type service);
}