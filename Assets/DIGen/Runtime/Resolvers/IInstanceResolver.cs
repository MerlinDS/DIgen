using System;

namespace DIGen.Runtime.Resolvers
{
    public interface IInstanceResolver : IDisposable
    {
        bool TryResolve<T>(out T instance);
    }
}