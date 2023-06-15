using System.Collections;
using System.Collections.Concurrent;

namespace SimpleDI.Descriptors;

internal class ServiceDescriptorCollection
{
    private readonly ConcurrentDictionary<Type, ServiceDescriptor> _descriptors = new();

    public ServiceDescriptorCollection(IEnumerable<ServiceDescriptor> descriptors)
    {
        Populate(descriptors);
    }

    private void Populate(IEnumerable<ServiceDescriptor> descriptors)
    {
        var descriptorsArray = descriptors.ToArray();

        foreach (var descriptor in descriptorsArray)
        {
            // TODO: Aggregate exceptions
            Validate(descriptor);
        }

        var internalDescriptors = _descriptors as IDictionary<Type, ServiceDescriptor>;
        var groupedByType = descriptorsArray.GroupBy(x => x.ServiceType);

        foreach (var typeDescriptors in groupedByType)
        {
            var type = typeDescriptors.Key;
            var items = typeDescriptors.ToArray();

            if (items.Length == 1)
            {
                internalDescriptors.Add(type, items[0]);
            }
            else
            {
                var collectiveDescriptor = new ServiceDescriptor
                {
                    Descriptors = items,
                    Lifetime = ServiceLifetime.Singleton,
                    ServiceType = type
                };

                internalDescriptors.Add(type, collectiveDescriptor);
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(type);
                internalDescriptors.Add(enumerableType, CreateCollectiveDescriptor(enumerableType, collectiveDescriptor));
            }
        }
    }

    public ServiceDescriptor? GetOrCreate(Type service)
    {
        if (_descriptors.TryGetValue(service, out var descriptor))
            return descriptor;

        if (service.IsAssignableTo(typeof(IEnumerable)) &&
            service.IsGenericType &&
            service.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var serviceDescriptor = GetOrCreate(service.GetGenericArguments().Single())!;
            return _descriptors.GetOrAdd(service, CreateCollectiveDescriptor(service, serviceDescriptor));
        }

        if (service.IsConstructedGenericType)
        {
            var genericType = service.GetGenericTypeDefinition();
            var genericDescriptor = GetOrCreate(genericType);

            if (genericDescriptor?.Type is ServiceInitializationType.ImplementationType)
            {
                var implementation = genericDescriptor.ImplementationType!
                    .MakeGenericType(service.GetGenericArguments());

                var serviceDescriptor = new ServiceDescriptor
                {
                    ServiceType = service,
                    ImplementationType = implementation,
                    Lifetime = genericDescriptor.Lifetime
                };

                return _descriptors.GetOrAdd(implementation, serviceDescriptor);
            }
        }

        return null;
    }

    private void Validate(ServiceDescriptor descriptor)
    {
        // TODO: Add other validations

        if (descriptor.Type is ServiceInitializationType.ImplementationType &&
            (descriptor.ImplementationType!.IsAbstract ||
             descriptor.ImplementationType.IsInterface))
        {
            throw new ArgumentException(
                $"Cannot instantiate implementation type '{descriptor.ImplementationType}' for service type '{descriptor.ServiceType}'.");
        }
    }

    private ServiceDescriptor CreateCollectiveDescriptor(Type enumerableGenericType, ServiceDescriptor descriptor) =>
        new()
        {
            ServiceType = enumerableGenericType,
            Factory = scope =>
            {
                var descriptors = descriptor.Descriptors ?? new[] {descriptor};
                var arr = Array.CreateInstance(descriptor.ServiceType, descriptors.Length);
                var typedScope = (ServiceScope) scope;
                
                for (var i = 0; i < arr.Length; i++)
                    arr.SetValue(typedScope.Resolve(descriptors[i]), i);
                
                return arr;
            },
            Lifetime = ServiceLifetime.Transient
        };
}