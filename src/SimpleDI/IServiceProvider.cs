namespace SimpleDI;

public interface IServiceProvider
{
    object? GetService(Type service);
}