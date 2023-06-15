using System.Collections.Concurrent;
using SimpleDI.Descriptors;

namespace SimpleDI;

internal class ServiceScope : IServiceScope, IServiceScopeFactory
{
    private readonly Container _container;
    private readonly bool _isRootScope;
    private readonly ConcurrentDictionary<ServiceDescriptor, object?> _resolvedServices = new();
    private readonly ConcurrentStack<object> _disposables = new();

    public ServiceScope(Container container, bool isRootScope)
    {
        _container = container;
        _isRootScope = isRootScope;
    }

    public IContainer Container => _container;

    public IServiceScope CreateScope() => _container.CreateScope();

    public object? GetService(Type service)
    {
        var descriptor = _container.FindDescriptor(service);
        return descriptor == null ? null : Resolve(descriptor);
    }

    internal object? Resolve(ServiceDescriptor descriptor)
    {
        if (_isRootScope && descriptor.Lifetime == ServiceLifetime.Singleton)
            return _resolvedServices.GetOrAdd(descriptor, _ => CreateInstance(descriptor));

        return descriptor.Lifetime switch
        {
            ServiceLifetime.Transient => CreateInstance(descriptor),
            ServiceLifetime.Scoped => _isRootScope
                ? throw new Exception("Scoped service can be resolved only in the scope.")
                : _resolvedServices.GetOrAdd(descriptor, _ => CreateInstance(descriptor)),
            ServiceLifetime.Singleton => _container.RootScope.Resolve(descriptor),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private object? CreateInstance(ServiceDescriptor descriptor)
    {
        var instance = _container.CreateInstance(this, descriptor);

        if (instance is IDisposable or IAsyncDisposable)
        {
            _disposables.Push(instance);
        }

        return instance;
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            if (disposable is IAsyncDisposable asyncDisposable)
            {
                asyncDisposable.DisposeAsync().GetAwaiter().GetResult();
            }
            else
            {
                (disposable as IDisposable)!.Dispose();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var disposable in _disposables)
        {
            if (disposable is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                (disposable as IDisposable)!.Dispose();
            }
        }
    }
}