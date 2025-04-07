using DIGen.Runtime.Bindings;
using DIGen.Runtime.Resolvers;

namespace DIGen.Runtime.Base
{
    internal interface IDependencyContainer : IContainer
    {
        IDependencyContainer Parent { get; }
        bool TryGetDescriptor<T>(out BindingDescriptor descriptor);
        IFactoryResolver GetFactoryResolver(int lookupIndex);
        IInstanceResolver GetInstanceResolver(int lookupIndex);

        void Cache(IInstanceResolver instanceResolver);
    }
}