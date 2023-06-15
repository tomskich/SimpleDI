namespace SimpleDI.Activation;

public interface IActivatorProvider
{
    IActivator GetActivator(IContainer container);
}