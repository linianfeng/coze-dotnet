using Coze.Sdk.Authentication;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Authentication;

public class OAuthOptionsTests
{
    [Fact]
    public void Constructor_WithRequiredProperties_SetsCorrectValues()
    {
        // Act
        var options = new OAuthOptions
        {
            ClientId = "client-123"
        };

        // Assert
        options.ClientId.Should().Be("client-123");
        options.BaseUrl.Should().Be("https://api.coze.cn");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(50));
    }

    [Fact]
    public void Constructor_WithAllProperties_SetsCorrectValues()
    {
        // Act
        var options = new OAuthOptions
        {
            ClientId = "client-123",
            ClientSecret = "secret-456",
            BaseUrl = "https://custom.api.com",
            WwwUrl = "https://custom.www.com",
            Timeout = TimeSpan.FromSeconds(100)
        };

        // Assert
        options.ClientId.Should().Be("client-123");
        options.ClientSecret.Should().Be("secret-456");
        options.BaseUrl.Should().Be("https://custom.api.com");
        options.WwwUrl.Should().Be("https://custom.www.com");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(100));
    }
}

public class OAuthTokenTests
{
    [Fact]
    public void OAuthToken_WithAllProperties_SetsCorrectValues()
    {
        // Act
        var token = new OAuthToken
        {
            AccessToken = "access-123",
            RefreshToken = "refresh-456",
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Scope = "read write"
        };

        // Assert
        token.AccessToken.Should().Be("access-123");
        token.RefreshToken.Should().Be("refresh-456");
        token.ExpiresIn.Should().Be(3600);
        token.TokenType.Should().Be("Bearer");
        token.Scope.Should().Be("read write");
    }

    [Fact]
    public void IsExpired_WithFutureExpiry_ReturnsFalse()
    {
        // Act
        var token = new OAuthToken
        {
            ExpiresIn = 3600
        };

        // Assert - Token should not be expired immediately after creation
        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void LogId_CanBeSet()
    {
        // Act
        var token = new OAuthToken
        {
            AccessToken = "access-123",
            LogId = "log-789"
        };

        // Assert
        token.LogId.Should().Be("log-789");
    }
}

public class DeviceAuthResponseTests
{
    [Fact]
    public void DeviceAuthResponse_WithAllProperties_SetsCorrectValues()
    {
        // Act
        var response = new DeviceAuthResponse
        {
            DeviceCode = "device-123",
            UserCode = "ABCD-EFGH",
            VerificationUri = "https://verify.coze.cn",
            ExpiresIn = 600,
            Interval = 5
        };

        // Assert
        response.DeviceCode.Should().Be("device-123");
        response.UserCode.Should().Be("ABCD-EFGH");
        response.VerificationUri.Should().Be("https://verify.coze.cn");
        response.ExpiresIn.Should().Be(600);
        response.Interval.Should().Be(5);
    }

    [Fact]
    public void DeviceAuthResponse_VerificationUrl_CanBeSet()
    {
        // Act
        var response = new DeviceAuthResponse
        {
            VerificationUri = "https://verify.coze.cn",
            UserCode = "ABCD-EFGH",
            VerificationUrl = "https://verify.coze.cn?user_code=ABCD-EFGH"
        };

        // Assert
        response.VerificationUrl.Should().Be("https://verify.coze.cn?user_code=ABCD-EFGH");
    }
}

public class PkceAuthUrlResponseTests
{
    [Fact]
    public void PkceAuthUrlResponse_WithAllProperties_SetsCorrectValues()
    {
        // Act
        var response = new PkceAuthUrlResponse
        {
            AuthorizationUrl = "https://auth.coze.cn/authorize?code=xyz",
            CodeVerifier = "verifier-123"
        };

        // Assert
        response.AuthorizationUrl.Should().Be("https://auth.coze.cn/authorize?code=xyz");
        response.CodeVerifier.Should().Be("verifier-123");
    }
}

public class CodeChallengeMethodTests
{
    [Theory]
    [InlineData(CodeChallengeMethod.Plain)]
    [InlineData(CodeChallengeMethod.S256)]
    public void CodeChallengeMethod_AllValues_AreDefined(CodeChallengeMethod method)
    {
        // Assert
        ((int)method).Should().BeGreaterOrEqualTo(0);
    }
}
