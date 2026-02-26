using Coze.Sdk.Exceptions;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Exceptions;

public class CozeExceptionTests
{
    [Fact]
    public void CozeException_WithMessage_SetsMessage()
    {
        // Act
        var exception = new CozeException("Test error");

        // Assert
        exception.Message.Should().Be("Test error");
        exception.LogId.Should().BeNull();
    }

    [Fact]
    public void CozeException_WithMessageAndLogId_SetsProperties()
    {
        // Act
        var exception = new CozeException("Test error", "log-123");

        // Assert
        exception.Message.Should().Be("Test error");
        exception.LogId.Should().Be("log-123");
    }

    [Fact]
    public void CozeException_WithInnerException_SetsInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new CozeException("Test error", innerException);

        // Assert
        exception.Message.Should().Be("Test error");
        exception.InnerException.Should().Be(innerException);
    }
}

public class CozeApiExceptionTests
{
    [Fact]
    public void Constructor_WithAllParameters_SetsProperties()
    {
        // Act
        var exception = new CozeApiException(400, 1001, "Bad request", "log-456", "{\"error\":true}");

        // Assert
        exception.StatusCode.Should().Be(400);
        exception.ErrorCode.Should().Be(1001);
        exception.Message.Should().Be("Bad request");
        exception.LogId.Should().Be("log-456");
        exception.RawResponse.Should().Be("{\"error\":true}");
    }

    [Fact]
    public void Constructor_WithInnerException_SetsProperties()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner");

        // Act
        var exception = new CozeApiException(500, 5000, "Server error", innerException, "log-789", "{}");

        // Assert
        exception.StatusCode.Should().Be(500);
        exception.ErrorCode.Should().Be(5000);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [InlineData(400, 1001)]
    [InlineData(401, 4000)]
    [InlineData(500, 5000)]
    public void Constructor_WithDifferentStatusCodes_SetsStatusCode(int statusCode, int errorCode)
    {
        // Act
        var exception = new CozeApiException(statusCode, errorCode, "Error");

        // Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.ErrorCode.Should().Be(errorCode);
    }
}

public class CozeAuthExceptionTests
{
    [Fact]
    public void Constructor_WithAllParameters_SetsProperties()
    {
        // Act
        var exception = new CozeAuthException(AuthErrorCode.InvalidClient, "Invalid client", "log-111", 401);

        // Assert
        exception.ErrorCode.Should().Be(AuthErrorCode.InvalidClient);
        exception.Message.Should().Be("Invalid client");
        exception.LogId.Should().Be("log-111");
        exception.StatusCode.Should().Be(401);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsProperties()
    {
        // Arrange
        var innerException = new Exception("Inner");

        // Act
        var exception = new CozeAuthException(AuthErrorCode.ServerError, "Server error", innerException);

        // Assert
        exception.ErrorCode.Should().Be(AuthErrorCode.ServerError);
        exception.InnerException.Should().Be(innerException);
    }

    [Theory]
    [InlineData(AuthErrorCode.InvalidClient)]
    [InlineData(AuthErrorCode.InvalidGrant)]
    [InlineData(AuthErrorCode.AccessDenied)]
    [InlineData(AuthErrorCode.ServerError)]
    [InlineData(AuthErrorCode.Unknown)]
    public void Constructor_WithDifferentErrorCodes_SetsErrorCode(AuthErrorCode errorCode)
    {
        // Act
        var exception = new CozeAuthException(errorCode, "Error");

        // Assert
        exception.ErrorCode.Should().Be(errorCode);
    }
}
