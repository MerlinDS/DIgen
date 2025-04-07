using System;
using System.Collections.Generic;
using System.Linq;
using DIGen.Runtime.Base;
using DIGen.Runtime.Bindings;
using DIGen.Runtime.Exceptions;
using DIGen.Runtime.Extensions;
using DIGen.Runtime.Resolvers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DIGen.Tests.Runtime.Extensions
{
    [TestFixture]
    [TestOf(typeof(ContainerExtensions))]
    public class ContainerExtensionsTests
    {
        [Test]
        public void Resolve_When_service_not_registered_Should_throw_exception()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            sut.Parent.Returns(_ => null);
            // Act
            Action act = () => sut.Resolve<IMockServiceA>();
            // Assert
            act.Should().Throw<DIGenException>()
                .WithMessage($"Service of type {typeof(IMockServiceA)} was not registered.");
        }

        [Test]
        public void Resolve_When_service_registered_as_transient_Should_create_new_instance_each_call()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            Init(sut, (typeof(IMockServiceA), BindingDescriptor.ForResolver(0)));

            // Act
            var actual = sut.Resolve<IMockServiceA>();
            // Assert
            actual.Should().NotBeNull();
            actual.Should().NotBeSameAs(sut.Resolve<IMockServiceA>());
        }

        [Test]
        public void
            Resolve_When_service_registered_as_scoped_and_called_in_scope_Should_create_single_instance_in_scope()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            Init(sut, (typeof(IMockServiceA), BindingDescriptor.ForResolver(0, Lifetime.Scoped)));
            // Act
            var actual = sut.Resolve<IMockServiceA>();
            // Assert
            actual.Should().NotBeNull();
            var expected = sut.Resolve<IMockServiceA>();
            actual.Should().Be(expected);
        }

        [Test]
        public void
            Resolve_When_service_registered_as_scoped_but_called_in_child_scope_Should_create_single_instance_in_child_scope()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            Init(sut, (typeof(IMockServiceA), BindingDescriptor.ForResolver(0, Lifetime.Scoped)));

            var child = Substitute.For<IDependencyContainer>();
            Init(child);
            child.Parent.Returns(sut);

            // Act
            var actual = child.Resolve<IMockServiceA>();
            // Assert
            actual.Should().NotBeNull();
            actual.Should().Be(child.Resolve<IMockServiceA>(),
                "Child scope should return the same instance");
            actual.Should().NotBe(sut.Resolve<IMockServiceA>(),
                "Parent scope should return a different instance");
        }


        [Test]
        public void Resolve_When_service_registered_as_scoped_but_called_in_parent_scope_Should_throw_exception()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            Init(sut, (typeof(MockServiceA), BindingDescriptor.ForResolver(0, Lifetime.Scoped)));

            var parent = Substitute.For<IDependencyContainer>();
            Init(parent);
            sut.Parent.Returns(parent);

            Action act = () => parent.Resolve<IMockServiceA>();
            // Act & Assert
            act.Should().Throw<DIGenException>()
                .WithMessage($"Service of type {typeof(IMockServiceA)} was not registered.");
        }

        [Test]
        public void
            Resolve_When_service_registered_as_singleton_and_called_in_root_scope_Should_create_single_instance_in_root_scope()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            Init(sut, (typeof(IMockServiceA), BindingDescriptor.ForResolver(0, Lifetime.Singleton)));
            // Act
            var actual = sut.Resolve<IMockServiceA>();
            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeAssignableTo<IMockServiceA>();
            actual.Should().Be(sut.Resolve<IMockServiceA>());
        }

        [Test]
        public void
            Resolve_When_service_registered_as_singleton_and_called_in_child_scope_Should_create_single_instance_in_root_scope()
        {
            // Arrange
            var sut = Substitute.For<IDependencyContainer>();
            Init(sut, (typeof(IMockServiceA), BindingDescriptor.ForResolver(0, Lifetime.Singleton)));

            var child = Substitute.For<IDependencyContainer>();
            Init(child);
            child.Parent.Returns(sut);
            // Act
            var actual = child.Resolve<IMockServiceA>();
            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeAssignableTo<IMockServiceA>();
            actual.Should().Be(sut.Resolve<IMockServiceA>());
        }

        private static void Init(IDependencyContainer container, params (Type, BindingDescriptor)[] lookup)
        {
            container.Parent.Returns(_ => null);
            Dictionary<IntPtr, BindingDescriptor> descriptors = lookup
                .ToDictionary(x => x.Item1.TypeHandle.Value, x => x.Item2);


            if (lookup.Length > 0)
            {
                int index = lookup.First().Item2.ResolverIndex;
                if (index >= 0)
                {
                    var factoryResolver = Substitute.For<IFactoryResolver>();
                    factoryResolver.Resolve<IMockServiceA>(Arg.Any<IContainer>())
                        .Returns(x =>
                        {
                            x.Arg<IContainer>().Should().Be(container);
                            return Substitute.For<MockServiceA>();
                        });

                    container.GetFactoryResolver(0).Returns(factoryResolver);
                }
            }

            IntPtr handleValue = typeof(IMockServiceA).TypeHandle.Value;
            container.TryGetDescriptor<IMockServiceA>(out _).Returns(x =>
            {
                bool result = descriptors.TryGetValue(handleValue, out BindingDescriptor descriptor);
                x[0] = descriptor;
                return result;
            });

            if (lookup.Length > 0)
            {
                int index = lookup.First().Item2.InstanceIndex;
                if (index >= 0)
                {
                    var instanceResolver = Substitute.For<IInstanceResolver>();
                    instanceResolver.TryResolve<IMockServiceA>(out _)
                        .Returns(x =>
                        {
                            x[0] = Substitute.For<MockServiceA>();
                            return true;
                        });

                    container.GetInstanceResolver(index).Returns(instanceResolver);
                }
            }


            container.When(x => x.Cache(Arg.Any<IInstanceResolver>()))
                .Do(x =>
                {
                    var resolver = x.Arg<IInstanceResolver>();
                    container.GetInstanceResolver(10).Returns(resolver);
                    if (descriptors.ContainsKey(handleValue))
                    {
                        BindingDescriptor descriptor = descriptors[handleValue];
                        descriptors[handleValue] = descriptor.WithInstanceIndex(10);
                        return;
                    }

                    descriptors.Add(handleValue, BindingDescriptor.ForInstance(10, Lifetime.Scoped));
                });
        }

        internal interface IMockServiceA
        {
        }

        internal abstract class MockServiceA : IMockServiceA
        {
        }
    }
}