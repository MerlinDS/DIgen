using DIGen.Runtime;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DIGen.Tests.Runtime
{
    [TestFixture]
    [TestOf(typeof(Temp))]
    public class TempTests
    {

        [Test]
        public void Test()
        {
            // Arrange
            var temp = Substitute.For<Temp>();
            temp.IsRunning.Returns(true);
            
            // Act
            bool actual = temp.Run();
            
            // Assert
            actual.Should().BeTrue();
        }
    }
}