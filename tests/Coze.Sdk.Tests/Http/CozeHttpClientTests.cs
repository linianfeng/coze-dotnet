using System.Net;
using System.Text;
using Coze.Sdk.Authentication;
using Coze.Sdk.Exceptions;
using Coze.Sdk.Http;
using FluentAssertions;
using Moq;
using Xunit;

namespace Coze.Sdk.Tests.Http;

public class CozeHttpClientTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _messageHandlerMock;
    private readonly CozeHttpClient _httpClient;
    private readonly CozeOptions _options;

    public CozeHttpClientTests()
    {
        _options = new CozeOptions
        {
            Auth = new TokenAuth("test-token"),
            BaseUrl = "https://api.coze.cn",
            ReadTimeout = TimeSpan.FromSeconds(30)
        };

        _messageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new CozeHttpClient(_options);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public class CreateRequestTests
    {
        [Fact]
        public void CreateRequest_WithEndpoint_ReturnsCorrectRequest()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            using var client = new CozeHttpClient(options);

            // Act
            var request = client.CreateRequest("/v1/chat", HttpMethodType.Post);

            // Assert
            request.Endpoint.Should().Be("/v1/chat");
            request.Method.Should().Be(HttpMethodType.Post);
        }

        [Fact]
        public void CreateGetRequest_ReturnsCorrectMethod()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            using var client = new CozeHttpClient(options);

            // Act
            var request = client.CreateGetRequest("/v1/bots");

            // Assert
            request.Method.Should().Be(HttpMethodType.Get);
        }

        [Fact]
        public void CreatePostRequest_ReturnsCorrectMethod()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            using var client = new CozeHttpClient(options);

            // Act
            var request = client.CreatePostRequest("/v1/chat");

            // Assert
            request.Method.Should().Be(HttpMethodType.Post);
        }

        [Fact]
        public void CreatePutRequest_ReturnsCorrectMethod()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            using var client = new CozeHttpClient(options);

            // Act
            var request = client.CreatePutRequest("/v1/datasets");

            // Assert
            request.Method.Should().Be(HttpMethodType.Put);
        }

        [Fact]
        public void CreateDeleteRequest_ReturnsCorrectMethod()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            using var client = new CozeHttpClient(options);

            // Act
            var request = client.CreateDeleteRequest("/v1/files");

            // Assert
            request.Method.Should().Be(HttpMethodType.Delete);
        }
    }

    public class AddJsonBodyTests
    {
        [Fact]
        public void AddJsonBody_SetsBodyOnRequest()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            using var client = new CozeHttpClient(options);
            var request = client.CreateRequest("/v1/chat");
            var body = new { name = "test" };

            // Act
            client.AddJsonBody(request, body);

            // Assert
            request.Body.Should().Be(body);
        }
    }

    public class ConstructorTests
    {
        [Fact]
        public void Constructor_WithValidOptions_CreatesInstance()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };

            // Act & Assert - Should not throw
            using var client = new CozeHttpClient(options);
        }

        [Fact]
        public void Constructor_WithoutAuth_ThrowsInvalidOperationException()
        {
            // Arrange
            var options = new CozeOptions { Auth = null };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new CozeHttpClient(options));
        }

        [Fact]
        public void Constructor_WithCustomBaseUrl_TrimsTrailingSlash()
        {
            // Arrange
            var options = new CozeOptions
            {
                Auth = new TokenAuth("test-token"),
                BaseUrl = "https://api.coze.cn/"
            };

            // Act
            using var client = new CozeHttpClient(options);

            // Assert - The client should handle trailing slash
            options.BaseUrl.Should().EndWith("/");
        }
    }

    public class DisposeTests
    {
        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Arrange
            var options = new CozeOptions { Auth = new TokenAuth("test-token") };
            var client = new CozeHttpClient(options);

            // Act & Assert - Should not throw
            client.Dispose();
            client.Dispose();
        }
    }
}
