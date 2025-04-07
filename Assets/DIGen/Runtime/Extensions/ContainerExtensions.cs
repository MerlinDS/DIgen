using System;
using System.Runtime.CompilerServices;
using DIGen.Runtime.Base;
using DIGen.Runtime.Bindings;
using DIGen.Runtime.Exceptions;
using DIGen.Runtime.Resolvers;

namespace DIGen.Runtime.Extensions
{
    public static class ContainerExtensions
    {
        public static bool IsRegistered<T>(this IContainer container)
        {
            throw new NotImplementedException();
        }

        public static T Resolve<T>(this IContainer container)
        {
            if (container is not IDependencyContainer dependencyContainer)
            {
                throw new DIGenException($"Container {container.GetType()} has invalid type.");
            }

            return dependencyContainer.Resolve<T>();
        }

        private static T Resolve<T>(this IDependencyContainer container)
        {
            BindingDescriptor descriptor = default;
            IDependencyContainer scope = container;

            while (container is not null)
            {
                if (container.TryGetDescriptor<T>(out descriptor))
                {
                    break;
                }

                container = container.Parent;
            }

            if (container is null)
            {
                throw new DIGenException($"Service of type {typeof(T)} was not registered.");
            }

            switch (descriptor.Lifetime)
            {
                case Lifetime.Transient:
                    // Create a new instance each time don't use scope
                    return container.Instantiate<T>(in descriptor);
                case Lifetime.Scoped:
                    bool isInstanceScoped = scope.TryGetDescriptor<T>(out BindingDescriptor scopeDescriptor) &&
                                   scopeDescriptor.InstanceIndex >= 0;
                    return !isInstanceScoped
                        ? InstantiateAndCache<T>(container, scope, descriptor)
                        : GetInstance<T>(scope, scopeDescriptor);
                case Lifetime.Singleton:
                    return descriptor.InstanceIndex < 0
                        ? InstantiateAndCache<T>(container, container, descriptor)
                        : GetInstance<T>(container, descriptor);
                default:
                    throw new DIGenException($"Invalid lifetime {descriptor.Lifetime} for type {typeof(T)}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetInstance<T>(IDependencyContainer container, in BindingDescriptor descriptor)
        {
            IInstanceResolver instanceResolver = container.GetInstanceResolver(descriptor.InstanceIndex);
            if (!instanceResolver.TryResolve(out T instance))
            {
                throw new DIGenException($"Could not resolve type {typeof(T)}.");
            }

            return instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T InstantiateAndCache<T>(this IDependencyContainer source,
            IDependencyContainer target, in BindingDescriptor descriptor)
        {
            var instance = source.Instantiate<T>(descriptor);
            target.Cache(new InstanceResolver<T>(instance));
            return instance;
        }

        private static T Instantiate<T>(this IDependencyContainer container, in BindingDescriptor descriptor)
        {
            if (descriptor.ResolverIndex < 0)
            {
                throw new DIGenException($"Could not resolve type {typeof(T)}.");
            }

            IFactoryResolver factoryResolver = container.GetFactoryResolver(descriptor.ResolverIndex);
            return factoryResolver.Resolve<T>(container);
        }
    }
}