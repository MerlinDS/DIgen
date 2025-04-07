using System;
using DIGen.Runtime.Base;
using DIGen.Runtime.Exceptions;

namespace DIGen.Runtime.Resolvers
{
    public sealed class FactoryResolver<TType> : IFactoryResolver
    {
        private readonly Func<IContainer, TType> _factory;

        public FactoryResolver(Func<IContainer, TType> factory)
        {
            _factory = factory;
        }

        public T Resolve<T>(IContainer container)
        {
            if (_factory.Invoke(container) is T result)
            {
                return result;
            }

            throw new DIGenException($"Type {typeof(T)} not registered.");
        }
    }
}