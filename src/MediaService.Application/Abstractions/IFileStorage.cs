namespace MediaService.Application.Abstractions;

public interface IFileStorage
{
    string ProviderName { get; }

    Task<StoredFileResult> SaveAsync(
        Guid tenantId,
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string bucketName, string objectKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string bucketName, string objectKey, CancellationToken cancellationToken = default);
}