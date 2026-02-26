using Coze.Sdk.Models.Datasets;
using Coze.Sdk.Models.Common;

namespace Coze.Sdk.Clients;

/// <summary>
/// 数据集操作接口。
/// 对应 Java SDK 中的 DatasetService.java。
/// </summary>
public interface IDatasetClient
{
    /// <summary>
    /// 创建新数据集。
    /// </summary>
    Task<Dataset> CreateAsync(CreateDatasetRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出空间中的数据集。
    /// </summary>
    Task<ListDatasetsResponse> ListAsync(ListDatasetsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新数据集。
    /// </summary>
    Task<UpdateDatasetResponse> UpdateAsync(UpdateDatasetRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除数据集。
    /// </summary>
    Task DeleteAsync(string datasetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文档操作客户端。
    /// </summary>
    IDocumentClient Documents { get; }

    /// <summary>
    /// 获取图片操作客户端。
    /// </summary>
    IDatasetImageClient Images { get; }
}

/// <summary>
/// 文档操作接口。
/// 对应 Java SDK 中的 DocumentService.java。
/// </summary>
public interface IDocumentClient
{
    /// <summary>
    /// 创建新文档。
    /// </summary>
    Task<Document> CreateAsync(CreateDocumentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出数据集中的文档。
    /// </summary>
    Task<ListDocumentsResponse> ListAsync(ListDocumentsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新文档。
    /// </summary>
    Task<Document> UpdateAsync(UpdateDocumentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除文档。
    /// </summary>
    Task DeleteAsync(string datasetId, string documentId, CancellationToken cancellationToken = default);
}

/// <summary>
/// 数据集图片操作接口。
/// 对应 Java SDK 中的 DatasetImageAPI.java。
/// </summary>
public interface IDatasetImageClient
{
    /// <summary>
    /// 列出数据集中的图片。
    /// </summary>
    Task<ListDatasetImagesResponse> ListAsync(ListDatasetImagesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新数据集中的图片。
    /// </summary>
    Task<UpdateDatasetImageResponse> UpdateAsync(string datasetId, string documentId, UpdateDatasetImageRequest request, CancellationToken cancellationToken = default);
}
