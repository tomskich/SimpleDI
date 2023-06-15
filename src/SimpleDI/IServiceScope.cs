namespace SimpleDI;

public interface IServiceScope : IServiceProvider, IDisposable, IAsyncDisposable
{
    IContainer Container { get; }
}