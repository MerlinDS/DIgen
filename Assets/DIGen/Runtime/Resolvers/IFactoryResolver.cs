using DIGen.Runtime.Base;

namespace DIGen.Runtime.Resolvers
{
    public interface IFactoryResolver
    {
        /// <summary>
        /// Resolves a dependency of type T.
        /// </summary>
        /// <returns></returns>
        T Resolve<T>(IContainer container);
    }
}