using SimpleDI.Activation;

namespace SimpleDI;

public class ContainerOptions
{
    public IActivatorProvider? ActivatorProvider { get; set; }
}