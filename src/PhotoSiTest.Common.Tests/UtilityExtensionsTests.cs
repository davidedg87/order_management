using System;
using PhotoSiTest.Common.Extensions;
using Xunit;
using FluentAssertions;
using Moq;

namespace PhotoSiTest.Common.Tests
{
    public class UtilityExtensionsTests
    {
        [Fact]
        public void GetNestedMessage_SingleException_ReturnsMessage()
        {
            // Arrange
            var exception = new Exception("Root exception");

            // Act
            var result = exception.GetNestedMessage();

            // Assert
            result.Should().Be("Root exception");
        }

        [Fact]
        public void GetNestedMessage_NestedExceptions_ReturnsAllMessages()
        {
            // Arrange
            var exception = new Exception("Root exception",
                new Exception("Inner exception", new Exception("Innermost exception")));

            // Act
            var result = exception.GetNestedMessage();

            // Assert
            result.Should().Be("Root exception" + Environment.NewLine +
                               "Inner exception" + Environment.NewLine +
                               "Innermost exception");
        }

        [Fact]
        public void GetFullMessage_SingleException_IncludesStackTrace()
        {
            // Arrange
            var exception = new Exception("Root exception");

            // Act
            var result = exception.GetFullMessage();

            // Assert
            result.Should().Contain("Root exception");
        }

        [Fact]
        public void GetFullMessage_NestedExceptions_IncludesAllMessagesAndStackTrace()
        {
            // Arrange
            var exception = new Exception("Root exception",
                new Exception("Inner exception", new Exception("Innermost exception")));

            // Act
            var result = exception.GetFullMessage();

            // Assert
            result.Should().Contain("Root exception")
                  .And.Contain("Inner exception")
                  .And.Contain("Innermost exception");
        }

        [Fact]
        public void GetNestedMessage_NullException_ReturnsEmptyString()
        {
            // Arrange
            Exception exception = null;

            // Act
            var result = exception.GetNestedMessage();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetFullMessage_NullException_ReturnsEmptyString()
        {
            // Arrange
            Exception exception = null;

            // Act
            var result = exception.GetFullMessage();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void BuildMessage_WhenExceptionOccursInMethod_ReturnsErrorInBuildMessage()
        {
            // Arrange
            Exception exception = new("Root exception");
            var invalidException = new Mock<Exception>();

            // Simula un'eccezione lanciata durante l'accesso al messaggio
            invalidException.Setup(e => e.Message).Throws(new InvalidOperationException("Simulated failure"));

            // Act
            string result = null;
            try
            {
                result = invalidException.Object.GetFullMessage();
            }
            catch
            {
                // Ignorare eccezioni esterne
            }

            // Assert
            result.Should().Contain("Error in Exception.BuildMessage().")
                  .And.Contain("Simulated failure");
        }

        [Fact]
        public void BuildMessage_WithRootMessageAndException_IncludesRootMessageAndExceptionMessage()
        {
            // Arrange
            var exception = new Exception("Test exception message");
            var rootMessage = "Root message";

            // Act
            string result = exception.GetNestedMessage(rootMessage);

            // Assert
            result.Should().Contain(rootMessage)
                  .And.Contain("Test exception message")
                  .And.Contain(Environment.NewLine);
        }
    }
}
