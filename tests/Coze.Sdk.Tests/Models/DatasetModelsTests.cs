using Coze.Sdk.Models.Datasets;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class DatasetModelsTests
{
    public class DatasetTests
    {
        [Fact]
        public void Dataset_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var dataset = new Dataset
            {
                DatasetId = "ds-123",
                Name = "Test Dataset",
                Description = "A test dataset",
                SpaceId = "space-456",
                Status = DatasetStatus.Enabled,
                FormatType = DocumentFormatType.Text,
                CanEdit = true,
                DocCount = 10
            };

            // Assert
            dataset.DatasetId.Should().Be("ds-123");
            dataset.Name.Should().Be("Test Dataset");
            dataset.Description.Should().Be("A test dataset");
            dataset.SpaceId.Should().Be("space-456");
            dataset.Status.Should().Be(DatasetStatus.Enabled);
            dataset.FormatType.Should().Be(DocumentFormatType.Text);
            dataset.CanEdit.Should().BeTrue();
            dataset.DocCount.Should().Be(10);
        }

        [Fact]
        public void Dataset_WithFileList_SetsFileList()
        {
            // Act
            var dataset = new Dataset
            {
                DatasetId = "ds-123",
                FileList = new List<string> { "file1.txt", "file2.txt" }
            };

            // Assert
            dataset.FileList.Should().HaveCount(2);
            dataset.FileList.Should().Contain("file1.txt");
        }
    }

    public class CreateDatasetRequestTests
    {
        [Fact]
        public void CreateDatasetRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateDatasetRequest
            {
                Name = "New Dataset",
                SpaceId = "space-123"
            };

            // Assert
            request.Name.Should().Be("New Dataset");
            request.SpaceId.Should().Be("space-123");
        }

        [Fact]
        public void CreateDatasetRequest_WithOptionalProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateDatasetRequest
            {
                Name = "New Dataset",
                SpaceId = "space-123",
                Description = "Dataset description",
                FormatType = DocumentFormatType.Table
            };

            // Assert
            request.Description.Should().Be("Dataset description");
            request.FormatType.Should().Be(DocumentFormatType.Table);
        }
    }

    public class ListDatasetsRequestTests
    {
        [Fact]
        public void ListDatasetsRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new ListDatasetsRequest
            {
                SpaceId = "space-123"
            };

            // Assert
            request.SpaceId.Should().Be("space-123");
            request.PageNumber.Should().Be(1); // Default value
            request.PageSize.Should().Be(20); // Default value
        }

        [Fact]
        public void ListDatasetsRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new ListDatasetsRequest
            {
                SpaceId = "space-123",
                PageNumber = 2,
                PageSize = 50,
                Name = "search-term"
            };

            // Assert
            request.PageNumber.Should().Be(2);
            request.PageSize.Should().Be(50);
            request.Name.Should().Be("search-term");
        }
    }

    public class UpdateDatasetRequestTests
    {
        [Fact]
        public void UpdateDatasetRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new UpdateDatasetRequest
            {
                DatasetId = "ds-123"
            };

            // Assert
            request.DatasetId.Should().Be("ds-123");
        }

        [Fact]
        public void UpdateDatasetRequest_WithOptionalProperties_SetsCorrectValues()
        {
            // Act
            var request = new UpdateDatasetRequest
            {
                DatasetId = "ds-123",
                Name = "Updated Name",
                Description = "Updated description"
            };

            // Assert
            request.Name.Should().Be("Updated Name");
            request.Description.Should().Be("Updated description");
        }
    }

    public class DeleteDatasetRequestTests
    {
        [Fact]
        public void DeleteDatasetRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new DeleteDatasetRequest
            {
                DatasetId = "ds-123"
            };

            // Assert
            request.DatasetId.Should().Be("ds-123");
        }
    }

    public class ListDatasetsResponseTests
    {
        [Fact]
        public void ListDatasetsResponse_WithDatasets_SetsCorrectValues()
        {
            // Act
            var response = new ListDatasetsResponse
            {
                Datasets = new List<Dataset>
                {
                    new Dataset { DatasetId = "ds-1", Name = "Dataset 1" },
                    new Dataset { DatasetId = "ds-2", Name = "Dataset 2" }
                },
                Total = 2
            };

            // Assert
            response.Datasets.Should().HaveCount(2);
            response.Total.Should().Be(2);
        }
    }

    public class DatasetStatusTests
    {
        [Theory]
        [InlineData(DatasetStatus.Unknown, 0)]
        [InlineData(DatasetStatus.Enabled, 1)]
        [InlineData(DatasetStatus.Disabled, 3)]
        public void DatasetStatus_HasCorrectValues(DatasetStatus status, int expectedValue)
        {
            // Assert
            ((int)status).Should().Be(expectedValue);
        }
    }

    public class DocumentFormatTypeTests
    {
        [Theory]
        [InlineData(DocumentFormatType.Text, 0)]
        [InlineData(DocumentFormatType.Table, 1)]
        [InlineData(DocumentFormatType.Image, 2)]
        public void DocumentFormatType_HasCorrectValues(DocumentFormatType formatType, int expectedValue)
        {
            // Assert
            ((int)formatType).Should().Be(expectedValue);
        }
    }
}
