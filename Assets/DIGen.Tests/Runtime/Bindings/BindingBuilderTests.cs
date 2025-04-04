using System;
using DIGen.Runtime.Base;
using DIGen.Runtime.Bindings;
using FluentAssertions;
using NUnit.Framework;

namespace DIGen.Tests.Runtime.Bindings
{
    [TestFixture]
    [TestOf(typeof(BindingBuilder))]
    public class BindingBuilderTests
    {
        [Test]
        public void ForResolver_When_called_Should_return_BindingBuilder_for_resolver()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[1];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[1];
            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles);
            BindingDescriptor actualDescriptor = descriptors[0];
            IntPtr actualTypeHandle = typeHandles[0];
            // Assert
            actualTypeHandle.Should().Be(type.TypeHandle.Value);
            actualDescriptor.ResolverIndex.Should().Be(10);
            actualDescriptor.InstanceIndex.Should().Be(-1, "instance index should be default");
            actualDescriptor.Lifetime.Should().Be(Lifetime.Transient, "lifetime should be default");
            actualDescriptor.IsLazy.Should().BeTrue("isLazy should be default");
        }

        [Test]
        public void ForInstance_When_called_Should_return_BindingBuilder_for_instance()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[1];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[1];
            // Act
            BindingBuilder.ForInstance(10, type, descriptors, typeHandles);
            BindingDescriptor actualDescriptor = descriptors[0];
            IntPtr actualTypeHandle = typeHandles[0];
            // Assert
            actualTypeHandle.Should().Be(type.TypeHandle.Value);
            actualDescriptor.InstanceIndex.Should().Be(10);
            actualDescriptor.ResolverIndex.Should().Be(-1, "resolver index should be default");
            actualDescriptor.Lifetime.Should().Be(Lifetime.Transient, "lifetime should be default");
            actualDescriptor.IsLazy.Should().BeTrue("isLazy should be default");
        }

        [Test]
        public void As_When_type_is_interface_Should_return_BindingBuilder_for_interface()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];
            BindingBuilder bindingBuilder = BindingBuilder.ForResolver(10, type, descriptors, typeHandles);
            // Act
            bindingBuilder.As<IMockService>();
            BindingDescriptor actualDescriptor = descriptors[1];
            IntPtr actualTypeHandle = typeHandles[1];
            // Assert
            actualTypeHandle.Should().Be(typeof(IMockService).TypeHandle.Value);
            actualDescriptor.ResolverIndex.Should().Be(10);
            actualDescriptor.InstanceIndex.Should().Be(-1, "instance index should be default");
            actualDescriptor.Lifetime.Should().Be(Lifetime.Transient, "lifetime should be default");
            actualDescriptor.IsLazy.Should().BeTrue("isLazy should be default");
        }

        [Test]
        public void As_When_type_is_class_Should_return_BindingBuilder_for_class()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];
            BindingBuilder bindingBuilder = BindingBuilder.ForResolver(10, type, descriptors, typeHandles);
            // Act
            bindingBuilder.As<MockService>();
            BindingDescriptor actualDescriptor = descriptors[1];
            IntPtr actualTypeHandle = typeHandles[1];
            // Assert
            actualTypeHandle.Should().Be(typeof(MockService).TypeHandle.Value);
            actualDescriptor.ResolverIndex.Should().Be(10);
            actualDescriptor.InstanceIndex.Should().Be(-1, "instance index should be default");
            actualDescriptor.Lifetime.Should().Be(Lifetime.Transient, "lifetime should be default");
            actualDescriptor.IsLazy.Should().BeTrue("isLazy should be default");
        }

        [TestCase(Lifetime.Transient)]
        [TestCase(Lifetime.Scoped)]
        [TestCase(Lifetime.Singleton)]
        public void With(Lifetime lifetime)
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[1];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[1];
            BindingBuilder bindingBuilder = BindingBuilder.ForResolver(10, type, descriptors, typeHandles);
            // Act
            bindingBuilder.With(lifetime);
            BindingDescriptor actualDescriptor = descriptors[0];
            // Assert
            actualDescriptor.Lifetime.Should().Be(lifetime, "lifetime should be set");
        }


        [Test]
        public void With_When_called_after_As_Should_apply_lifetime_to_the_last_binding()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];

            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles)
                .As<IMockService>()
                .With(Lifetime.Scoped);

            // Assert
            descriptors[1].Lifetime.Should().Be(Lifetime.Scoped);
        }

        [Test]
        public void With_When_called_before_As_Should_apply_lifetime_to_the_first_binding_and_inherit_to_the_second()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];

            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles)
                .With(Lifetime.Scoped)
                .As<IMockService>();

            // Assert
            descriptors[0].Lifetime.Should().Be(Lifetime.Scoped);
            descriptors[1].Lifetime.Should().Be(Lifetime.Scoped);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsLazy(bool isLazy)
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[1];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[1];
            BindingBuilder bindingBuilder = BindingBuilder.ForResolver(10, type, descriptors, typeHandles);
            // Act
            bindingBuilder.IsLazy(isLazy);
            BindingDescriptor actualDescriptor = descriptors[0];
            // Assert
            actualDescriptor.IsLazy.Should().Be(isLazy, "isLazy should be set");
        }

        [Test]
        public void IsLazy_When_called_after_As_Should_apply_isLazy_to_the_last_binding()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];

            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles)
                .As<IMockService>()
                .IsLazy(false);

            // Assert
            descriptors[1].IsLazy.Should().BeFalse();
        }

        [Test]
        public void IsLazy_When_called_before_As_Should_apply_isLazy_to_the_first_binding_and_inherit_to_the_second()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];

            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles)
                .IsLazy(false)
                .As<IMockService>();

            // Assert
            descriptors[0].IsLazy.Should().BeFalse();
            descriptors[1].IsLazy.Should().BeFalse();
        }

        [Test]
        public void ChainedMethods_When_chaining_multiple_methods_Should_apply_all_configurations()
        {
            // Arrange
            Type type = typeof(MockService);
            Span<IntPtr> typeHandles = stackalloc IntPtr[2];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[2];

            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles)
                .With(Lifetime.Scoped)
                .IsLazy(false)
                .As<IMockService>()
                .With(Lifetime.Singleton);

            // Assert
            // Check original binding
            descriptors[0].ResolverIndex.Should().Be(10);
            descriptors[0].Lifetime.Should().Be(Lifetime.Scoped);
            descriptors[0].IsLazy.Should().BeFalse();

            // Check interface binding
            descriptors[1].ResolverIndex.Should().Be(10);
            descriptors[1].Lifetime.Should().Be(Lifetime.Singleton);
            descriptors[1].IsLazy.Should().BeFalse();
            typeHandles[1].Should().Be(typeof(IMockService).TypeHandle.Value);
        }


        [Test]
        public void MultipleAs_When_bind_for_several_types_Should_create_bindings_for_all_types()
        {
            // Arrange
            Type type = typeof(MultiInterfaceMock);
            Span<IntPtr> typeHandles = stackalloc IntPtr[3];
            Span<BindingDescriptor> descriptors = stackalloc BindingDescriptor[3];

            // Act
            BindingBuilder.ForResolver(10, type, descriptors, typeHandles)
                .As<IInterface1>()
                .As<IInterface2>();

            // Assert
            typeHandles[0].Should().Be(typeof(MultiInterfaceMock).TypeHandle.Value);
            typeHandles[1].Should().Be(typeof(IInterface1).TypeHandle.Value);
            typeHandles[2].Should().Be(typeof(IInterface2).TypeHandle.Value);

            // All should have the same resolver index
            descriptors[0].ResolverIndex.Should().Be(10);
            descriptors[1].ResolverIndex.Should().Be(10);
            descriptors[2].ResolverIndex.Should().Be(10);
        }


        private interface IInterface1
        {
        }

        private interface IInterface2
        {
        }

        private class MultiInterfaceMock : IInterface1, IInterface2
        {
        }

        private class MockService : IMockService
        {
        }

        private interface IMockService
        {
        }
    }
}