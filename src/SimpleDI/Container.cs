using System.Collections.Concurrent;
using SimpleDI.Activation;
using SimpleDI.Descriptors;

namespace SimpleDI;

public class Container : IContainer
{
    private readonly ServiceDescriptorCollection _descriptors;
    private readonly ConcurrentDictionary<ServiceDescriptor, Func<IServiceScope, object?>> _activators = new();
    private readonly IActivator _activator;

    public Container(IEnumerable<ServiceDescriptor> serviceDescriptors, ContainerOptions? options = default)
    {
        _descriptors = new ServiceDescriptorCollection(serviceDescriptors);
        _activator = options?.ActivatorProvider?.GetActivator(this)
                     ?? new ExpressionActivator(new ConstructorFinder(this));
        RootScope = new ServiceScope(this, true);
    }

    internal ServiceScope RootScope { get; }

    public object? GetService(Type service) => RootScope.GetService(service);
    public IServiceScope CreateScope() => new ServiceScope(this, false);
    internal ServiceDescriptor? FindDescriptor(Type service) => _descriptors.GetOrCreate(service);

    internal object? CreateInstance(IServiceScope scope, ServiceDescriptor descriptor) =>
        _activators.GetOrAdd(descriptor, _ => GetActivation(descriptor)).Invoke(scope);

    private Func<IServiceScope, object?> GetActivation(ServiceDescriptor serviceDescriptor) =>
        serviceDescriptor.Type switch
        {
            ServiceInitializationType.Factory => serviceDescriptor.Factory!,
            ServiceInitializationType.Instance => _ => serviceDescriptor.Instance,
            _ => _activator.CreateActivation(serviceDescriptor.ImplementationType!)
        };

    public void Dispose() => RootScope.Dispose();

    public ValueTask DisposeAsync() => RootScope.DisposeAsync();
}