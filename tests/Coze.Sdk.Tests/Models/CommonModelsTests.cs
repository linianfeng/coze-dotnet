using Coze.Sdk.Models.Common;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class CommonModelsTests
{
    public class ApiResponseTests
    {
        [Fact]
        public void ApiResponse_WithSuccessData_SetsCorrectValues()
        {
            // Act
            var response = new ApiResponse<string>
            {
                Code = 0,
                Message = "success",
                Data = "test data"
            };

            // Assert
            response.Code.Should().Be(0);
            response.Message.Should().Be("success");
            response.Data.Should().Be("test data");
            response.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ApiResponse_WithError_SetsCorrectValues()
        {
            // Act
            var response = new ApiResponse<string>
            {
                Code = 4000,
                Message = "error",
                Detail = new ErrorDetail { LogId = "log-123" }
            };

            // Assert
            response.Code.Should().Be(4000);
            response.IsSuccess.Should().BeFalse();
            response.LogId.Should().Be("log-123");
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(4000, false)]
        [InlineData(5000, false)]
        public void ApiResponse_IsSuccess_ReturnsCorrectValue(int code, bool expectedSuccess)
        {
            // Act
            var response = new ApiResponse<string> { Code = code };

            // Assert
            response.IsSuccess.Should().Be(expectedSuccess);
        }
    }

    public class ErrorDetailTests
    {
        [Fact]
        public void ErrorDetail_WithProperties_SetsCorrectValues()
        {
            // Act
            var detail = new ErrorDetail
            {
                LogId = "log-123"
            };

            // Assert
            detail.LogId.Should().Be("log-123");
        }
    }

    public class PagedResponseTests
    {
        [Fact]
        public void PagedResponse_WithItems_SetsCorrectValues()
        {
            // Act
            var response = new PagedResponse<string>
            {
                Items = new List<string> { "item1", "item2", "item3" },
                Total = 3,
                HasMore = false
            };

            // Assert
            response.Items.Should().HaveCount(3);
            response.Total.Should().Be(3);
            response.HasMore.Should().BeFalse();
        }

        [Fact]
        public void PagedResponse_WithPagination_SetsCorrectValues()
        {
            // Act
            var response = new PagedResponse<string>
            {
                Items = new List<string> { "item1" },
                Total = 100,
                HasMore = true,
                FirstId = "first-123",
                LastId = "last-456"
            };

            // Assert
            response.HasMore.Should().BeTrue();
            response.FirstId.Should().Be("first-123");
            response.LastId.Should().Be("last-456");
        }
    }

    public class PageRequestTests
    {
        [Fact]
        public void PageRequest_WithDefaultValues_SetsCorrectValues()
        {
            // Act
            var request = new PageRequest();

            // Assert
            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(20);
        }

        [Fact]
        public void PageRequest_WithCustomValues_SetsCorrectValues()
        {
            // Act
            var request = new PageRequest
            {
                PageNumber = 5,
                PageSize = 100
            };

            // Assert
            request.PageNumber.Should().Be(5);
            request.PageSize.Should().Be(100);
        }
    }
}
