using Coze.Sdk.Authentication;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Authentication;

public class TokenAuthTests
{
    [Fact]
    public void Constructor_WithValidToken_SetsAccessToken()
    {
        // Arrange
        const string token = "test-token";

        // Act
        var auth = new TokenAuth(token);

        // Assert
        auth.GetToken().Should().Be(token);
    }

    [Fact]
    public void Constructor_WithNullToken_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new TokenAuth(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TokenType_ReturnsBearer()
    {
        // Arrange
        var auth = new TokenAuth("test-token");

        // Act & Assert
        auth.TokenType.Should().Be("Bearer");
    }
}
