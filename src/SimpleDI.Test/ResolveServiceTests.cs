using SimpleDI.Test.Services;

namespace SimpleDI.Test;

public class ResolveServiceTests
{
    [Fact]
    public void WhenServiceNotRegistered_ReturnsNull()
    {
        var container = new ContainerBuilder().Build();

        var userService = container.GetService(typeof(IUserService));

        Assert.Null(userService);
    }

    [Fact]
    public void WhenDependencyNotRegistered_ThrowsException()
    {
        var container = new ContainerBuilder()
            .AddSingleton<IUserService, UserService>()
            .Build();

        object? Resolve() => container.GetService(typeof(IUserService));

        Assert.Throws<Exception>(Resolve);
    }

    [Fact]
    public void Singleton_WithScopedDependency_ThrowsException()
    {
        // TODO: Singleton_WithTransientDependency_ThrowsException ?

        var container = new ContainerBuilder()
            .AddSingleton<IUserService, UserService>()
            .AddScoped<IUserRepository, UserRepository>()
            .Build();

        object? Resolve() => container.GetService(typeof(IUserService));

        Assert.Throws<Exception>(Resolve);
    }

    [Fact]
    public void Scopes_AreNotNull()
    {
        using var container = new ContainerBuilder().Build();
        using var scope1 = container.CreateScope();
        using var scope2 = container.CreateScope();

        Assert.NotNull(scope1);
        Assert.NotNull(scope2);
    }

    [Fact]
    public void Scopes_AreNotEqual()
    {
        using var container = new ContainerBuilder().Build();
        using var scope1 = container.CreateScope();
        using var scope2 = container.CreateScope();

        Assert.NotEqual(scope1, scope2);
    }

    [Fact]
    public void TransientInstances_FromContainer_AreNotNull()
    {
        var container = new ContainerBuilder()
            .AddTransient<IUserService, UserService>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .Build();

        var scope = container.CreateScope();

        var userService = scope.GetService(typeof(IUserService));
        var userRepository = scope.GetService(typeof(IUserRepository));
        var unitOfWork = scope.GetService(typeof(IUnitOfWork));

        Assert.NotNull(userService);
        Assert.NotNull(userRepository);
        Assert.NotNull(unitOfWork);
    }

    [Fact]
    public void TransientInstances_FromScope_AreNotNull()
    {
        var container = new ContainerBuilder()
            .AddTransient<IUserService, UserService>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .Build();

        var scope = container.CreateScope();

        var userService = scope.GetService(typeof(IUserService));
        var userRepository = scope.GetService(typeof(IUserRepository));
        var unitOfWork = scope.GetService(typeof(IUnitOfWork));

        Assert.NotNull(userService);
        Assert.NotNull(userRepository);
        Assert.NotNull(unitOfWork);
    }

    [Fact]
    public void TransientInstances_FromOneScope_AreNotEqual()
    {
        using var container = new ContainerBuilder()
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .Build();

        var instance1 = container.GetService(typeof(IUnitOfWork));
        var instance2 = container.GetService(typeof(IUnitOfWork));

        Assert.NotEqual(instance1, instance2);
    }

    [Fact]
    public void TransientInstances_FromDifferentScopes_AreNotEqual()
    {
        using var container = new ContainerBuilder()
            .AddTransient<IUnitOfWork, UnitOfWork>()
            .Build();

        using var scope1 = container.CreateScope();
        using var scope2 = container.CreateScope();

        var instance1 = scope1.GetService(typeof(IUnitOfWork));
        var instance2 = scope2.GetService(typeof(IUnitOfWork));

        Assert.NotEqual(instance1, instance2);
    }

    [Fact]
    public void SingletonInstances_FromContainer_AreNotNull()
    {
        var container = new ContainerBuilder()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IUnitOfWork, UnitOfWork>()
            .Build();

        var userService = container.GetService(typeof(IUserService));
        var userRepository = container.GetService(typeof(IUserRepository));
        var unitOfWork = container.GetService(typeof(IUnitOfWork));

        Assert.NotNull(userService);
        Assert.NotNull(userRepository);
        Assert.NotNull(unitOfWork);
    }

    [Fact]
    public void SingletonInstances_FromScope_AreNotNull()
    {
        var container = new ContainerBuilder()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<IUnitOfWork, UnitOfWork>()
            .Build();

        var scope = container.CreateScope();

        var userService = scope.GetService(typeof(IUserService));
        var userRepository = scope.GetService(typeof(IUserRepository));
        var unitOfWork = scope.GetService(typeof(IUnitOfWork));

        Assert.NotNull(userService);
        Assert.NotNull(userRepository);
        Assert.NotNull(unitOfWork);
    }

    [Fact]
    public void SingletonInstances_FromOneScope_AreEqual()
    {
        using var container = new ContainerBuilder()
            .AddSingleton<IUnitOfWork, UnitOfWork>()
            .Build();

        var instance1 = container.GetService(typeof(IUnitOfWork));
        var instance2 = container.GetService(typeof(IUnitOfWork));

        Assert.Equal(instance1, instance2);
    }

    [Fact]
    public void SingletonInstances_FromDifferentScopes_AreEqual()
    {
        using var container = new ContainerBuilder()
            .AddSingleton<IUnitOfWork, UnitOfWork>()
            .Build();

        using var scope1 = container.CreateScope();
        using var scope2 = container.CreateScope();

        var instance1 = scope1.GetService(typeof(IUnitOfWork));
        var instance2 = scope2.GetService(typeof(IUnitOfWork));

        Assert.Equal(instance1, instance2);
    }

    [Fact]
    public void ScopedInstances_FromScope_AreNotNull()
    {
        var container = new ContainerBuilder()
            .AddScoped<IUserService, UserService>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .Build();

        var scope = container.CreateScope();

        var userService = scope.GetService(typeof(IUserService));
        var userRepository = scope.GetService(typeof(IUserRepository));
        var unitOfWork = scope.GetService(typeof(IUnitOfWork));

        Assert.NotNull(userService);
        Assert.NotNull(userRepository);
        Assert.NotNull(unitOfWork);
    }

    [Fact]
    public void ScopedInstance_FromContainer_ThrowsException()
    {
        using var container = new ContainerBuilder()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .Build();

        object? Resolve() => container.GetService(typeof(IUnitOfWork));

        Assert.Throws<Exception>(Resolve);
    }

    [Fact]
    public void ScopedInstances_FromOneScope_AreEqual()
    {
        using var container = new ContainerBuilder()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .Build();

        using var scope1 = container.CreateScope();

        var instance1 = scope1.GetService(typeof(IUnitOfWork));
        var instance2 = scope1.GetService(typeof(IUnitOfWork));

        Assert.Equal(instance1, instance2);
    }

    [Fact]
    public void ScopedInstances_FromDifferentScopes_AreNotEqual()
    {
        using var container = new ContainerBuilder()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .Build();

        using var scope1 = container.CreateScope();
        using var scope2 = container.CreateScope();

        var instance1 = scope1.GetService(typeof(IUnitOfWork));
        var instance2 = scope2.GetService(typeof(IUnitOfWork));

        Assert.NotEqual(instance1, instance2);
    }
}