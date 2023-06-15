using System.Reflection;

namespace SimpleDI.Activation;

public interface IConstructorFinder
{
    ConstructorInfo Find(Type type);
}