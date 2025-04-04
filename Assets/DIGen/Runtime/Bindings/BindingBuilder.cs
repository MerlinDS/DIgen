using System;
using DIGen.Runtime.Base;

namespace DIGen.Runtime.Bindings
{
    public readonly ref struct BindingBuilder
    {
        private readonly ushort _lookupIndex;
        private readonly Span<BindingDescriptor> _descriptors;
        private readonly Span<IntPtr> _typeHandles;

        public static BindingBuilder ForResolver(int resolverIndex, Type type,
            Span<BindingDescriptor> descriptors, Span<IntPtr> typeHandles)
        {
            BindingDescriptor descriptor = BindingDescriptor.ForResolver(resolverIndex);
            return new BindingBuilder(0, type.TypeHandle.Value, descriptor, descriptors, typeHandles);
        }

        public static BindingBuilder ForInstance(int instanceIndex, Type type,
            Span<BindingDescriptor> descriptors, Span<IntPtr> typeHandles)
        {
            BindingDescriptor descriptor = BindingDescriptor.ForInstance(instanceIndex);
            return new BindingBuilder(0, type.TypeHandle.Value, descriptor, descriptors, typeHandles);
        }


        private BindingBuilder(ushort lookupIndex, in IntPtr typeHandle, in BindingDescriptor descriptor,
            Span<BindingDescriptor> descriptors, Span<IntPtr> typeHandles)
        {
            _lookupIndex = lookupIndex;
            _typeHandles = typeHandles;
            _descriptors = descriptors;

            _descriptors[_lookupIndex] = descriptor;
            _typeHandles[_lookupIndex] = typeHandle;
        }

        public BindingBuilder As<T>() where T : class
        {
            var lookupIndex = (ushort)(_lookupIndex + 1);
            IntPtr typeHandle = typeof(T).TypeHandle.Value;
            BindingDescriptor baseDescriptor = _descriptors[_lookupIndex];
            return new BindingBuilder(lookupIndex, typeHandle, baseDescriptor, _descriptors, _typeHandles);
        }

        public BindingBuilder With(Lifetime lifetime)
        {
            BindingDescriptor descriptor = _descriptors[_lookupIndex];
            descriptor = descriptor.WithLifetime(lifetime);
            _descriptors[_lookupIndex] = descriptor;
            return this;
        }

        public BindingBuilder IsLazy(bool isLazy)
        {
            BindingDescriptor descriptor = _descriptors[_lookupIndex];
            descriptor = descriptor.WithLazy(isLazy);
            _descriptors[_lookupIndex] = descriptor;
            return this;
        }
    }
}