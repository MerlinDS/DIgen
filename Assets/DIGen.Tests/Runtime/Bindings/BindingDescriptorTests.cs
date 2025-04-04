using System;
using DIGen.Runtime.Base;
using DIGen.Runtime.Bindings;
using FluentAssertions;
using NUnit.Framework;

namespace DIGen.Tests.Runtime.Bindings
{
    [TestFixture]
    [TestOf(typeof(BindingDescriptor))]
    public class BindingDescriptorTests
    {

        [Test]
        public void ForResolver_When_valid_index_provided_Should_create_descriptor_with_resolver_index()
        {
            //Arrange
            const int expectedIndex = 42;
        
            //Act
            BindingDescriptor descriptor = BindingDescriptor.ForResolver(expectedIndex);
        
            //Assert
            descriptor.ResolverIndex.Should().Be(expectedIndex);
            descriptor.InstanceIndex.Should().Be(-1);
            descriptor.Lifetime.Should().Be(Lifetime.Transient);
            descriptor.IsLazy.Should().BeTrue();
        }
        
        [Test]
        public void ForInstance_When_valid_index_provided_Should_create_descriptor_with_instance_index()
        {
            //Arrange
            const int expectedIndex = 42;
        
            //Act
            var descriptor = BindingDescriptor.ForInstance(expectedIndex);
        
            //Assert
            descriptor.InstanceIndex.Should().Be(expectedIndex);
            descriptor.ResolverIndex.Should().Be(-1);
            descriptor.Lifetime.Should().Be(Lifetime.Transient);
            descriptor.IsLazy.Should().BeTrue();
        }
        
        [Test]
        public void ForResolver_When_custom_lifetime_provided_Should_create_descriptor_with_custom_lifetime()
        {
            //Arrange
            const Lifetime expectedLifetime = Lifetime.Singleton;
        
            //Act
            BindingDescriptor descriptor = BindingDescriptor.ForResolver(42, expectedLifetime);
        
            //Assert
            descriptor.Lifetime.Should().Be(expectedLifetime);
        }
        
        [Test]
        public void ForInstance_When_custom_lifetime_provided_Should_create_descriptor_with_custom_lifetime()
        {
            //Arrange
            const Lifetime expectedLifetime = Lifetime.Singleton;
        
            //Act
            var descriptor = BindingDescriptor.ForInstance(42, expectedLifetime);
        
            //Assert
            descriptor.Lifetime.Should().Be(expectedLifetime);
        }
        
        [Test]
        public void ForResolver_When_lazy_flag_provided_Should_create_descriptor_with_specified_lazy_flag()
        {
            //Arrange
            const bool expectedIsLazy = false;
        
            //Act
            BindingDescriptor descriptor = BindingDescriptor.ForResolver(42, isLazy: expectedIsLazy);
        
            //Assert
            descriptor.IsLazy.Should().Be(expectedIsLazy);
        }
        
        [Test]
        public void ForInstance_When_lazy_flag_provided_Should_create_descriptor_with_specified_lazy_flag()
        {
            //Arrange
            const bool expectedIsLazy = false;
        
            //Act
            var descriptor = BindingDescriptor.ForInstance(42, isLazy: expectedIsLazy);
        
            //Assert
            descriptor.IsLazy.Should().Be(expectedIsLazy);
        }
        
        [Test]
        public void WithLifetime_When_new_lifetime_provided_Should_return_new_descriptor_with_updated_lifetime()
        {
            //Arrange
            BindingDescriptor original = BindingDescriptor.ForResolver(42);
            const Lifetime newLifetime = Lifetime.Singleton;
        
            //Act
            BindingDescriptor updated = original.WithLifetime(newLifetime);
        
            //Assert
            updated.Lifetime.Should().Be(newLifetime);
            updated.ResolverIndex.Should().Be(original.ResolverIndex);
            updated.InstanceIndex.Should().Be(original.InstanceIndex);
            updated.IsLazy.Should().Be(original.IsLazy);
        }
        
        [Test]
        public void WithLazy_When_new_lazy_flag_provided_Should_return_new_descriptor_with_updated_lazy_flag()
        {
            //Arrange
            BindingDescriptor original = BindingDescriptor.ForResolver(42);
            const bool newIsLazy = false;
        
            //Act
            BindingDescriptor updated = original.WithLazy(newIsLazy);
        
            //Assert
            updated.IsLazy.Should().Be(newIsLazy);
            updated.ResolverIndex.Should().Be(original.ResolverIndex);
            updated.InstanceIndex.Should().Be(original.InstanceIndex);
            updated.Lifetime.Should().Be(original.Lifetime);
        }
        
        [Test]
        public void ForResolver_When_negative_index_provided_Should_throw_argument_out_of_range_exception()
        {
            //Arrange
            const int invalidIndex = -1;
        
            //Act
            Action act = () => BindingDescriptor.ForResolver(invalidIndex);
        
            //Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("resolverIndex");
        }
        
        [Test]
        public void ForResolver_When_index_is_too_large_Should_throw_argument_out_of_range_exception()
        {
            //Arrange
            int invalidIndex = ushort.MaxValue;
        
            //Act
            Action act = () => BindingDescriptor.ForResolver(invalidIndex);
        
            //Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("resolverIndex");
        }
        
        [Test]
        public void ForInstance_When_negative_index_provided_Should_throw_argument_out_of_range_exception()
        {
            //Arrange
            const int invalidIndex = -1;
        
            //Act
            Action act = () => BindingDescriptor.ForInstance(invalidIndex);
        
            //Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("instanceIndex");
        }
        
        [Test]
        public void ForInstance_When_index_is_too_large_Should_throw_argument_out_of_range_exception()
        {
            //Arrange
            int invalidIndex = ushort.MaxValue;
        
            //Act
            Action act = () => BindingDescriptor.ForInstance(invalidIndex);
        
            //Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("instanceIndex");
        }
    }
}