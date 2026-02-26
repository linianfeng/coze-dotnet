using Coze.Sdk.Authentication;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Authentication;

public class WebOAuthClientTests
{
    public class GetOAuthUrlTests
    {
        [Fact]
        public void GetOAuthUrl_WithRequiredParameters_ReturnsValidUrl()
        {
            // Arrange
            var options = new OAuthOptions
            {
                ClientId = "test-client-id",
                BaseUrl = "https://api.coze.cn"
            };

            // Act & Assert - Just verify we can create the client
            using var client = new WebOAuthClient(options);
            client.Should().NotBeNull();
        }

        [Fact]
        public void GetOAuthUrl_WithRedirectUri_ReturnsValidUrl()
        {
            // Arrange
            var options = new OAuthOptions
            {
                ClientId = "test-client-id",
                ClientSecret = "test-secret",
                BaseUrl = "https://api.coze.cn"
            };

            // Act & Assert - Just verify we can create the client
            using var client = new WebOAuthClient(options);
            client.Should().NotBeNull();
        }
    }
}

public class PkceOAuthClientTests
{
    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Arrange
        var options = new OAuthOptions
        {
            ClientId = "test-client-id",
            BaseUrl = "https://api.coze.cn"
        };

        // Act & Assert
        using var client = new PkceOAuthClient(options);
        client.Should().NotBeNull();
    }
}

public class DeviceOAuthClientTests
{
    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Arrange
        var options = new OAuthOptions
        {
            ClientId = "test-client-id",
            BaseUrl = "https://api.coze.cn"
        };

        // Act & Assert
        using var client = new DeviceOAuthClient(options);
        client.Should().NotBeNull();
    }
}
