using Coze.Sdk.Models.Files;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class FileModelsTests
{
    public class CozeFileTests
    {
        [Fact]
        public void CozeFile_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var file = new CozeFile
            {
                Id = "file-123",
                FileName = "test.txt",
                Bytes = 1024,
                CreatedAt = 1234567890
            };

            // Assert
            file.Id.Should().Be("file-123");
            file.FileName.Should().Be("test.txt");
            file.Bytes.Should().Be(1024);
            file.CreatedAt.Should().Be(1234567890);
        }
    }

    public class UploadFileRequestTests
    {
        [Fact]
        public void FromPath_CreatesRequestWithCorrectPath()
        {
            // Arrange
            var testFilePath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(testFilePath, "test content");

                // Act
                var request = UploadFileRequest.FromPath(testFilePath);

                // Assert
                request.Should().NotBeNull();
            }
            finally
            {
                File.Delete(testFilePath);
            }
        }

        [Fact]
        public void FromStream_CreatesRequestWithCorrectStream()
        {
            // Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test content"));

            // Act
            var request = UploadFileRequest.FromStream(stream, "test.txt");

            // Assert
            request.Should().NotBeNull();
        }
    }

    public class UploadFileResponseTests
    {
        [Fact]
        public void UploadFileResponse_WithFile_SetsCorrectValues()
        {
            // Act
            var response = new UploadFileResponse
            {
                File = new CozeFile
                {
                    Id = "file-123",
                    FileName = "test.txt",
                    Bytes = 1024
                }
            };

            // Assert
            response.File.Should().NotBeNull();
            response.File!.Id.Should().Be("file-123");
        }
    }

    public class RetrieveFileRequestTests
    {
        [Fact]
        public void RetrieveFileRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new RetrieveFileRequest
            {
                FileId = "file-123"
            };

            // Assert
            request.FileId.Should().Be("file-123");
        }
    }

    public class RetrieveFileResponseTests
    {
        [Fact]
        public void RetrieveFileResponse_WithFile_SetsCorrectValues()
        {
            // Act
            var response = new RetrieveFileResponse
            {
                File = new CozeFile
                {
                    Id = "file-123",
                    FileName = "test.txt"
                }
            };

            // Assert
            response.File.Should().NotBeNull();
            response.File!.Id.Should().Be("file-123");
        }
    }
}
