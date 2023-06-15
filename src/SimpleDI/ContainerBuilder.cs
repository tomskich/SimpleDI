using SimpleDI.Descriptors;

namespace SimpleDI;

public class ContainerBuilder : IContainerBuilder
{
    private readonly List<ServiceDescriptor> _descriptors = new();

    public IContainerBuilder Add(ServiceDescriptor service)
    {
        _descriptors.Add(service);
        return this;
    }

    public IContainer Build()
    {
        return new Container(_descriptors);
    }
}