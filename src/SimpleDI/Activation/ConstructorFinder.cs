using System.Reflection;

namespace SimpleDI.Activation;

public class ConstructorFinder : IConstructorFinder
{
    private readonly Container _container;

    public ConstructorFinder(Container container)
    {
        _container = container;
    }

    public ConstructorInfo Find(Type type)
    {
        var constructors = GetConstructors(type).ToList();
        
        if (constructors.Count == 0)
            throw new Exception($"Instance, Public constructors of type '{type}' not found");
        
        return constructors.FirstOrDefault(AllDependenciesRegistered) ??
               throw new Exception($"Registered dependencies required for constructors of type '{type}' not found");
    }

    private IEnumerable<ConstructorInfo> GetConstructors(Type type) => type
        .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
        .OrderByDescending(x => x.GetParameters().Length);

    private bool AllDependenciesRegistered(ConstructorInfo constructor) => constructor
        .GetParameters()
        .All(parameter => _container.FindDescriptor(parameter.ParameterType) != null);
}