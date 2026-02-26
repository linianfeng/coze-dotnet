using Coze.Sdk.Http;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Http;

public class CozeHttpRequestTests
{
    [Fact]
    public void Constructor_CreatesEmptyRequest()
    {
        // Act
        var request = new CozeHttpRequest();

        // Assert
        request.Endpoint.Should().BeEmpty();
        request.Method.Should().Be(HttpMethodType.Post);
        request.QueryParameters.Should().BeEmpty();
        request.Headers.Should().BeEmpty();
        request.Body.Should().BeNull();
        request.RawContent.Should().BeNull();
    }

    [Fact]
    public void AddQueryParameter_AddsParameterToRequest()
    {
        // Arrange
        var request = new CozeHttpRequest();

        // Act
        var result = request.AddQueryParameter("key", "value");

        // Assert
        request.QueryParameters.Should().ContainKey("key");
        request.QueryParameters["key"].Should().Be("value");
        result.Should().BeSameAs(request); // Fluent interface
    }

    [Fact]
    public void AddQueryParameter_WithNullValue_AddsNullValue()
    {
        // Arrange
        var request = new CozeHttpRequest();

        // Act
        request.AddQueryParameter("key", null);

        // Assert
        request.QueryParameters.Should().ContainKey("key");
        request.QueryParameters["key"].Should().BeNull();
    }

    [Fact]
    public void AddQueryParameter_WithObjectValue_ConvertsToString()
    {
        // Arrange
        var request = new CozeHttpRequest();

        // Act
        request.AddQueryParameter("page", 123);

        // Assert
        request.QueryParameters["page"].Should().Be("123");
    }

    [Fact]
    public void AddHeader_AddsHeaderToRequest()
    {
        // Arrange
        var request = new CozeHttpRequest();

        // Act
        var result = request.AddHeader("X-Custom-Header", "value");

        // Assert
        request.Headers.Should().ContainKey("X-Custom-Header");
        request.Headers["X-Custom-Header"].Should().Be("value");
        result.Should().BeSameAs(request); // Fluent interface
    }

    [Fact]
    public void SetJsonBody_SetsBody()
    {
        // Arrange
        var request = new CozeHttpRequest();
        var body = new { Name = "Test", Value = 123 };

        // Act
        var result = request.SetJsonBody(body);

        // Assert
        request.Body.Should().Be(body);
        result.Should().BeSameAs(request); // Fluent interface
    }

    [Fact]
    public void HttpMethodType_AllValues_AreDefined()
    {
        // Assert
        Enum.GetValues<HttpMethodType>().Should().HaveCount(5);
        Enum.GetValues<HttpMethodType>().Should().Contain(
        [
            HttpMethodType.Get,
            HttpMethodType.Post,
            HttpMethodType.Put,
            HttpMethodType.Delete,
            HttpMethodType.Patch
        ]);
    }
}
