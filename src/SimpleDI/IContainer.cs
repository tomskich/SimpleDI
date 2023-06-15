namespace SimpleDI;

public interface IContainer : IServiceProvider, IServiceScopeFactory, IDisposable, IAsyncDisposable
{
}