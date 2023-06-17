# SimpleDI

One more runtime based DI container.

This project was written with the purpose of studying the internal structure and mechanics of DI containers.
There are many things that can be added and improved, but since the project was written for educational purposes,
significant further development is not planned.

You can find a list of popular DI containers here -
[C# Dependency Injection](https://github.com/tomskich/csharp-dependency-injection).

## Project Structure

The following components have been implemented:

- `IContainerBuilder`: Performs service registration and container building
- `IContainer`:
  - Resolves services directly (if they were registered with a `ServiceLifetime` other than `Scoped`)
  - `Singleton` services are resolved and stored in the root `IServiceScope` located in `Container.RootScope`
    throughout the container instance's lifetime
  - Creates new `IServiceScope` instances by calling the `CreateScope()` method
- `IServiceScope`:
  - Resolves services
  - Manages the lifecycle of `Scoped` services that were resolved within the current `scope` instance
- `IActivator`: Responsible for creating a service activation function
  - `ReflectionActivator`: Creates an instance using reflection
  - `ExpressionActivator`: Creates an instance using System.Linq.Expressions
- `ServiceLifetime`

## TODO

Some ideas for improvement:

- Add documentation
  - Documenting comments
  - More info in README
- Add more tests
- Add benchmarks
- Add activator based on [FastExpressionCompiler](https://github.com/dadhi/FastExpressionCompiler)
- Handle potential exceptions
- Throw exceptions:
  - When attempting to resolve an unregistered service
  - When the lifetime of a dependency is shorter than the lifetime of the service
- Add registration extensions
  - Factory based
  - Instance based
  - Generic types
- Think more...
