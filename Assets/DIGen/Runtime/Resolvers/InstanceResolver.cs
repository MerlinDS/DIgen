using System;

namespace DIGen.Runtime.Resolvers
{
    public readonly struct InstanceResolver<TType> : IInstanceResolver
    {
        private readonly TType _instance;

        public InstanceResolver(TType instance)
        {
            _instance = instance;
        }

        public bool TryResolve<T>(out T instance)
        {
            if (_instance is T typedInstance)
            {
                instance = typedInstance;
                return true;
            }

            instance = default;
            return false;
        }

        public void Dispose()
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}