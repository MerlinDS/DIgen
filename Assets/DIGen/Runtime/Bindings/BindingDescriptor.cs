using System;
using System.Runtime.InteropServices;
using DIGen.Runtime.Base;

namespace DIGen.Runtime.Bindings
{
    /// <summary>
    /// Describes a binding of a type to a resolver or an instance in the container.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 6)]
    public readonly struct BindingDescriptor
    {
        [FieldOffset(0)]
        private readonly ushort _resolverIndex;

        [FieldOffset(2)]
        private readonly ushort _instanceIndex;

        [FieldOffset(4)]
        private readonly byte _lifetime;

        [FieldOffset(5)]
        private readonly byte _isLazy;


        public static BindingDescriptor ForResolver(int resolverIndex, Lifetime lifetime = Lifetime.Transient,
            bool isLazy = true)
        {
            if (resolverIndex is < 0 or >= ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(resolverIndex),
                    $"Resolver index must be between 0 and {ushort.MaxValue - 1}");
            }

            return new BindingDescriptor((ushort)resolverIndex, ushort.MaxValue,
                (byte)lifetime, (byte)(isLazy ? 1 : 0));
        }

        public static BindingDescriptor ForInstance(int instanceIndex, Lifetime lifetime = Lifetime.Transient,
            bool isLazy = true)
        {
            if (instanceIndex is < 0 or >= ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceIndex),
                    $"Instance index must be between 0 and {ushort.MaxValue - 1}");
            }

            return new BindingDescriptor(ushort.MaxValue, (ushort)instanceIndex,
                (byte)lifetime, (byte)(isLazy ? 1 : 0));
        }

        private BindingDescriptor(ushort resolverIndex, ushort instanceIndex, byte lifetime, byte isLazy)
        {
            _resolverIndex = resolverIndex;
            _instanceIndex = instanceIndex;
            _lifetime = lifetime;
            _isLazy = isLazy;
        }

        /// <summary>
        /// The index of the resolver in the container.
        /// </summary>
        public int ResolverIndex => _resolverIndex >= ushort.MaxValue ? -1 : _resolverIndex;

        /// <summary>
        /// The index of the instance in the container.
        /// </summary>
        public int InstanceIndex => _instanceIndex >= ushort.MaxValue ? -1 : _instanceIndex;

        /// <summary>
        /// Lifetime of the instance in the container.
        /// </summary>
        /// <seealso cref="Lifetime"/>
        public Lifetime Lifetime => (Lifetime)_lifetime;

        /// <summary>
        /// Whether the instantiation is lazy (will be created on first access) or not (will be created on container creation).
        /// </summary>
        public bool IsLazy => _isLazy == 1;

        internal BindingDescriptor WithLifetime(Lifetime lifetime)
        {
            return new BindingDescriptor(_resolverIndex, _instanceIndex, (byte)lifetime, _isLazy);
        }

        internal BindingDescriptor WithLazy(bool isLazy)
        {
            return new BindingDescriptor(_resolverIndex, _instanceIndex, _lifetime, (byte)(isLazy ? 1 : 0));
        }
    }
}