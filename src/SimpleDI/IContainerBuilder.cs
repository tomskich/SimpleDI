using SimpleDI.Descriptors;

namespace SimpleDI;

public interface IContainerBuilder
{
    IContainerBuilder Add(ServiceDescriptor service);
    IContainer Build();
}