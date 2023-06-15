namespace SimpleDI.Descriptors;

public class ServiceDescriptor
{
    public required ServiceLifetime Lifetime { get; init; }
    public required Type ServiceType { get; init; }
    public Type? ImplementationType { get; init; }
    public object? Instance { get; init; }
    public Func<IServiceScope, object>? Factory { get; init; }
    public ServiceDescriptor[]? Descriptors { get; init; }

    public ServiceInitializationType Type => this switch
    {
        {Factory: not null} => ServiceInitializationType.Factory,
        {Instance: not null} => ServiceInitializationType.Instance,
        _ => ServiceInitializationType.ImplementationType
    };
}