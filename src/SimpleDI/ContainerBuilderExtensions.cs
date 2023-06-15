using SimpleDI.Descriptors;

namespace SimpleDI;

public static class ContainerBuilderExtensions
{
    #region Type based registration

    public static IContainerBuilder Add<TService>(
        this IContainerBuilder builder,
        ServiceLifetime lifetime)
    {
        var type = typeof(TService);
        return Add(builder, type, type, lifetime);
    }

    public static IContainerBuilder Add<TService, TImplementation>(
        this IContainerBuilder builder,
        ServiceLifetime lifetime)
        => Add(builder, typeof(TService), typeof(TImplementation), lifetime);

    public static IContainerBuilder AddTransient<TService, TImplementation>(this IContainerBuilder builder)
        => AddTransient(builder, typeof(TService), typeof(TImplementation));

    public static IContainerBuilder AddScoped<TService, TImplementation>(this IContainerBuilder builder)
        => AddScoped(builder, typeof(TService), typeof(TImplementation));

    public static IContainerBuilder AddSingleton<TService, TImplementation>(this IContainerBuilder builder)
        => AddSingleton(builder, typeof(TService), typeof(TImplementation));

    public static IContainerBuilder AddTransient(this IContainerBuilder builder, Type service, Type implementation)
        => builder.Add(service, implementation, ServiceLifetime.Transient);

    public static IContainerBuilder AddScoped(this IContainerBuilder builder, Type service, Type implementation)
        => builder.Add(service, implementation, ServiceLifetime.Scoped);

    public static IContainerBuilder AddSingleton(this IContainerBuilder builder, Type service, Type implementation)
        => builder.Add(service, implementation, ServiceLifetime.Singleton);

    private static IContainerBuilder Add(
        this IContainerBuilder builder,
        Type service,
        Type implementation,
        ServiceLifetime lifetime)
    {
        var descriptor = new ServiceDescriptor
        {
            ServiceType = service,
            ImplementationType = implementation,
            Lifetime = lifetime
        };
        builder.Add(descriptor);
        return builder;
    }

    #endregion

    #region Factory based registration

    // TODO: Add factory based registration extensions

    private static IContainerBuilder Add(this IContainerBuilder builder, Type service,
        Func<IServiceScope, object> factory, ServiceLifetime lifetime)
    {
        var descriptor = new ServiceDescriptor
        {
            ServiceType = service,
            Factory = factory,
            Lifetime = lifetime
        };
        builder.Add(descriptor);
        return builder;
    }

    #endregion

    #region Instance based registration

    // TODO: Add instance based registration extensions

    private static IContainerBuilder Add(this IContainerBuilder builder, Type service, object instance)
    {
        var descriptor = new ServiceDescriptor
        {
            ServiceType = service,
            Instance = instance,
            Lifetime = ServiceLifetime.Singleton
        };
        builder.Add(descriptor);
        return builder;
    }

    #endregion
}